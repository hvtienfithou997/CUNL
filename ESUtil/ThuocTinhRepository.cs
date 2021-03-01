using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESUtil
{
    public class ThuocTinhRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static ThuocTinhRepository instance;

        public ThuocTinhRepository(string modify_index)
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

        public static ThuocTinhRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_thuoc_tinh";
                    instance = new ThuocTinhRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<ThuocTinh>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<ThuocTinh> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<ThuocTinh>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public bool Index(ThuocTinh data)
        {
            var gia_tri_random = Nanoid.Nanoid.Generate(alphabet: "1234567890", size: 8);

            var max_gia_tri = Convert.ToInt32(gia_tri_random);
            bool need_retry = true;
            if (!IsGiaTriThuocTinhExist(data.app_id, data.loai, data.type, max_gia_tri))
            {
                data.gia_tri = max_gia_tri;
                int retry = 0; int max_retry = 5;

                while (retry++ < max_retry && need_retry)
                {
                    need_retry = !Index(_default_index, data, "");
                    if (need_retry)
                        Task.Delay(1000).Wait();
                }
            }
            return !need_retry;
        }

        public bool Update(ThuocTinh data, out string msg)
        {
            msg = "";
            string id = $"{data.id}";

            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);

            var re_old = client.Get<ThuocTinh>(data.id);
            if (re_old.Found)
            {
                data.gia_tri = re_old.Source.gia_tri;

                var re = client.Search<ThuocTinh>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(data.app_id)) && q.Term(t => t.Field("loai").Value(data.loai))
                    && q.Term(t => t.Field("type").Value(data.type)) && q.Term(t => t.Field("gia_tri").Value(data.gia_tri))
                ).Size(100));
                if (re.Hits.Any(x => x.Id != id))
                {
                    msg = "Giá trị thuộc tính cho đối tượng này đã tồn tại";
                    return false;
                }
                return Update(_default_index, data, id);
            }
            else
            {
                msg = "Không tìm thấy thuộc tính để cập nhật";
                return false;
            }
        }

        public bool UpdateTenNhom(ThuocTinh data)
        {
            string id = $"{data.id}";
            data.id = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);

            var re = client.Update<ThuocTinh, object>(id, u => u.Doc(new { data.nguoi_sua, data.ten, data.loai, data.type, data.nhom }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }

        public bool Delete(string id)
        {
            return Delete<ThuocTinh>(_default_index, id);
        }

        public ThuocTinh GetById(string id)
        {
            var obj = GetById<ThuocTinh>(_default_index, id);
            if (obj != null)
            {
                obj.id = id;
                return obj;
            }
            return null;
        }

        private ThuocTinh ConvertDoc(IHit<ThuocTinh> hit)
        {
            ThuocTinh u = new ThuocTinh();

            try
            {
                u = hit.Source;
                u.id = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<ThuocTinh> Search(string app_id, string term, int loai, int type, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<ThuocTinh> lst = new List<ThuocTinh>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                if (!string.IsNullOrEmpty(term))
                {
                    must.Add(new QueryStringQuery() { Fields = new string[] { "ten" }, Query = term });
                }
                if (loai > -1)
                {
                    must.Add(new TermQuery()
                    {
                        Field = "loai",
                        Value = loai
                    });
                }
                if (type > -1)
                {
                    must.Add(new TermQuery()
                    {
                        Field = "type",
                        Value = type
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
                req.TrackTotalHits = true;
                var res = client.Search<ThuocTinh>(req);
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

        public List<ThuocTinh> Search(string app_id, string nguoi_tao, string term, int loai, int type, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<ThuocTinh> lst = new List<ThuocTinh>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!is_admin)
                    must.Add(new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao });
                if (!string.IsNullOrEmpty(term))
                {
                    must.Add(new QueryStringQuery() { Fields = new string[] { "ten" }, Query = term });
                }
                if (loai > -1)
                {
                    must.Add(new TermQuery()
                    {
                        Field = "loai",
                        Value = loai
                    });
                }
                if (type > -1)
                {
                    must.Add(new TermQuery()
                    {
                        Field = "type",
                        Value = type
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
                req.TrackTotalHits = true;
                var res = client.Search<ThuocTinh>(req);
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

        public List<ThuocTinh> GetAllByLoaiThuocTinh(string app_id, int loai, int type)
        {
            if (type == -1)
            {
                var re = client.Search<ThuocTinh>(s => s.Source(so => so.Includes(ex => ex.Fields(new string[] { "id", "loai", "gia_tri", "ten", "nhom", "type" }))).Size(9999).Query(q =>
                   q.Bool(b => b.Filter(f => f.Term(t => t.Field("app_id.keyword").Value(app_id)) && f.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) && f.Term(t => t.Field("loai").Value(loai))))));
                return re.Documents.ToList();
            }
            else
            {
                var re = client.Search<ThuocTinh>(s => s.Source(so => so.Includes(ex => ex.Fields(new string[] { "id", "loai", "gia_tri", "ten", "nhom", "type" }))).Size(9999).Query(q =>
                q.Bool(b => b.Filter(f => f.Term(t => t.Field("app_id.keyword").Value(app_id)) && f.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                && f.Term(t => t.Field("loai").Value(loai)) && f.Term(t => t.Field("type").Value(type))))));
                return re.Documents.ToList();
            }
        }

        public List<ThuocTinh> GetAllByLoaiThuocTinh(string app_id, string nguoi_tao, int loai)
        {
            var re = client.Search<ThuocTinh>(s => s.Source(so => so.Includes(ex => ex.Fields(new string[] { "id", "loai", "gia_tri", "ten", "nhom", "type" }))).Size(9999).Query(q =>
            q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
             q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
                q.Bool(b => b.Filter(f => (f.Term(t => t.Field("type").Value(ThuocTinhType.SHARED)) && f.Term(t => t.Field("loai").Value(loai))) ||
                (f.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao)) && f.Term(t => t.Field("loai").Value(loai)) && f.Term(t => t.Field("type").Value(ThuocTinhType.PRIVATE)))
                ))));
            return re.Documents.ToList();
        }

        public bool IsGiaTriThuocTinhExist(string app_id, LoaiThuocTinh loai, ThuocTinhType type, int gia_tri)
        {
            var re = client.Search<ThuocTinh>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
            q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                 && q.Term(t => t.Field("gia_tri").Value(gia_tri))
            ).Size(0));
            if (re.IsValid)
                return re.Total > 0;
            return true;
        }

        public bool IsTenThuocTinhExist(string app_id, LoaiThuocTinh loai, ThuocTinhType type, string ten)
        {
            var re = client.Search<ThuocTinh>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
            q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
            q.Term(t => t.Field("loai").Value(loai))
                && q.Term(t => t.Field("type").Value(type)) && q.Term(t => t.Field("ten.keyword").Value(ten))
            ).Size(0));
            if (re.IsValid)
                return re.Total > 0;
            return true;
        }

        public List<ThuocTinh> GeSharedtByLoaiGiaTri(string app_id, IEnumerable<int> thuoc_tinh, LoaiThuocTinh loai)
        {
            if (thuoc_tinh.Count() == 0)
                return new List<ThuocTinh>();
            var re = client.Search<ThuocTinh>(s => s.Source(so => so.Includes(ic => ic.Fields(new string[] { "loai", "gia_tri", "nhom", "type", "ten" }))).Query(q =>
            q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
            q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
                q.Term(t => t.Field("loai").Value(loai)) && q.Terms(t => t.Field("gia_tri").Terms(thuoc_tinh)) && q.Term(t => t.Field("type").Value(ThuocTinhType.SHARED))

                ).Size(100));

            return re.Documents.ToList();
        }

        public List<ThuocTinh> GetPrivateByLoaiGiaTri(string app_id, string nguoi_tao, IEnumerable<int> thuoc_tinh, LoaiThuocTinh loai, bool is_admin)
        {
            if (thuoc_tinh.Count() == 0)
                return new List<ThuocTinh>();
            if (is_admin)
            {
                var re = client.Search<ThuocTinh>(s => s.Source(so => so.Includes(ic => ic.Fields(new string[] { "loai", "gia_tri", "nhom", "type", "ten" }))).Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
                    q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
                    q.Term(t => t.Field("loai").Value(loai)) &&
                     q.Terms(t => t.Field("gia_tri").Terms(thuoc_tinh)) &&
                      q.Term(t => t.Field("type").Value(ThuocTinhType.PRIVATE))

                    ).Size(100));

                return re.Documents.ToList();
            }
            else
            {
                var re = client.Search<ThuocTinh>(s =>
                    s.Source(
                            so => so.Includes(ic => ic.Fields(new string[] {"loai", "gia_tri", "nhom", "type", "ten"})))
                        .Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
                                    q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
                                    q.Term(t => t.Field("loai").Value(loai)) &&
                                    q.Terms(t => t.Field("gia_tri").Terms(thuoc_tinh)) &&
                                    q.Term(t => t.Field("type").Value(ThuocTinhType.PRIVATE)) &&
                                    q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))).Size(100));

                return re.Documents.ToList();
            }
        }

        public List<ThuocTinh> GetMany(IEnumerable<string> lst_id)
        {
            List<ThuocTinh> lst = new List<ThuocTinh>();
            if (lst_id.Count() > 0)
            {
                var re = client.GetMany<ThuocTinh>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        item.Source.id = item.Id;
                        lst.Add(item.Source);
                    }
                }
            }
            return lst;
        }

        public List<ThuocTinh> GetManyByGiaTri(string app_id, IEnumerable<int> lst_gia_tri, LoaiThuocTinh loai, ThuocTinhType type)
        {
            List<ThuocTinh> lst = new List<ThuocTinh>();

            if (lst_gia_tri.Count() > 0)
            {
                var re = client.Search<ThuocTinh>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) &&
                q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
                q.Term(t => t.Field("loai").Value(loai)) && q.Term(t => t.Field("type").Value(type)) && q.Terms(t => t.Field("gia_tri").Terms(lst_gia_tri))));
                return re.Hits.Select(x => ConvertDoc(x)).ToList();
            }
            return lst;
        }

        public List<ThuocTinh> GetManyByGiaTri(string app_id, IEnumerable<int> lst_gia_tri, LoaiThuocTinh loai, int type)
        {
            List<ThuocTinh> lst = new List<ThuocTinh>();

            if (lst_gia_tri.Count() > 0)
            {
                if (type > 0)
                {
                    var re = client.Search<ThuocTinh>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
                        && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                        && q.Term(t => t.Field("loai").Value(loai))
                        && q.Term(t => t.Field("type").Value(type))
                        && q.Terms(t => t.Field("gia_tri").Terms(lst_gia_tri))));
                    return re.Hits.Select(x => ConvertDoc(x)).ToList();
                }
                else
                {
                    var re = client.Search<ThuocTinh>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
                        && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                        && q.Term(t => t.Field("loai").Value(loai))
                        && q.Terms(t => t.Field("gia_tri").Terms(lst_gia_tri))));
                    return re.Hits.Select(x => ConvertDoc(x)).ToList();
                }
            }
            return lst;
        }

        public bool IsOwner(string id, string nguoi_tao)
        {
            return IsOwner<ThuocTinh>(_default_index, id, nguoi_tao);
        }

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<ThuocTinh>(app_id);
        }

        public void UpdateTest(string id, int gia_tri)
        {
            var re = client.Update<ThuocTinh, object>(id, u => u.Doc(new { gia_tri = gia_tri }));
        }
    }
}