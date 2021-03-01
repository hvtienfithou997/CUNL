using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESUtil
{
    public class LogSendMailRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static LogSendMailRepository instance;

        public LogSendMailRepository(string modify_index)
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

        public static LogSendMailRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_log_send_mail";
                    instance = new LogSendMailRepository(_default_index);
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

        #endregion Init

        public int IndexMany(List<LogSendMail> data)
        {
            var bulk = new BulkDescriptor();
            foreach (var item in data)
            {
                string id = $"{item.Id}";
                if (client.Search<LogSendMail>(s => s.Size(0).Query(q =>
                        q.Term((t => t.Field("trang_thai").Value((TrangThai.ACTIVE)))) &&
                        q.Ids(i => i.Values(id)))).Total <= 0)
                {
                    item.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    item.ngay_tao = item.ngay_sua;
                    item.Id = id;
                    bulk.Index<LogSendMail>(i => i.Id(id).Document(item));
                }
            }
            var re = client.Bulk(bulk);
            var count = re.ItemsWithErrors.Count();
            return data.Count - count;
        }

        private LogSendMail ConvertDoc(IHit<LogSendMail> hit)
        {
            LogSendMail u = new LogSendMail();

            try
            {
                u = hit.Source;
                u.Id = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<LogSendMail> Search(string app_id, string status, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<LogSendMail> lst = new List<LogSendMail>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                if (!string.IsNullOrEmpty(status))
                {
                    if (ValidateQuery(status))
                    {
                        must.Add(new QueryStringQuery() { Fields = new string[] { "status" }, Query = status });
                    }
                }
                List<ISort> sort = new List<ISort>() { new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending } };
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.TrackTotalHits = true;
                req.Sort = sort;
                var res = client.Search<LogSendMail>(req);
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

        public List<LogSendMail> TimMailMoi(string app_id)
        {
            var lst = new List<LogSendMail>();
            var re = client.Search<LogSendMail>(s => s.Source().Size(1000).Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id))
                    && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                    && q.Term(t => t.Field("status").Value("1"))));
            if (re.Total > 0)
            {
                lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            }

            return lst;
        }

        public List<LogSendMail> TimMailMoiByStt(string stt)
        {
            var lst = new List<LogSendMail>();
            var re = client.Search<LogSendMail>(s => s.Source().Size(1000).Query(q => q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                    && q.Term(t => t.Field("status").Value(stt))));
            if (re.Total > 0)
            {
                lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            }

            return lst;
        }

        public bool Update(LogSendMail data)
        {
            string id = $"{data.Id}";
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }

        public bool UpdateStatus(string id, string stt)
        {
            string msg = "";
            var log = new LogSendMail();
            log.Id = $"{id}";

            var re_u = client.Search<LogSendMail>(s => s.Query(q => q.Term(t => t.Field("id.keyword").Value(id))).Size(1));
            log = re_u.Hits.First().Source;
            log.Id = re_u.Hits.First().Id;
            if (re_u.Total > 0)
            {
                var re = client.Update<LogSendMail, object>(log.Id, u => u.Doc(new { status = stt }));
                return re.Result == Result.Updated || re.Result == Result.Noop;
            }

            return false;
        }
    }
}