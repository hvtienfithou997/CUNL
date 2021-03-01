using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUtil
{
    public class MenuRepository : IESRepository
    {
        #region Init
        protected static string _default_index;
        //protected new ElasticClient client;
        private static MenuRepository instance;
        public MenuRepository(string modify_index)
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
        public static MenuRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_menu";
                    instance = new MenuRepository(_default_index);
                }
                return instance;
            }
        }
        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(3)).Map<Menu>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }
        #endregion
        public IEnumerable<Menu> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<Menu>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public IEnumerable<Menu> GetAll(string app_id, int page_index, int page_size)
        {
            var re = client.Search<Menu>(s => s.Query(q=> q.Term(t => t.Field("app_id.keyword").Value(app_id))).Size(page_size).From(page_size * (page_index - 1)));
            List<Menu> lc = new List<Menu>();
            foreach (var item in re.Hits)
            {
                lc.Add(ConvertDoc(item));
            }

            return lc;

        }

        public bool Index(Menu data)
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
        public bool Update(Menu data)
        {
            string id = $"{data.id_menu}";
            data.id_menu = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }
        public bool Delete(string id)
        {
            return Delete<Menu>(_default_index, id);
        }
        public Menu GetById(string id)
        {
            var obj = GetById<Menu>(_default_index, id);
            if (obj != null)
            {
                obj.id_menu = id;
                return obj;
            }
            return null;
        }
        Menu ConvertDoc(IHit<Menu> hit)
        {
            Menu u = new Menu();

            try
            {
                u = hit.Source;
                u.id_menu = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<Menu> SearchAll(string app_id, string nguoi_tao, int group, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<Menu> lst = new List<Menu>();
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
                        PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.MENU, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

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
                var res = client.Search<Menu>(req);
                if (res.IsValid)
                {
                    //total_recs = res.Total;
                    total_recs = client.Search<Menu>(s => s.MatchAll().Size(9999).From(0)).Total;
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }

            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }
        public List<Menu> Search(string app_id, string nguoi_tao, int group, string term, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<Menu> lst = new List<Menu>();
            try
            {

                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!string.IsNullOrEmpty(term))
                {
                    if (ValidateQuery(term))
                    {
                        must.Add(new QueryStringQuery() { Fields = new string[] { "ten_menu", "url" }, Query = term });
                    }
                }

                if (!is_admin)
                {
                    if (string.IsNullOrEmpty(nguoi_tao))
                    {
                        nguoi_tao = "__NULL__";
                    }

                    //Check quyền view ở các bản ghi khác
                    List<PhanQuyen> lst_pq =
                        PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.MENU, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

                    must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );

                }
                List<ISort> sort = new List<ISort>();
                
                sort.Add(new FieldSort() { Field = "order", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.TrackTotalHits = true;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Menu>(req);
                if (res.IsValid)
                {
                    total_recs = res.Total;
                    //total_recs = 0
                    lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                }

            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }
        public bool IsOwner(string id, string nguoi_tao)
        {
            return IsOwner<Menu>(_default_index, id, nguoi_tao);
        }
        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<Menu>(app_id);
        }
    }
}
