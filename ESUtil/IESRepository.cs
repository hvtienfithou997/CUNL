using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESUtil
{
    public class IESRepository
    {
        private static string reg_null_or_must_remove = "[\"][a-zA-Z0-9_]*[\"]:null[ ]*[,]?|[\"][a-zA-Z0-9_]*[\"]:" + -Int32.MaxValue + "[,]?";
        private static string reg_array_null = "\\[( *null *,? *)*]";
        protected static Uri node = new Uri(XMedia.XUtil.ConfigurationManager.AppSetting["ES:Server"]);
        protected ElasticClient client;

        protected static StickyConnectionPool connectionPool = new StickyConnectionPool(new[] { node });

        protected static string NULL_VALUE = "_null_";
        public string EscapeTerm(string term)
        {
            string[] rmChars = new string[] { "-", "\"", "+", "=", "&&", "||", ">", "<", "!", "(", ")", "{", "}", "[", "]", "^", "~", ":", "\\", "/" };
            if (term.Count(p => p == '"') % 2 == 0)
            {
                rmChars = new string[] { "-", "+", "=", "&&", "||", ">", "<", "!", "(", ")", "{", "}", "[", "]", "^", "~", ":", "\\", "/" };
            }
            foreach (string item in rmChars)
            {
                term = term.Replace(item, " ");
            }

            return term;
        }
        public string RemoveCharNotAllow(string term)
        {
            string[] rmChars = new string[] { "-", "\"", "+", "=", "&&", "||", ">", "<", "!", "(", ")", "{", "}", "[", "]", "^", "~", ":", "\\", "/", ",", "?", "@", "#", "$", "%", "*", "." };

            foreach (string item in rmChars)
            {
                term = term.Replace(item, " ");
            }

            return term;
        }
        public bool ValidateQuery(string term)
        {
            var vali = new ValidateQueryRequest();
            vali.Query = new QueryContainer(new QueryStringQuery() { Query = term });
            var re = client.Indices.ValidateQuery(vali);
            return re.IsValid;
        }
        public async Task<ConcurrentBag<IHit<T>>> BackupAndScroll<T>(string _default_index, QueryContainer query, SourceFilter so, string scroll_timeout = "5m", int scroll_pageize = 2000) where T : class
        {
            if (query == null)
                query = new MatchAllQuery();
            if (so == null)
                so = new SourceFilter() { };
            ConcurrentBag<IHit<T>> bag = new ConcurrentBag<IHit<T>>();
            try
            {
                var searchResponse = await client.SearchAsync<T>(sd => sd.Source(s => so).Index(_default_index).From(0).Take(scroll_pageize).Query(q => query).Scroll(scroll_timeout));

                List<Task> lst_task = new List<Task>();
                while (true)
                {
                    if (!searchResponse.IsValid || string.IsNullOrEmpty(searchResponse.ScrollId))
                        break;

                    if (!searchResponse.Documents.Any())
                        break;

                    var tmp = searchResponse.Hits;
                    foreach (var item in tmp)
                    {
                        bag.Add(item);
                    }
                    searchResponse = await client.ScrollAsync<T>(scroll_timeout, searchResponse.ScrollId);
                }

                await client.ClearScrollAsync(new ClearScrollRequest(searchResponse.ScrollId));
                await Task.WhenAll(lst_task);
            }
            catch (Exception)
            {

            }
            finally
            {

            }
            return bag;
        }
        protected ConcurrentBag<T> GetObjectScroll<T>(string _default_index, QueryContainer query, SourceFilter so, List<ISort> lSort = null, string routing = "") where T : class
        {
            try
            {
                var numberOfShards = 5;
                var seenSlices = new ConcurrentBag<int>();
                ConcurrentBag<T> bag = new ConcurrentBag<T>();

                ScrollAllRequest req = new ScrollAllRequest("5m", numberOfShards);
                SearchRequest sReq = new SearchRequest(_default_index);
                if (!string.IsNullOrEmpty(routing))
                    sReq.Routing = routing;
                sReq.Query = query;
                if (lSort != null)
                    sReq.Sort = lSort;
                if (so != null)
                    sReq.Source = so;
                req.Search = sReq;
                req.MaxDegreeOfParallelism = numberOfShards;// / 2;
                sReq.Size = 10000;

                var scrollObserver = client.ScrollAll<T>(req).Wait(TimeSpan.FromMinutes(20), r =>
                {
                    seenSlices.Add(r.Slice);
                    if (lSort == null)
                        System.Threading.Tasks.Parallel.ForEach(r.SearchResponse.Documents, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = 10 }, item =>
                        {
                            bag.Add(item);
                        });
                    else
                        foreach (var item in r.SearchResponse.Documents)
                        {
                            bag.Add(item);
                        }

                });
                return bag;

            }
            catch
            {
            }
            return null;
        }
        protected ConcurrentDictionary<string, T> GetDicObjectScroll<T>(string _default_index, QueryContainer query, SourceFilter so) where T : class
        {
            try
            {
                var numberOfShards = 5;
                var seenSlices = new ConcurrentBag<int>();
                ConcurrentDictionary<string, T> bag = new ConcurrentDictionary<string, T>();

                ScrollAllRequest req = new ScrollAllRequest("1m", numberOfShards);
                SearchRequest sReq = new SearchRequest(_default_index);
                sReq.Query = query;
                if (so != null)
                    sReq.Source = so;
                req.Search = sReq;
                req.MaxDegreeOfParallelism = numberOfShards / 2;


                var scrollObserver = client.ScrollAll<T>(req).Wait(TimeSpan.FromMinutes(5), r =>
                {
                    seenSlices.Add(r.Slice);
                    foreach (var item in r.SearchResponse.Hits)
                    {
                        bag.TryAdd(item.Id, item.Source);
                    }

                });
                return bag;

            }
            catch
            {
            }
            return null;
        }
        protected bool Index<T>(string _default_index, T data, string route, string id = "") where T : class
        {
            IndexRequest<object> req = new IndexRequest<object>(_default_index, typeof(T));
            if (!string.IsNullOrEmpty(route))
                req.Routing = route;

            req.Document = data;
            IndexResponse re = null;

            if (string.IsNullOrEmpty(id))
            {
                id = Nanoid.Nanoid.Generate(size: 16);
            }

            re = client.Index<T>(data, i => i.Id(id));

            return re.Result == Result.Created || re.Result == Result.Updated;
        }
        protected string Index<T>(string _default_index, T data) where T : class
        {
            IndexRequest<object> req = new IndexRequest<object>(_default_index, typeof(T));
            string json_doc = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Regex regex = new Regex(reg_null_or_must_remove);
            json_doc = regex.Replace(json_doc, string.Empty);
            regex = new Regex(reg_array_null);
            json_doc = regex.Replace(json_doc, "[]");
            var ob_up = Newtonsoft.Json.JsonConvert.DeserializeObject(json_doc);
            req.Document = ob_up;
            IndexResponse re = null;
            string id = Nanoid.Nanoid.Generate(size: 16);
            re = client.Index<T>(data, i => i.Id(id));

            return re.Id;
        }
        protected bool Update<T>(string _default_index, T data, string id) where T : class
        {
            ConnectionSettings settings_update = new ConnectionSettings(connectionPool, sourceSerializer: Nest.JsonNetSerializer.JsonNetSerializer.Default).DefaultIndex(_default_index).DisableDirectStreaming(true);
            var client = new ElasticClient(settings_update);

            string json_doc = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Regex regex = new Regex(reg_null_or_must_remove);
            json_doc = regex.Replace(json_doc, string.Empty);
            regex = new Regex(reg_array_null);
            json_doc = regex.Replace(json_doc, "[]");
            json_doc = json_doc.Replace("\"ngay_tao\":0,", "").Replace("\"auto_id\":0,", "");
            var ob_up = Newtonsoft.Json.JsonConvert.DeserializeObject(json_doc);
            var re_up = client.Update<T, object>(id, u => u.Doc(ob_up));

            if (re_up.Result == Result.Error)
            {
                return Index<T>(_default_index, data, "", id);
            }
            return re_up.Result == Result.Updated;
        }
        protected bool Delete<T>(string _default_index, string id) where T : class
        {
            var re_up = client.Update<T, object>(id, u => u.Doc(new { trang_thai = QLCUNL.Models.TrangThai.DELETED }));

            return re_up.Result == Result.Updated || re_up.Result == Result.Noop;
        }
        protected T GetById<T>(string _default_index, string id) where T : class
        {
            try
            {
                var g = new GetRequest(_default_index, id);
                var re = client.Get<T>(g);
                if (re.Found)
                    return re.Source;
            }
            catch (Exception)
            {

            }
            return null;
        }
        protected bool Refresh(string _default_index)
        {
            var re = client.Indices.Refresh(_default_index, r => r.RequestConfiguration(c => c.RequestTimeout(TimeSpan.FromSeconds(5))));
            return re.IsValid;
        }
        protected bool Exist<T>(string _default_index, string id) where T : class
        {
            IDocumentExistsRequest req = new DocumentExistsRequest<T>(_default_index, id);
            var re = client.DocumentExists(req);
            return re.Exists;
        }
        protected bool IsOwner<T>(string _default_index, string id, string nguoi_tao) where T : class
        {
            try
            {
                var re = client.Get<T>(id, g => g.Index(_default_index).SourceIncludes(new string[] { "nguoi_tao" }));

                Type t = re.Source.GetType();

                PropertyInfo prop = t.GetProperty("nguoi_tao");

                if (prop != null)
                {
                    string owner = Convert.ToString(prop.GetValue(re.Source));
                    return owner == nguoi_tao;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
        protected void ResetAppId(string _default_index, string app_id)
        {
            var req = new UpdateByQueryRequest(_default_index);
            //req.Query = new QueryContainer(new MatchAllQuery());
            //req.Script = new InlineScript($"ctx._source.app_id='{app_id}'");

            var re = client.UpdateByQuery(req);
        }
        public IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<string> lst_id) where T : class
        {
            lst_id = lst_id.Where(x => !string.IsNullOrEmpty(x));
            if (lst_id.Count() > 0)
            {
                var re = client.GetMany<T>(lst_id);

                return re;
            }
            return null;
        }
        public bool UpdateThuocTinh<T>(string id, List<int> thuoc_tinh) where T : class
        {
            var re = client.Update<T, object>(id, u => u.Doc(new { thuoc_tinh = thuoc_tinh }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }
        public bool RemoveDataByAppId<T>(string app_id) where T : class
        {
            var re = client.DeleteByQuery<T>(d => d.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))));
            return re.IsValid;
        }
        public bool RemovePhanQuyenByUser<T>(string user, List<string> obj_id) where T : class
        {
            var re = client.DeleteByQuery<T>(d => d.Query(q => q.Term(t => t.Field("user.keyword").Value(user)) && q.Terms(t => t.Field("obj_id.keyword").Terms(obj_id))));

            return re.IsValid;
        }
    }
}
