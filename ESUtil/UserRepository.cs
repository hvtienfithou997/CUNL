using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESUtil
{
    public class UserRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static UserRepository instance;

        public UserRepository(string modify_index)
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

        public static UserRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_user";
                    instance = new UserRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<User>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<User> GetAll(string term, string[] fields)
        {
            List<QueryContainer> must = new List<QueryContainer>();
            if (!string.IsNullOrEmpty(term))
            {
                must.Add(new QueryStringQuery() { Fields = new string[] { "user_name.keyword", "full_name", "email" }, Query = term });
            }
            List<ISort> sort = new List<ISort>() { new FieldSort() { Field = "user_name.keyword", Order = SortOrder.Ascending } };
            return GetObjectScroll<User>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter() { Includes = fields }, sort);
        }

        public IEnumerable<User> GetByTeam()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<User>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public bool Index(User data, out string msg)
        {
            msg = "";
            int retry = 0; int max_retry = 5;
            bool need_retry = true;
            string id_user = $"{data.app_id}_{data.user_name}";
            if (!Exist<User>(_default_index, id_user))
            {
                while (retry++ < max_retry && need_retry)
                {
                    need_retry = !Index(_default_index, data, "", id_user);
                    if (need_retry)
                        Task.Delay(1000).Wait();
                }
            }
            else
            {
                msg = "Người dùng đã tồn tại";
            }
            return !need_retry;
        }

        public bool Update(User data)
        {
            string id = $"{data.id_user}";
            data.id_user = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }

        public bool UserEditInfo(string id, string full_name, out string msg)
        {
            msg = "";
            var user = new User();
            user.id_user = $"{id}";

            var re_u = client.Search<User>(s => s.Query(q => q.Term(t => t.Field("user_name.keyword").Value(id))).Size(1));
            user = re_u.Hits.First().Source;
            user.id_user = re_u.Hits.First().Id;
            if (re_u.Total > 0)
            {
                var re = client.Update<User, object>(user.id_user, u => u.Doc(new { full_name = full_name }));
                return re.Result == Result.Updated || re.Result == Result.Noop;
            }
            else
            {
                msg = "Không tìm thấy người dùng này";
                return false;
            }
        }

        public bool UpdatePassWord(string id, string password, string old_password, out string msg)
        {
            msg = "";
            var user = new User();
            //user.id_user = $"{id}";
            //var re_old = client.Get<User>(id, g => g.SourceIncludes(new string[] { "password" }));
            var re_u = client.Search<User>(s => s.Query(q => q.Term(t => t.Field("user_name.keyword").Value(id))).Size(1));
            user = re_u.Hits.First().Source;
            user.id_user = re_u.Hits.First().Id;

            if (re_u.Total > 0)
            {
                if (re_u.Documents.First().password == old_password)
                {
                    var re = client.Update<User, object>(user.id_user, u => u.Doc(new { password = password }));
                    return re.Result == Result.Updated || re.Result == Result.Noop;
                }
                else
                {
                    msg = "Mật khẩu cũ không trùng";
                    return false;
                }
            }
            msg = "Không tìm thấy người dùng này";
            return false;
        }

        public bool ResetPassWord(string id, string password, out string msg, bool is_admin = false)
        {
            msg = "";
            var user = new User();
            user.id_user = $"{id}";
            var re_u = client.Search<User>(s => s.Query(q => q.Term(t => t.Field("user_name.keyword").Value(id))).Size(1));
            var re_old = client.Get<User>(id, g => g.SourceIncludes(new string[] { "password" }));
            user = re_u.Hits.First().Source;
            user.id_user = re_u.Hits.First().Id;
            if (re_u.Total > 0)
            {
                var re = client.Update<User, object>(user.id_user, u => u.Doc(new { password = password }));
                return re.Result == Result.Updated || re.Result == Result.Noop;
            }
            msg = "Không tìm thấy người dùng này";
            return false;
        }

        public bool Delete(string id)
        {
            return Delete<User>(_default_index, id);
        }

        public User GetById(string id)
        {
            var user = GetById<User>(_default_index, id);
            if (user != null)
            {
                user.id_user = id;
                return user;
            }
            return null;
        }

        public User GetByUserName(string user_name)
        {
            var re = client.Search<User>(s => s.Query(q => q.Term(t => t.Field("user_name.keyword").Value(user_name))).Size(1));
            if (re.Total > 0)
            {
                User user = re.Hits.First().Source;
                user.id_user = re.Hits.First().Id;
                user.password = "";
                user.old_password = "";
                return re.Documents.First();
            }
            return null;
        }

        public bool IsSysAdmin(string id)
        {
            return client.Count<User>(c => c.Query(q => q.Ids(i => i.Values(new string[] { id })) && q.Term(t => t.Field("trang_thai").Value(QLCUNL.Models.TrangThai.ACTIVE)) && q.Term(t => t.Field("type").Value(1)))).Count > 0;
        }

        private User ConvertDoc(IHit<User> hit)
        {
            User u = new User();

            try
            {
                u = hit.Source;
                u.id_user = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<User> Search(string app_id, string term, string id_team, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<User> lst = new List<User>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });

                if (!string.IsNullOrEmpty(id_team))
                {
                    must.Add(new TermQuery() { Field = "id_team", Value = id_team });
                }
                if (!string.IsNullOrEmpty(term))
                {
                    if (ValidateQuery(term))
                    {
                        must.Add(new QueryStringQuery() { Fields = new string[] { "user_name.keyword", "full_name", "email" }, Query = term });
                    }
                }

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "user_name.keyword", Order = SortOrder.Ascending });

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;

                var res = client.Search<User>(req);
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

        public User Login(string user_name, string password, string ip, string browser)
        {
            if (!string.IsNullOrEmpty(user_name) && !string.IsNullOrEmpty(password))
            {
                var re = client.Search<User>(s => s.Query(q => q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) && q.Term(t => t.Field("user_name.keyword").Value(user_name)) && q.Term(t => t.Field("password.keyword").Value(password))).Size(1));

                if (re.Total > 0)
                {
                    User user = re.Documents.First();
                    user.password = "";
                    user.nguoi_tao = "";
                    user.nguoi_sua = "";
                    user.ngay_sua = 0;
                    user.ngay_tao = 0;
                    client.Update<User, object>(re.Hits.First().Id, u => u.Doc(new { last_login = XMedia.XUtil.TimeInEpoch(DateTime.Now), browser = browser, ip = ip }));
                    return user;
                }
            }
            return null;
        }

        public List<string> GetRoles(string user_name)
        {
            var re = client.Search<User>(s => s.Source(so => so.Includes(ic => ic.Fields(new string[] { "roles" }))).Query(q => q.Term(t => t.Field("user_name.keyword").Value(user_name))).Size(1));
            if (re.IsValid && re.Total > 0)
                return re.Documents.First().roles;
            return new List<string>();
        }

        public List<User> GetAllUserNameByTeam(string app_id, int id_team, string[] fields = null, bool is_admin = false)
        {
            List<User> lst = new List<User>();
            if (fields == null)
                fields = new string[] { "user_name" };

            if (!is_admin)
            {
                var re = client.Search<User>(s => s.Source(so => so.Includes(ic => ic.Fields(fields))).Size(1000).Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id))
                    && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                    && q.Term(t => t.Field("id_team").Value(id_team))));

                if (re.Total > 0)
                    lst = re.Documents.ToList();
            }
            else
            {
                var re = client.Search<User>(s => s.Source(so => so.Includes(ic => ic.Fields(fields))).Size(1000).Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id))
                    && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                    ));

                if (re.Total > 0)
                    lst = re.Documents.ToList();
            }
            return lst;
        }

        public List<User> GetAllUserNameByAppId(string app_id, string[] fields = null, bool is_admin = false)
        {
            List<User> lst = new List<User>();
            if (fields == null)
                fields = new string[] { "user_name" };

            var re = client.Search<User>(s => s.Source(so => so.Includes(ic => ic.Fields(fields))).Size(1000).Query(q =>
                q.Term(t => t.Field("app_id.keyword").Value(app_id))
                && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                ));

            if (re.Total > 0)
                lst = re.Documents.ToList();

            return lst;
        }

        public UserSetting GetDefaultSettingByAppId(string app_id)
        {
            var setting = new UserSetting();
            var re = client.Search<User>(s => s.Size(1).Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
                && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                && q.Term(t => t.Field("roles.keyword").Value(Role.APP_ADMIN.ToString()))
                ));

            if (re.Total > 0)
            {
                try
                {
                    setting = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSetting>(re.Documents.First().default_settings);
                }
                catch (Exception)
                {
                    setting = new UserSetting();
                }
            }
            return setting;
        }

        public void UpdateDefaultSettingByAppId(string app_id, string setting)
        {
            var re = client.UpdateByQuery<User>(u => u.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))).Script($"ctx._source.default_settings='{setting}'"));
        }

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<User>(app_id);
        }
    }
}