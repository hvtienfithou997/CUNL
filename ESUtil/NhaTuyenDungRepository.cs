using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESUtil
{
    public class NhaTuyenDungRepository : IESRepository
    {
        #region Init

        private int max_token = 10;
        protected static string _default_index;

        //protected new ElasticClient client;
        private static NhaTuyenDungRepository instance;

        public NhaTuyenDungRepository(string modify_index)
        {
            _default_index = !string.IsNullOrEmpty(modify_index) ? modify_index : _default_index;
            ConnectionSettings settings = new ConnectionSettings(connectionPool).DefaultIndex(_default_index).DisableDirectStreaming(true);

            settings.MaximumRetries(10);
            client = new ElasticClient(settings);
            var ping = client.Ping(p => p.Pretty(true));
            if (ping.ServerError != null && ping.ServerError.Error != null)
            {
                throw new Exception("START ES FIRST");
            }
        }

        public static NhaTuyenDungRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_nha_tuyen_dung";
                    instance = new NhaTuyenDungRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(3)).Map<NhaTuyenDung>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<NhaTuyenDung> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<NhaTuyenDung>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public string Index(NhaTuyenDung employer, out string token)
        {
            token = "";
            string _token = Nanoid.Nanoid.Generate(size: max_token);
            string id = $"{employer.id_nha_tuyen_dung}";
            employer.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            int retry = 0;
            while (IsTokenExist(_token, employer.app_id) && retry++ < 10)
            {
                Task.Delay(TimeSpan.FromSeconds(1));
                _token = Nanoid.Nanoid.Generate(size: max_token);
            }
            if (retry >= 10)
            {
                return "";
            }
            try
            {
                employer.token = _token;
            }
            catch (Exception)
            {
            }
            token = employer.token;
            return Index(_default_index, employer);
        }

        private bool IsTokenExist(string token, string app_id)
        {
            var re = client.Count<NhaTuyenDung>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("token").Value(token))));
            return re.Count > 0;
        }

        public bool Update(NhaTuyenDung employer)
        {
            string id = $"{employer.id}";

            employer.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, employer, id);
        }

        public bool Delete(string id)
        {
            return Delete<NhaTuyenDung>(_default_index, id);
        }

        public NhaTuyenDung GetById(string id)
        {
            var obj = GetById<NhaTuyenDung>(_default_index, id);
            if (obj != null)
            {
                return obj;
            }
            return null;
        }

        private NhaTuyenDung ConvertDoc(IHit<NhaTuyenDung> hit)
        {
            NhaTuyenDung tuyen_dung = new NhaTuyenDung();

            try
            {
                tuyen_dung = hit.Source;
                tuyen_dung.id = hit.Id;
            }
            catch
            {
            }
            return tuyen_dung;
        }

        public NhaTuyenDung GetByToken(string token)
        {
            var re = client.Search<NhaTuyenDung>(s => s.Query(q => q.Term(t => t.Field("token.keyword").Value(token))
            && !q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))).Size(1));
            if (re.Total > 0)
            {
                NhaTuyenDung tuyen_dung = re.Hits.First().Source;
                tuyen_dung.id = re.Hits.First().Id;
                return re.Documents.First();
            }
            return null;
        }

        public List<NhaTuyenDung> GetMany(IEnumerable<string> lst_id)
        {
            List<NhaTuyenDung> lst = new List<NhaTuyenDung>();
            lst_id = lst_id.Where(x => !string.IsNullOrEmpty(x));
            if (lst_id.Count() > 0)
            {
                var re = client.GetMany<NhaTuyenDung>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        var ntd = item.Source;
                        ntd.id = item.Id;
                        lst.Add(ntd);
                    }
                }
            }
            return lst;
        }

        public List<NhaTuyenDung> Search(string term, IEnumerable<int> thuoc_tinh, string app_id, string nguoi_tao, int page, string token, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<NhaTuyenDung> lst = new List<NhaTuyenDung>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();

                if (!is_admin)
                {
                    must.Add(new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao });
                }
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                if (thuoc_tinh.Count() > 0)
                {
                    must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NHA_TUYEN_DUNG });
                    must.Add(new TermsQuery()
                    {
                        Field = "thuoc_tinh",
                        Terms = thuoc_tinh.Select(x => (object)x)
                    });
                }
                //var lst_job_is_owner = JobRepository.Instance.GetAllJobsIsOwner(app_id, nguoi_tao).Select(x => x.id_job);

                must.Add(new TermQuery() { Field = "token.keyword", Value = token });
                //must.Add(new TermsQuery() { Field = "id_job.keyword", Terms = lst_job_is_owner });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!string.IsNullOrEmpty(term))
                {
                    must.Add(new QueryStringQuery() { Fields = new string[] { "id_nha_tuyen_dung" }, Query = term });
                }
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "id_nha_tuyen_dung.keyword", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NhaTuyenDung>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public List<NhaTuyenDung> GetNhaTuyenDungByLstIdShare(string app_id, IEnumerable<string> lst_id_share, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<NhaTuyenDung> lst = new List<NhaTuyenDung>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                must.Add(new TermsQuery() { Field = "lst_id_share.keyword", Terms = lst_id_share });

                //var lst_job_is_owner = JobRepository.Instance.GetAllJobsIsOwner(app_id, nguoi_tao).Select(x => x.id_job);
                //must.Add(new TermsQuery() { Field = "id_job.keyword", Terms = lst_job_is_owner });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "id_nha_tuyen_dung.keyword", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NhaTuyenDung>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public List<NhaTuyenDung> GetNhaTuyenDungByIdJob(string app_id, IEnumerable<string> lst_id_job, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<NhaTuyenDung> lst = new List<NhaTuyenDung>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                must.Add(new TermsQuery() { Field = "id_job.keyword", Terms = lst_id_job });

                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "id_nha_tuyen_dung.keyword", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NhaTuyenDung>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public List<NhaTuyenDung> GetNhaTuyenDungByIdObj(string app_id, IEnumerable<string> lst_id_obj, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<NhaTuyenDung> lst = new List<NhaTuyenDung>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                must.Add(new TermsQuery() { Field = "id_obj.keyword", Terms = lst_id_obj });

                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "id_nha_tuyen_dung.keyword", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NhaTuyenDung>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public List<NhaTuyenDung> GetAllNhaTuyenDung(string term, IEnumerable<int> thuoc_tinh, string app_id, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<NhaTuyenDung> lst = new List<NhaTuyenDung>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();

                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                if (thuoc_tinh.Count() > 0)
                {
                    must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NHA_TUYEN_DUNG });
                    must.Add(new TermsQuery()
                    {
                        Field = "thuoc_tinh",
                        Terms = thuoc_tinh.Select(x => (object)x)
                    });
                }
                //var lst_job_is_owner = JobRepository.Instance.GetAllJobsIsOwner(app_id, nguoi_tao).Select(x => x.id_job);

                //must.Add(new TermsQuery() { Field = "id_job.keyword", Terms = lst_job_is_owner });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!string.IsNullOrEmpty(term))
                {
                    must.Add(new QueryStringQuery() { Fields = new string[] { "id_nha_tuyen_dung" }, Query = term });
                }
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "id_nha_tuyen_dung.keyword", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NhaTuyenDung>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public NhaTuyenDung GetByIdJob(string id_job)
        {

            var re = client.Search<NhaTuyenDung>(s => s.Query(q => q.Term(t => t.Field("id_job.keyword").Value(id_job))).Size(50));
            if (re.Total > 0)
            {
                NhaTuyenDung em = re.Hits.First().Source;
                em.id = re.Hits.First().Id;
                return re.Documents.First();
            }
            return null;
        }

        public List<NhaTuyenDung> GetListNtdByIdJob(string id_job)
        {
            var lst = new List<NhaTuyenDung>();
            var re = client.Search<NhaTuyenDung>(s => s.Source().Size(1000).Query(q =>
                    q.Term(t => t.Field("id_job.keyword").Value(id_job))
                    && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))));
            if (re.Total > 0)
            {
                lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            }

            return lst;
        }

    }
}