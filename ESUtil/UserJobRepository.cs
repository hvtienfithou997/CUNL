using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUtil
{
    public class UserJobRepository : IESRepository
    {
        #region Init
        protected static string _default_index;
        //protected new ElasticClient client;
        private static UserJobRepository instance;
        public UserJobRepository(string modify_index)
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
        public static UserJobRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_user_job";
                    instance = new UserJobRepository(_default_index);
                }
                return instance;
            }
        }
        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(3)).Map<UserJob>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }
        #endregion
        public IEnumerable<UserJob> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<UserJob>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }
        public bool Index(UserJob data)
        {
            string id = $"{data.id_user}_{data.id_job}";
            return Index(_default_index, data, "", id);
        }
        public int IndexMany(List<UserJob> data)
        {
            var bulk = new BulkDescriptor();
            foreach (var item in data)
            {
                string id = $"{item.id_user}_{item.id_job}";
                if (!client.DocumentExists<UserJob>(id).Exists)
                {
                    item.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    item.ngay_tao = item.ngay_sua;
                    bulk.Index<UserJob>(i => i.Id(id).Document(item));
                }
            }
            var re = client.Bulk(bulk);
            var count = re.ItemsWithErrors.Count();
            return data.Count - count;
        }
        public bool Update(UserJob data)
        {
            string id = $"{data.id_user_job}";
            data.id_user_job = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }
        public bool UpdateThuocTinh(UserJob data)
        {
            string id = $"{data.id_user_job}";
            data.id_user_job = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            var re = client.Update<UserJob, object>(id, u => u.Doc(new { ngay_sua = data.ngay_sua, thuoc_tinh = data.thuoc_tinh }));
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }
        public bool Delete(string id)
        {
            return Delete<UserJob>(_default_index, id);
        }
        public UserJob GetById(string id)
        {
            var obj = GetById<UserJob>(_default_index, id);
            if (obj != null)
            {
                obj.id_user_job = id;
                return obj;
            }
            return null;
        }
        UserJob ConvertDoc(IHit<UserJob> hit)
        {
            UserJob u = new UserJob();

            try
            {
                u = hit.Source;
                u.id_user_job = hit.Id;
            }
            catch
            {
            }
            return u;
        }
        public List<UserJob> Search(string app_id, string nguoi_tao, int group, IEnumerable<string> id_job, string id_ung_vien, long ngay_nhan_job_from,
            long ngay_nhan_job_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, int page, out long total_recs,
            out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<UserJob> lst = new List<UserJob>();
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
                    List<PhanQuyen> lst_pq =
                       PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.USER_JOB, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

                    must.Add(
                        /*new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao } || */new TermQuery() { Field = "id_user.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );

                }
                if (id_job != null && id_job.Count() > 0)
                {
                    must.Add(new TermsQuery() { Field = "id_job.keyword", Terms = id_job.Select(x => (object)x) });
                }
                if (!string.IsNullOrEmpty(id_ung_vien))
                {
                    must.Add(new TermQuery() { Field = "id_ung_vien", Value = id_ung_vien });
                }
                if (ngay_nhan_job_from > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_nhan_job", GreaterThanOrEqualTo = ngay_nhan_job_from });
                }
                if (ngay_nhan_job_to > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_nhan_job", LessThanOrEqualTo = ngay_nhan_job_to });
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
                        Values = lst_id.Select(x => (Id)x)
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
                var res = client.Search<UserJob>(req);
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
        public bool IsOwner(string id, string nguoi_tao)
        {
            return IsOwner<UserJob>(_default_index, id, nguoi_tao);
        }
        public List<UserJob> GetByIdJob(string app_id, string id_job)
        {
            var re = client.Search<UserJob>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && !q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))
            && q.Term(t => t.Field("id_job.keyword").Value(id_job))
            ).Size(9999));

            return re.Hits.Select(x => ConvertDoc(x)).ToList();

        }
        public List<UserJob> GetUserJobByIdJob(string app_id, string id_job, string user)
        {
            var re = client.Search<UserJob>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && !q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))
            && q.Term(t => t.Field("id_job.keyword").Value(id_job))
            ).Size(9999));

            return re.Hits.Select(x => ConvertDoc(x)).Where(o => o.id_user == user && o.id_job == id_job).ToList();

        }
        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<UserJob>(id, thuoc_tinh);
        }
        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<UserJob>(app_id);
        }
        public async Task<List<string>> GetListIdJobByUser(string app_id, IEnumerable<string> ids_user)
        {


            QueryContainer qc = new QueryContainer(new TermQuery() { Field = "app_id.keyword", Value = app_id }
            && new TermQuery() { Field = "trang_thai", Value = TrangThai.ACTIVE }
            && new TermsQuery() { Field = "id_user.keyword", Terms = ids_user }
            );
            SourceFilter so = new SourceFilter();
            so.Includes = new string[] { "id_job" };

            var all = await BackupAndScroll<UserJob>(_default_index, qc, so);
            return all.Select(x => x.Source.id_job).Distinct().ToList();
        }
        public Dictionary<string, long> ThongKeUserJob(string app_id, IEnumerable<string> ids_job)
        {
            Dictionary<string, long> dic = new Dictionary<string, long>();
            if (ids_job.Count() > 0)
            {
                QueryContainer qc = new QueryContainer(new TermQuery() { Field = "app_id.keyword", Value = app_id }
                && new TermQuery() { Field = "trang_thai", Value = TrangThai.ACTIVE }
                && new TermsQuery() { Field = "id_job.keyword", Terms = ids_job }
                );
                var re = client.Search<UserJob>(s => s.Size(0).Query(q => qc).Aggregations(
                    ag => ag.Terms("group_by_id_job", t => t.Field("id_job.keyword").Size(ids_job.Count()))
                    ));

                var re_ag = re.Aggregations.Terms("group_by_id_job");


                foreach (var item in re_ag.Buckets)
                {
                    dic.Add(item.Key, item.DocCount.GetValueOrDefault());
                }
            }
            return dic;
        }

        public bool DeleteByIdUserJob(IEnumerable<string> id_user_job)
        {
            foreach(var lst_id in id_user_job)
            {
                return Delete<UserJob>(_default_index, lst_id);
            }
            return true;            
        }
    }
}
