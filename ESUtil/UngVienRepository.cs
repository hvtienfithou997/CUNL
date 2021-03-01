using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESUtil
{
    public class UngVienRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static UngVienRepository instance;

        public UngVienRepository(string modify_index)
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

        public static UngVienRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_ung_vien";
                    instance = new UngVienRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(5)).Map<UngVien>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<UngVien> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer> { new MatchAllQuery() };
            return GetObjectScroll<UngVien>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public bool Index(UngVien data)
        {
            int retry = 0; int max_retry = 5;
            bool need_retry = true;
            while (retry++ < max_retry && need_retry)
            {
                need_retry = !Index(_default_index, data, "");
                if (need_retry)
                    Task.Delay(1000).Wait();
            }
            return !need_retry;
        }

        public string IndexRetId(UngVien data)
        {
            string id = Index(_default_index, data);
            return id;
        }

        public bool Update(UngVien data)
        {
            string id = $"{data.id_ung_vien}";
            data.id_ung_vien = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            data.ngay_tao = -Int32.MaxValue;
            return Update(_default_index, data, id);
        }

        public bool Delete(string id)
        {
            return Delete<UngVien>(_default_index, id);
        }

        public UngVien GetById(string id)
        {
            var obj = GetById<UngVien>(_default_index, id);
            if (obj != null)
            {
                obj.id_ung_vien = id;
                return obj;
            }
            return null;
        }

        private UngVien ConvertDoc(IHit<UngVien> hit)
        {
            UngVien u = new UngVien();

            try
            {
                u = hit.Source;
                u.id_ung_vien = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public Tuple<List<UngVien>, Dictionary<int, long>> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, long ngay_di_lam_from, long ngay_di_lam_to,
            double luong_mong_muon_from, double luong_mong_muon_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
            long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, IEnumerable<string> lst_id_neq = null,
            Dictionary<string, bool> sort_order = null, bool thong_ke = false)
        {
            Dictionary<int, long> dic_thong_ke = new Dictionary<int, long>();
            msg = "";
            total_recs = 0;
            List<UngVien> lst = new List<UngVien>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!is_admin)
                {
                    ///Ứng viên có thể được nhìn thấy bằng cách Chia sẻ, tự tạo hoặc cùng 1 Team
                    if (string.IsNullOrEmpty(nguoi_tao))
                    {
                        nguoi_tao = "__NULL__";
                    }
                    List<PhanQuyen> lst_pq =
                       PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.UNG_VIEN, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

                    must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                }
                if (lst_id.Count() > 0)
                {
                    must.Add(new IdsQuery()
                    {
                        Values = lst_id.Select(x => (Id)x)
                    });
                }
                if (!string.IsNullOrEmpty(term))
                {
                    if (ValidateQuery(term))
                    {
                        must.Add(new QueryStringQuery() { Fields = new string[] { "ho_ten_ung_vien", "so_dien_thoai", "email", "dia_chi", "vi_tri_ung_tuyen", "noi_dung", "ghi_chu_cuoi" }, Query = term });
                    }
                }
                if (!string.IsNullOrEmpty(id_ung_vien))
                {
                    must.Add(new TermQuery() { Field = "id_ung_vien", Value = id_ung_vien });
                }
                if (ngay_tao_from > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_tao", GreaterThanOrEqualTo = ngay_tao_from });
                }
                if (ngay_tao_to > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_tao", LessThanOrEqualTo = ngay_tao_to });
                }
                if (ngay_di_lam_from > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_di_lam", GreaterThanOrEqualTo = ngay_di_lam_from });
                }
                if (ngay_di_lam_to > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_di_lam", LessThanOrEqualTo = ngay_di_lam_to });
                }
                if (luong_mong_muon_from > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "luong_mong_muon", GreaterThanOrEqualTo = luong_mong_muon_from });
                }
                if (luong_mong_muon_to > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "luong_mong_muon", LessThanOrEqualTo = luong_mong_muon_to });
                }
                must.Add(new TermsQuery()
                {
                    Field = "thuoc_tinh",
                    Terms = thuoc_tinh.Select(x => (object)x)
                });
                if (lst_id_neq != null && lst_id_neq.Count() > 0)
                {
                    must_not.Add(new IdsQuery() { Values = lst_id_neq.Select(x => (Id)x) });
                }
                List<ISort> sort = new List<ISort>();
                if (sort_order == null)
                {
                    sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                    sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                }
                else
                {
                    foreach (var item in sort_order)
                    {
                        sort.Add(new FieldSort() { Field = item.Key, Order = item.Value ? SortOrder.Descending : SortOrder.Ascending });
                    }
                }
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
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

                var res = client.Search<UngVien>(req);
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
            return new Tuple<List<UngVien>, Dictionary<int, long>>(lst, dic_thong_ke);
        }

        public List<UngVien> Search(string app_id, int page, out long total_recs, out string msg, int page_size = 9999)
        {
            msg = "";
            total_recs = 0;
            List<UngVien> lst = new List<UngVien>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                var res = client.Search<UngVien>(req);
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

        public List<UngVien> GetMany(IEnumerable<string> lst_id)
        {
            List<UngVien> lst = new List<UngVien>();
            if (lst_id != null && lst_id.Count() > 0)
            {
                var re = client.GetMany<UngVien>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        item.Source.id_ung_vien = item.Id;
                        lst.Add(item.Source);
                    }
                }
            }
            return lst;
        }

        public bool IsOwner(string id, string nguoi_tao)
        {
            return IsOwner<UngVien>(_default_index, id, nguoi_tao);
        }

        private bool _HasPermission(string id, string nguoi_tao, int group, Quyen quyen)
        {
            return IsOwner(id, nguoi_tao) || PhanQuyenRepository.Instance.IsExistQuyen(nguoi_tao, group, PhanQuyenObjType.UNG_VIEN, id, nguoi_tao, new List<int>() { (int)quyen });
        }

        public bool CanView(string id, string nguoi_tao, int group)
        {
            return _HasPermission(id, nguoi_tao, group, Quyen.VIEW);
        }

        public bool CanEdit(string id, string nguoi_tao, int group)
        {
            return _HasPermission(id, nguoi_tao, group, Quyen.EDIT);
        }

        public bool CanDelete(string id, string nguoi_tao, int group)
        {
            return _HasPermission(id, nguoi_tao, group, Quyen.DELETE);
        }

        public bool SetThuocTinh(string id, List<int> thuoc_tinh)
        {
            var re = client.Update<UngVien, object>(id, u => u.Doc(new { thuoc_tinh }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }

        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<UngVien>(id, thuoc_tinh);
        }

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<UngVien>(app_id);
        }
    }
}