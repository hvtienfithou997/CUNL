using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESUtil
{
    public class CongTyRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static CongTyRepository instance;

        public CongTyRepository(string modify_index)
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

        public static CongTyRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_cong_ty";
                    instance = new CongTyRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(3)).Map<CongTy>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        private bool IsAutoIdExist(string auto_id, string app_id)
        {
            var re = client.Count<CongTy>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("auto_id").Value(auto_id))));
            return re.Count > 0;
        }

        public IEnumerable<CongTy> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            List<QueryContainer> must_not = new List<QueryContainer>();
            must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
            must.Add(new MatchAllQuery());
            return GetObjectScroll<CongTy>(_default_index, new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not }), new SourceFilter());
        }

        public IEnumerable<CongTy> GetAll(string app_id, string term, int page_index, int page_size)
        {
            var re = client.Search<CongTy>(s => s.Query(q => q.QueryString(qs => qs.Fields(new string[] { "ten_cong_ty", "dia_chi", "ma_so_thue", "dien_thoai", "so_dkkd" }).Query(term)) && q.Term(t => t.Field("app_id.keyword").Value(app_id)) && !q.Term(t => t.Field("trang_thai").Value(QLCUNL.Models.TrangThai.DELETED))).Size(page_size).From(page_size * (page_index - 1)));
            List<CongTy> lc = new List<CongTy>();
            foreach (var item in re.Hits)
            {
                lc.Add(ConvertDoc(item));
            }

            return lc;
        }

        private int GenAutoId(string app_id, int max_id_job = 6)
        {
            int auto_id = 0;
            string _auto_id = Nanoid.Nanoid.Generate("0123456789", max_id_job);
            int retry = 0;
            while (IsAutoIdExist(_auto_id, app_id) && retry++ < 10)
            {
                Task.Delay(TimeSpan.FromSeconds(1));
                _auto_id = Nanoid.Nanoid.Generate("0123456789", max_id_job);
            }
            if (retry >= 10)
            {
                return -1;
            }
            try
            {
                auto_id = Convert.ToInt32(_auto_id);
            }
            catch (Exception)
            {
            }
            return auto_id;
        }

        public string Index(CongTy data)
        {
            data.auto_id = GenAutoId(data.app_id);
            if (data.auto_id > 0)
            {
                return Index<CongTy>(_default_index, data);
            }
            return "";
        }

        public bool IndexCongTy(CongTy cong_ty, out string id_cong_ty)
        {
            cong_ty.auto_id = GenAutoId(cong_ty.app_id);
            id_cong_ty = "";
            if (cong_ty.auto_id > 0)
            {
                var req = new IndexRequest<CongTy>();
                req.Document = cong_ty;
                var re = client.Index<CongTy>(req);
                if (re.Result == Result.Created)
                {
                    id_cong_ty = re.Id;
                    return true;
                }
            }
            return false;
        }

        public bool Update(CongTy data)
        {
            string id = $"{data.id_cong_ty}";
            data.id_cong_ty = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);

            return Update(_default_index, data, id);
        }

        public bool Delete(string id)
        {
            return Delete<CongTy>(_default_index, id);
        }

        public CongTy GetById(string id)
        {
            var obj = GetById<CongTy>(_default_index, id);
            if (obj != null)
            {
                obj.id_cong_ty = id;
                return obj;
            }
            return null;
        }

        private CongTy ConvertDoc(IHit<CongTy> hit)
        {
            CongTy u = new CongTy();

            try
            {
                u = hit.Source;
                u.id_cong_ty = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<CongTy> SearchAll(string app_id, string nguoi_tao, int group, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<CongTy> lst = new List<CongTy>();
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

                    //Check quyền view ở các bản ghi khác
                    List<PhanQuyen> lst_pq =
                        PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.CONG_TY, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

                    must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                }
                List<ISort> sort = new List<ISort>();

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<CongTy>(req);
                if (res.IsValid)
                {
                    total_recs = client.Search<CongTy>(s => s.MatchAll().Size(9999).From(0)).Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }

        public Tuple<List<CongTy>, Dictionary<int, long>> Search(string app_id, string nguoi_tao, int group, string term, IEnumerable<int> thuoc_tinh,
            IEnumerable<string> lst_id, IEnumerable<string> lst_id_should, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, bool thong_ke = false)
        {
            msg = "";
            total_recs = 0;
            List<CongTy> lst = new List<CongTy>(); Dictionary<int, long> dic_thong_ke = new Dictionary<int, long>();
            try
            {
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must_not.Add(new TermQuery() { Field = "thuoc_tinh", Value = -1 });
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                if (!string.IsNullOrEmpty(term))
                {
                    if (ValidateQuery(term))
                    {
                        must.Add(new QueryStringQuery() { Fields = new string[] { "ten_cong_ty" }, Query = term });
                    }
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
                        Values = lst_id.Where(x => !string.IsNullOrEmpty(x)).Select(x => (Id)x)
                    });
                }
                if (!is_admin)
                {
                    if (string.IsNullOrEmpty(nguoi_tao))
                    {
                        nguoi_tao = "__NULL__";
                    }

                    //Check quyền view ở các bản ghi khác
                    List<PhanQuyen> lst_pq = PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.CONG_TY, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });
                    var lst_id_query = new List<Id>();

                    if (lst_pq.Count > 0)
                    {
                        lst_id_query = lst_pq.Select(x => (Id)x.obj_id).ToList();
                    }
                    if (lst_id_should != null && lst_id_should.Count() > 0)
                    {
                        lst_id_should = lst_id_should.Where(x => !string.IsNullOrEmpty(x));
                        foreach (var item in lst_id_should)
                        {
                            lst_id_query.Add(new Id(item));
                        }
                    }
                    must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_id_query }
                    );
                }
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.TrackTotalHits = true;
                req.Size = page_size;
                req.Sort = sort;
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
                req.TrackTotalHits = true;
                var res = client.Search<CongTy>(req);
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
            return new Tuple<List<CongTy>, Dictionary<int, long>>(lst, dic_thong_ke);
        }

        public bool IsOwner(string id, string nguoi_tao)
        {
            return IsOwner<CongTy>(_default_index, id, nguoi_tao);
        }

        public List<CongTy> GetMany(IEnumerable<string> lst_id)
        {
            lst_id = lst_id.Where(x => !string.IsNullOrEmpty(x));
            List<CongTy> lst = new List<CongTy>();
            if (lst_id.Count() > 0)
            {
                var re = GetMany<CongTy>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        item.Source.id_cong_ty = item.Id;
                        lst.Add(item.Source);
                    }
                }
            }
            return lst;
        }

        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<CongTy>(id, thuoc_tinh);
        }

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<CongTy>(app_id);
        }

        public CongTy GetCongTyByName(string app_id, string ten_cong_ty)
        {
            var re = client.Search<CongTy>(s => s.Query(q =>
                q.Term(t => t.Field("app_id.keyword").Value(app_id))
                && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                && q.Term(t => t.Field("ten_cong_ty.keyword").Value(ten_cong_ty))
            ));
            if (re.Total > 0)
            {
                var cong_ty = re.Hits.Select(x => ConvertDoc(x)).First();
                return cong_ty;
            }
            return null;
        }

        //var re = client.Search<CongTy>(s => s.Query(q => q.QueryString(qs => qs.Fields(new string[] { "ten_cong_ty", "dia_chi", "ma_so_thue", "dien_thoai", "so_dkkd" }).Query(term)) && q.Term(t => t.Field("app_id.keyword").Value(app_id)) && !q.Term(t => t.Field("trang_thai").Value(QLCUNL.Models.TrangThai.DELETED))).Size(page_size).From(page_size * (page_index - 1)));

        public CongTy GetCongTyByNameV2(string app_id, string ten_cong_ty)
        {
            var re = client.Search<CongTy>(s => s.Query(q => q.QueryString(qs => qs.Fields(new string[] { "ten_cong_ty", "dia_chi", "ma_so_thue", "dien_thoai", "so_dkkd" }).Query(ten_cong_ty)) && q.Term(t => t.Field("app_id.keyword").Value(app_id)) && !q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))));
            if (re.Total > 0)
            {
                var cong_ty = re.Hits.Select(x => ConvertDoc(x)).First();
                return cong_ty;
            }
            return null;
        }

        public bool IsExistTenCongTy(string app_id, string ten_cong_ty)
        {
            var cong_ty = GetCongTyByName(app_id, ten_cong_ty);
            return cong_ty != null;
        }

        public int IndexMany(List<CongTy> data)
        {
            var bulk = new BulkDescriptor();
            foreach (var item in data)
            {
                item.auto_id = GenAutoId(item.app_id);
                string id = $"{item.id_cong_ty}";

                if (client.Search<NoteUngVienJob>(s => s.Size(0).Query(q =>
                        q.Term((t => t.Field("trang_thai").Value((TrangThai.ACTIVE)))) &&
                        q.Ids(i => i.Values(id)))).Total <= 0)
                {
                    item.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    item.ngay_tao = item.ngay_sua;
                    item.id_cong_ty = id;
                    bulk.Index<CongTy>(i => i.Id(id).Document(item));
                }
            }
            var re = client.Bulk(bulk);
            var count = re.ItemsWithErrors.Count();
            return data.Count - count;
        }
    }
}