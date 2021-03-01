using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESUtil
{
    public class NoteUngVienJobRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static NoteUngVienJobRepository instance;

        public NoteUngVienJobRepository(string modify_index)
        {
            _default_index = !string.IsNullOrEmpty(modify_index) ? modify_index : _default_index;
            ConnectionSettings settings = new ConnectionSettings(connectionPool, sourceSerializer: Nest.JsonNetSerializer.JsonNetSerializer.Default).DefaultIndex(_default_index).DisableDirectStreaming(true);
            settings.MaximumRetries(10);
            client = new ElasticClient(settings);
            var ping = client.Ping(p => p.Pretty(true));
            if (ping.ServerError != null && ping.ServerError.Error != null)
            {
                throw new Exception("START ES FIRST");
            }
        }

        public static NoteUngVienJobRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_note_ung_vien_job";
                    instance = new NoteUngVienJobRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(5)).Map<NoteUngVienJob>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<NoteUngVienJob> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<NoteUngVienJob>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public string Index(NoteUngVienJob data)
        {
            string id = $"{data.id_user_job}_{data.id_job}_{data.id_ung_vien}";
            data.id_note_ung_vien_job = id;
            if (Index(_default_index, data, "", id))
            {
                return id;
            }
            return "";
        }

        public int IndexMany(List<NoteUngVienJob> data)
        {
            var bulk = new BulkDescriptor();
            foreach (var item in data)
            {
                string id = $"{item.id_user_job}_{item.id_job}_{item.id_ung_vien}";
                if (client.Search<NoteUngVienJob>(s => s.Size(0).Query(q =>
                        q.Term((t => t.Field("trang_thai").Value((TrangThai.ACTIVE)))) &&
                        q.Ids(i => i.Values(id)))).Total <= 0)
                {
                    item.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    item.ngay_tao = item.ngay_sua;
                    item.id_note_ung_vien_job = id;
                    bulk.Index<NoteUngVienJob>(i => i.Id(id).Document(item));
                }
            }
            var re = client.Bulk(bulk);
            var count = re.ItemsWithErrors.Count();
            return data.Count - count;
        }

        public bool Update(NoteUngVienJob data)
        {
            string id = $"{data.id_note_ung_vien_job}";
            data.id_note_ung_vien_job = id;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }

        public bool Delete(string id)
        {
            return Delete<NoteUngVienJob>(_default_index, id);
        }

        public bool UpdateThuocTinh(NoteUngVienJob data)
        {
            string id = $"{data.id_note_ung_vien_job}";
            data.id_note_ung_vien_job = id;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            var re = client.Update<NoteUngVienJob, object>(id, u => u.Doc(new { data.thuoc_tinh, data.ngay_sua, data.nguoi_sua }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }

        public bool UpdateGioPhongVan(NoteUngVienJob data)
        {
            string id = $"{data.id_note_ung_vien_job}";
            data.id_note_ung_vien_job = id;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            var re = client.Update<NoteUngVienJob, object>(id, u => u.Doc(new { data.ngay_gio_phong_van, data.ngay_sua, data.nguoi_sua }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }

        public NoteUngVienJob GetById(string id)
        {
            var obj = GetById<NoteUngVienJob>(_default_index, id);
            if (obj != null)
            {
                obj.id_note_ung_vien_job = id;
                return obj;
            }
            return null;
        }
        public bool IsOwner(string id, string nguoi_tao)
        {
            return IsOwner<NoteUngVienJob>(_default_index, id, nguoi_tao);
        }
        private NoteUngVienJob ConvertDoc(IHit<NoteUngVienJob> hit)
        {
            NoteUngVienJob u = new NoteUngVienJob();

            try
            {
                u = hit.Source;
                u.id_note_ung_vien_job = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public Tuple<List<NoteUngVienJob>, Dictionary<int, long>> Search(string app_id, string nguoi_tao, int group, string term, string id_user, string id_job, string id_ung_vien,
            long ngay_gio_phong_van_from, long ngay_gio_phong_van_to, long ngay_di_lam_from, long ngay_di_lam_to,
            double luong_thu_viec_from, long luong_thu_viec_to, double luong_chinh_thuc_from, long luong_chinh_thuc_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
            int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null, bool thong_ke = false)
        {
            Dictionary<int, long> dic_thong_ke = new Dictionary<int, long>();
            msg = "";
            total_recs = 0;
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!is_admin)
                {
                    if (string.IsNullOrEmpty(nguoi_tao))
                    {
                        nguoi_tao = "__NULL__";
                    }
                    List<PhanQuyen> lst_pq =
                       PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.NOTE_UNG_VIEN_JOB, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

                    must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                }
                if (!string.IsNullOrEmpty(term))
                {
                    if (ValidateQuery(term))
                        must.Add(new QueryStringQuery() { Fields = new string[] { "ghi_chu" }, Query = term });
                }
                if (!string.IsNullOrEmpty(id_user))
                {
                    must.Add(new TermQuery() { Field = "id_user.keyword", Value = id_user });
                }
                if (!string.IsNullOrEmpty(id_job))
                {
                    must.Add(new TermQuery() { Field = "id_job.keyword", Value = id_job });
                }
                if (!string.IsNullOrEmpty(id_ung_vien))
                {
                    must.Add(new TermQuery() { Field = "id_ung_vien.keyword", Value = id_ung_vien });
                }
                if (ngay_gio_phong_van_from > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_gio_phong_van", GreaterThanOrEqualTo = ngay_gio_phong_van_from });
                }
                if (ngay_gio_phong_van_to > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_gio_phong_van", LessThanOrEqualTo = ngay_gio_phong_van_to });
                }
                if (ngay_di_lam_from > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_di_lam", GreaterThanOrEqualTo = ngay_di_lam_from });
                }
                if (ngay_di_lam_to > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_di_lam", LessThanOrEqualTo = ngay_di_lam_to });
                }
                if (luong_thu_viec_from > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "luong_thu_viec", GreaterThanOrEqualTo = luong_thu_viec_from });
                }
                if (luong_thu_viec_to > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "luong_thu_viec", LessThanOrEqualTo = luong_thu_viec_to });
                }
                if (luong_chinh_thuc_from > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "luong_chinh_thuc", GreaterThanOrEqualTo = luong_chinh_thuc_from });
                }
                if (luong_chinh_thuc_to > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "luong_chinh_thuc", LessThanOrEqualTo = luong_chinh_thuc_to });
                }
                if (thuoc_tinh.Count() > 0)
                {
                    must.Add(new TermsQuery()
                    {
                        Field = "thuoc_tinh",
                        Terms = thuoc_tinh.Select(x => (object)x)
                    });
                }
                if (lst_id.Count() > 0)
                {
                    must.Add(new IdsQuery()
                    {
                        Values = lst_id.Select(x => (Id)x)
                    });
                }
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                if (fields == null)
                    fields = new string[] { "id_note_ung_vien_job", "id_ung_vien", "id_job", "id_user_job" };
                req.Source = new SourceFilter() { Includes = fields };
                req.TrackTotalHits = true;
                if (thong_ke)
                {
                    req.Aggregations = new AggregationDictionary
                    {
                        {
                        "thuoc_tinh", new TermsAggregation("tt")
                            {
                                Field="thuoc_tinh",Size=9999
                            }
                        }
                    };
                }
                var res = client.Search<NoteUngVienJob>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                    if (thong_ke)
                    {
                        foreach (var item in res.Aggregations.Terms("thuoc_tinh").Buckets)
                        {
                            dic_thong_ke.Add(Convert.ToInt32(item.Key), item.DocCount.GetValueOrDefault());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return new Tuple<List<NoteUngVienJob>, Dictionary<int, long>>(lst, dic_thong_ke);
        }


        public List<string> GetListIdUngVienByIdUserJob(string app_id, string id_user_job, int page, out long total_recs, out string msg, int page_size, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!string.IsNullOrEmpty(id_user_job))
                {
                    must.Add(new TermQuery() { Field = "id_user_job.keyword", Value = id_user_job });
                }

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Source = new SourceFilter() { Includes = new string[] { "id_ung_vien" } };
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVienJob>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    return res.Documents.Select(x => x.id_ung_vien).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return new List<string>();
        }

        public Dictionary<string, int> GetListIdUngVienByIdUserJobNew(string app_id, IEnumerable<string> id_user_job)
        {
            
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                
                must.Add(new TermsQuery() { Field = "id_user_job.keyword", Terms = id_user_job });               

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = 9999;
                req.Source = new SourceFilter() { Includes = new string[] { "id_ung_vien", "id_user_job"} };
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVienJob>(req);
                if (res.IsValid)
                {
                    return res.Documents.GroupBy(x => x.id_user_job).ToDictionary(x => x.Key, y => y.Count());                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new Dictionary<string, int>();
        }


        public List<NoteUngVienJob> GetNoteUngVienJobByIdUserJob(string app_id, IEnumerable<string> id_user_job, IEnumerable<int> thuoc_tinh, int page, out long total_recs, out string msg, int page_size, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            if (id_user_job.Count() == 0)
                return lst;
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermsQuery() { Field = "id_user_job.keyword", Terms = id_user_job });
                if (thuoc_tinh.Count() > 0)
                {
                    must.Add(new TermsQuery() { Field = "thuoc_tinh", Terms = thuoc_tinh.Select(x => (object)x) });
                }

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVienJob>(req);
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

        // Get note ứng viên job by id ứng viên

        public List<NoteUngVienJob> GetNoteUngVienJobByIdUngVien(string app_id, IEnumerable<string> id_ung_vien, string id_job, int page, out long total_recs, out string msg, int page_size, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            if (id_ung_vien.Count() == 0)
                return lst;
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermsQuery() { Field = "id_ung_vien.keyword", Terms = id_ung_vien });
                if (!string.IsNullOrEmpty(id_job))
                {
                    must.Add(new TermQuery() { Field = "id_job.keyword", Value = id_job });
                }

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVienJob>(req);
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

        public List<NoteUngVienJob> GetNoteUngVienByIdUngVien(string app_id, IEnumerable<string> id_ung_vien, int page, out long total_recs, out string msg, int page_size, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            if (id_ung_vien.Count() == 0)
                return lst;
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermsQuery() { Field = "id_ung_vien.keyword", Terms = id_ung_vien });
                

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVienJob>(req);
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

        public List<NoteUngVienJob> Searchbyidungvien(string app_id, string id_ung_vien, string user, out long total_recs, out string msg)
        {
            msg = "";
            total_recs = 0;
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();

                if (!string.IsNullOrEmpty(id_ung_vien))
                {
                    must.Add(new TermQuery() { Value = id_ung_vien, Field = "id_ung_vien.keyword" });
                }
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                must.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.ACTIVE });
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must });

                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVienJob>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).Where(x => x.id_ung_vien == id_ung_vien && x.nguoi_tao == user).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }
        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<NoteUngVienJob>(id, thuoc_tinh);
        }
        public List<NoteUngVienJob> GetNoteUngVienJobByIdJobOwner(string app_id, string id_job)
        {
            var re = client.Search<NoteUngVienJob>(s => s.Query(
                q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
                && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                && q.Term(t => t.Field("id_job.keyword").Value(id_job))
                ));
            return re.Hits.Select(x => ConvertDoc(x)).ToList();
        }
        public bool IsNoteUngVienJobExistInJob(string app_id, string id_job, string id_ung_vien)
        {
            var re = client.Search<NoteUngVienJob>(s => s.Query(
                q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
                && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                && q.Term(t => t.Field("id_job.keyword").Value(id_job))
                && q.Term(t => t.Field("id_ung_vien.keyword").Value(id_ung_vien))
                ).Size(0));
            return re.Total > 0;
        }
        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<NoteUngVienJob>(app_id);
        }
        public Dictionary<string, Dictionary<int, long>> ThongKeTrangThaiUngVien(string app_id, string nguoi_tao,
            List<int> trang_thai_can_thong_ke, IEnumerable<string> ids_job, IEnumerable<string> ids_job_owner,
            bool is_admin)
        {
            Dictionary<string, Dictionary<int, long>> dic = new Dictionary<string, Dictionary<int, long>>();
            if (ids_job != null && ids_job.Count() > 0)
            {
                try
                {
                    List<QueryContainer> lst_must = new List<QueryContainer>();
                    lst_must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                    lst_must.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.ACTIVE });
                    //lst_must.Add(new TermsQuery() { Field = "thuoc_tinh", Terms = trang_thai_can_thong_ke.Select(x => (object)x) });
                    lst_must.Add(new TermsQuery() { Field = "id_job.keyword", Terms = ids_job });

                    if (!is_admin)
                    {
                        if (string.IsNullOrEmpty(nguoi_tao))
                        {
                            nguoi_tao = "__NULL__";
                        }

                        lst_must.Add(new BoolQuery()
                        {
                            Should = new List<QueryContainer>() { new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao },
                                new TermsQuery() {  Field= "id_job.keyword",Terms = ids_job_owner } }
                        });
                    }

                    var qc = new QueryContainer(
                            new BoolQuery() { Must = lst_must }
                        );


                    var re = client.Search<NoteUngVienJob>(s => s.Query(
                       q => qc
                    ).Size(0).Aggregations(
                    ag => ag.Terms("group_by_id_job", t => t.Field("id_job.keyword").Size(30).Aggregations(agg => agg.Terms("group_by_thuoc_tinh", tt => tt.Field("thuoc_tinh").Size(9999))))
                    ));

                    var a = re.Aggregations.Terms("group_by_id_job");

                    foreach (var item in a.Buckets)
                    {

                        var a2 = item.Terms("group_by_thuoc_tinh");
                        var dic_thong_ke = new Dictionary<int, long>();
                        //Tổng số không phụ thuộc giá trị
                        dic_thong_ke.Add(0, item.DocCount.GetValueOrDefault());
                        if (!dic.ContainsKey(item.Key))
                        {
                            dic.Add(item.Key, dic_thong_ke);
                        }
                        else
                        {
                            var _dic_thong_ke = dic[item.Key];
                            if (!_dic_thong_ke.ContainsKey(0))
                            {
                                _dic_thong_ke.Add(0, item.DocCount.GetValueOrDefault());
                            }
                        }
                        foreach (var item2 in a2.Buckets)
                        {

                            int key = Convert.ToInt32(item2.Key);
                            if (trang_thai_can_thong_ke.Contains(key))
                            {
                                dic_thong_ke.Add(key, item2.DocCount.GetValueOrDefault());

                                if (!dic.ContainsKey(item.Key))
                                {
                                    dic.Add(item.Key, dic_thong_ke);
                                }
                                else
                                {
                                    var _dic_thong_ke = dic[item.Key];
                                    foreach (var tk in dic_thong_ke)
                                    {
                                        if (!_dic_thong_ke.ContainsKey(tk.Key))
                                        {
                                            _dic_thong_ke.Add(tk.Key, tk.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            return dic;
        }
        public Dictionary<string, long> ThongKeUngVienTheoJob(string app_id, IEnumerable<string> ids_ung_vien)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            if (ids_ung_vien.Count() > 0)
            {
                try
                {
                    List<QueryContainer> lst_must = new List<QueryContainer>();
                    lst_must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                    lst_must.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.ACTIVE });
                    lst_must.Add(new TermsQuery() { Field = "id_ung_vien.keyword", Terms = ids_ung_vien });


                    var qc = new QueryContainer(
                            new BoolQuery() { Must = lst_must }
                        );


                    var re = client.Search<NoteUngVienJob>(s => s.Query(
                       q => qc).Size(0).Aggregations(ag => ag.Terms("group_by_id_ung_vien", t => t.Field("id_ung_vien.keyword").Size(ids_ung_vien.Count()))
                    ));

                    var a = re.Aggregations.Terms("group_by_id_ung_vien");

                    foreach (var item in a.Buckets)
                    {
                        dic.Add(item.Key, item.DocCount.GetValueOrDefault());
                    }
                }
                catch (Exception)
                {

                }


            }
            return dic;
        }
        public List<NoteUngVienJob> GetMany(IEnumerable<string> lst_id)
        {
            List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
            if (lst_id != null && lst_id.Count() > 0)
            {
                var re = client.GetMany<NoteUngVienJob>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        item.Source.id_note_ung_vien_job = item.Id;
                        lst.Add(item.Source);
                    }
                }
            }
            return lst;
        }

        public bool UpdateGhiChuNhaTuyenDung(string id, string ghi_chu_nha_tuyen_dung, out string msg)
        {
            msg = "";
            var ghi_chu = new NoteUngVienJob();
            ghi_chu.id_note_ung_vien_job = $"{id}";
            var re = client.Update<NoteUngVienJob, object>(ghi_chu.id_note_ung_vien_job, u => u.Doc(new { ghi_chu_nha_tuyen_dung = ghi_chu_nha_tuyen_dung }));
            return re.Result == Result.Updated || re.Result == Result.Noop;          

        }
        public bool UpdateGhiChuUngVien(string id, string ghi_chu_ung_vien, out string msg)
        {
            msg = "";
            var ghi_chu = new NoteUngVienJob();
            ghi_chu.id_note_ung_vien_job = $"{id}";
            var re = client.Update<NoteUngVienJob, object>(ghi_chu.id_note_ung_vien_job, u => u.Doc(new { ghi_chu_ung_vien = ghi_chu_ung_vien }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
           
        }
    }
}