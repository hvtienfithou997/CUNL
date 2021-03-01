using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUtil
{
    public class LabelRepository : IESRepository
    {
        #region Init
        protected static string _default_index;
        //protected new ElasticClient client;
        private static LabelRepository instance;
        public LabelRepository(string modify_index)
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
        public static LabelRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_label";
                    instance = new LabelRepository(_default_index);
                }
                return instance;
            }
        }
        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<Label>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }
        #endregion
        public IEnumerable<Label> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<Label>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }
        public bool Index(Label data)
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
        public bool Update(Label data)
        {
            string id = $"{data.id_label}";
            data.id_label = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }
        public Label GetById(string id)
        {
            var obj = GetById<Label>(_default_index, id);
            if (obj != null)
            {
                obj.id_label = id;
                return obj;
            }
            return null;
        }
        Label ConvertDoc(IHit<Label> hit)
        {
            Label u = new Label();

            try
            {
                u = hit.Source;
                u.id_label = hit.Id;
            }
            catch
            {
            }
            return u;
        }
        public List<Label> Search(string app_id, string term, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Label> lst = new List<Label>();
            try
            {
                if (ValidateQuery(term))
                {
                    List<QueryContainer> must = new List<QueryContainer>();

                    must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                    List<QueryContainer> must_not = new List<QueryContainer>();
                    must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED }); 
                    if (!string.IsNullOrEmpty(term))
                    {

                        must.Add(new QueryStringQuery() { Fields = new string[] { "ten_label" }, Query = term });

                    }

                    SearchRequest req = new SearchRequest(_default_index);
                    req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot=must_not });
                    req.From = (page - 1) * page_size;
                    req.Size = page_size;
                    var res = client.Search<Label>(req);
                    if (res.IsValid)
                    {
                        total_recs = res.Total;
                        lst = res.Hits.Select(x => ConvertDoc(x)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return lst;
        }
    }
}
