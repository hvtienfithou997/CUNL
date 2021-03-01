using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESUtil
{
    public class GroupUserRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static GroupUserRepository instance;

        public GroupUserRepository(string modify_index)
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

        public static GroupUserRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_group_user";
                    instance = new GroupUserRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<GroupUser>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<GroupUser> GetAll(string app_id, string[] fields)
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
            must.Add(new MatchAllQuery());
            SearchRequest req = new SearchRequest(_default_index);
            req.Query = new QueryContainer(new BoolQuery() { Must = must });
            req.From = 0;
            req.Size = 9999;
            req.TrackTotalHits = true;
            var res = client.Search<GroupUser>(req);
            if (res.IsValid)
            {
                return res.Hits.Select(x => ConvertDoc(x)).ToList();
            }
            return new List<GroupUser>();
        }

        public bool Index(GroupUser data, out string msg)
        {
            msg = "";
            var re = client.Search<GroupUser>(s => s.Query(q => q.MatchAll()).Size(0)
            .Aggregations(ag => ag.Max("max_gia_tri", a => a.Field("id_team"))));

            var max_gia_tri = Convert.ToInt32(re.Aggregations.Max("max_gia_tri").Value) + 1;
            data.id_team = max_gia_tri;
            string id_team = data.id_team.ToString();
            if (!Exist<GroupUser>(_default_index, id_team))
            {
                return Index(_default_index, data, "", id_team);
            }
            else
            {
                msg = "Nhóm đã tồn tại";
            }
            return false;
        }

        public bool Update(GroupUser data)
        {
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, data.id);
        }

        public bool Delete(string id)
        {
            return Delete<GroupUser>(_default_index, id);
        }

        public GroupUser GetById(string id)
        {
            var group_user = GetById<GroupUser>(_default_index, id);
            if (group_user != null)
            {
                group_user.id = id;
                return group_user;
            }
            return null;
        }

        public List<GroupUser> Search(string app_id, string term, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<GroupUser> lst = new List<GroupUser>();
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
                        must.Add(new QueryStringQuery() { Fields = new string[] { "team_name" }, Query = term });
                    }
                }

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.TrackTotalHits = true;
                var res = client.Search<GroupUser>(req);
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

        private GroupUser ConvertDoc(IHit<GroupUser> hit)
        {
            GroupUser g = hit.Source;
            g.id = hit.Id;

            return g;
        }

        public List<GroupUser> Search(string app_id, int page, out long total_recs, out string msg, int page_size = 9999)
        {
            msg = "";
            total_recs = 0;
            List<GroupUser> lst = new List<GroupUser>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });

                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "team_name.keyword", Order = SortOrder.Ascending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                var res = client.Search<GroupUser>(req);
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

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<GroupUser>(app_id);
        }
    }
}