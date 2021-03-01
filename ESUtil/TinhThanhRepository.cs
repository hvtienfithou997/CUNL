using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUtil
{
    public class TinhThanhRepository : IESRepository
    {
        #region Init
        protected static string _default_index;
        //protected new ElasticClient client;
        private static TinhThanhRepository instance;
        public TinhThanhRepository(string modify_index)
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
        public static TinhThanhRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_tinh_thanh";
                    instance = new TinhThanhRepository(_default_index);
                }
                return instance;
            }
        }
        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<TinhThanh>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }
        #endregion
        public IEnumerable<TinhThanh> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<TinhThanh>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }
        public bool Index(TinhThanh data)
        {
            return Index(_default_index, data, "", data.id_tinh);
        }
        public bool Update(TinhThanh data)
        {
            string id = $"{data.id_tinh}";
            data.id_tinh = string.Empty;

            return Update(_default_index, data, id);
        }
        public TinhThanh GetById(string id)
        {

            var obj = GetById<TinhThanh>(_default_index, id);
            if (obj != null)
            {
                obj.id_tinh = id;
                return obj;
            }
            return null;
        }

        TinhThanh ConvertDoc(IHit<TinhThanh> hit)
        {
            TinhThanh u = new TinhThanh();

            try
            {
                u = hit.Source;
                u.id_tinh = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<TinhThanh> Search(int page, out long total_recs, out string msg, int page_size = 9999)
        {
            msg = "";
            total_recs = 0;
            List<TinhThanh> lst = new List<TinhThanh>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();

                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                var res = client.Search<TinhThanh>(req);
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
        public bool UpdateSTT(string id, int stt)
        {
            var re = client.Update<TinhThanh, object>(id, u => u.Doc(new { stt }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }
    }
}
