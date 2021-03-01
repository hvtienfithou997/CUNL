using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESUtil
{
    public class PhanQuyenRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static PhanQuyenRepository instance;

        public PhanQuyenRepository(string modify_index)
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

        public static PhanQuyenRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_phan_quyen";
                    instance = new PhanQuyenRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<PhanQuyen>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<PhanQuyen> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<PhanQuyen>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public bool Index(PhanQuyen data)
        {
            string id = data.AutoId();
            data.id = id;
            return Index(_default_index, data, "", id);
        }

        public bool Update(PhanQuyen data)
        {
            string id = $"{data.id}";
            data.id = string.Empty;

            return Update(_default_index, data, id);
        }

        public PhanQuyen GetById(string id)
        {
            var obj = GetById<PhanQuyen>(_default_index, id);
            if (obj != null)
            {
                obj.id = id;
                return obj;
            }
            return null;
        }

        public bool UpdateNgayHetHan(List<PhanQuyen> lst_id, string nguoi_sua)
        {
            bool result = false;
            List<PhanQuyen> temp = new List<PhanQuyen>();
            try
            {
                int times = 0;
                long now = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                int spl = 500;
                while ((times * spl) <= lst_id.Count && times < lst_id.Count / spl + 1)
                {
                    BulkDescriptor bulk = new BulkDescriptor();
                    temp = lst_id.Skip(times * spl).Take(spl).ToList();
                    times++;
                    if (temp.Count > 0)
                    {
                        foreach (var obj in temp)
                        {
                            bulk.Update<PhanQuyen, object>(u => u.Id(obj.id).Doc(new
                            {
                                ngay_het = obj.ngay_het,
                                nguoi_sua = nguoi_sua,
                                ngay_sua = now
                            }).DocAsUpsert(false));
                        }
                        var res = client.Bulk(bulk);
                    }
                    bulk = null;
                    System.Threading.Thread.Sleep(500);
                }
                return true;
            }
            catch
            {
            }
            finally
            {
                temp = null;
            }
            return result;
        }

        public PhanQuyen GetById(string id, string[] fields = null)
        {
            try
            {
                if (fields != null && fields.Length > 0)
                {
                    IGetResponse<PhanQuyen> res = client.Get<PhanQuyen>(id, g => g.SourceIncludes(fields));
                    if (res != null && res.IsValid)
                    {
                        var obj = res.Source;
                        obj.id = id;
                        return obj;
                    }
                }
                else
                {
                    return client.Get<PhanQuyen>(id).Source;
                }
            }
            catch
            {
            }
            return new PhanQuyen();
        }

        public List<PhanQuyen> GetByUser(string user, string[] fields = null)
        {
            List<PhanQuyen> lst = new List<PhanQuyen>();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            try
            {
                sw.Start();
                ISearchResponse<PhanQuyen> res;
                if (fields != null && fields.Length > 0)
                {
                    res = client.Search<PhanQuyen>(s => s
                        .Source(so => so.Includes(ic => ic.Fields(fields)))
                        .Query(q => q.Term(t => t.Field("type").Value(PhanQuyenType.USERS)) && q.Term(t => t.Field("user.keyword").Value(user))).Size(9999));
                }
                else
                {
                    res = client.Search<PhanQuyen>(s => s.Query(q => q.Term(t => t.Field("type").Value(PhanQuyenType.USERS)) && q.Term(t => t.Field("user.keyword").Value(user))).Size(9999));
                }
                lst = res.Documents.ToList();
            }
            catch
            {
            }
            finally
            {
                if (sw != null)
                {
                    sw.Stop();
                    sw = null;
                }
            }
            return lst;
        }

        public List<PhanQuyen> GetQuyenActive(string user, int group, PhanQuyenObjType obj_type, IEnumerable<int> quyen, string[] fields = null)
        {
            List<PhanQuyen> lst = new List<PhanQuyen>();
            List<QueryContainer> must = new List<QueryContainer>();
            if (string.IsNullOrEmpty(user))
                return lst;
            try
            {
                must.Add(new TermQuery() { Field = "obj_type", Value = obj_type }
                && new LongRangeQuery() { Field = "ngay_het", GreaterThan = XMedia.XUtil.TimeInEpoch(DateTime.Now) } &&
                (
                    (new TermQuery() { Field = "rule", Value = PhanQuyenRule.OBJECT }
                        && new TermsQuery() { Field = "quyen", Terms = quyen.Select(x => (object)x) }
                        && ((new TermQuery() { Field = "type", Value = PhanQuyenType.USERS } && new TermQuery() { Field = "user.keyword", Value = user })
                        || (new TermQuery() { Field = "type", Value = PhanQuyenType.GROUP_USERS } && new TermQuery() { Field = "user.keyword", Value = group }))
                    )
                    ||
                    (new TermQuery() { Field = "rule", Value = PhanQuyenRule.USER }
                        && (new TermQuery() { Field = "type", Value = PhanQuyenType.USERS } && new TermQuery() { Field = "user.keyword", Value = user })
                        || (new TermQuery() { Field = "type", Value = PhanQuyenType.GROUP_USERS } && new TermQuery() { Field = "user.keyword", Value = group })
                    )
                )
                );

                SourceFilter so = new SourceFilter();
                if (fields != null && fields.Length > 0)
                {
                    so.Includes = fields;
                }
                /*
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must});
                req.From = 0;
                req.Size = 9999;
                if (fields != null)
                    req.Source = new SourceFilter() { Includes = fields };

                var res = client.Search<PhanQuyen>(req);

                lst = res.Documents.ToList();
                */
                lst = GetObjectScroll<PhanQuyen>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), so).ToList();
            }
            catch
            {
            }
            finally
            {
            }
            return lst;
        }

        public List<PhanQuyen> Get(string tu_khoa, PhanQuyenRule rule, PhanQuyenType type, string user,
           PhanQuyenObjType obj_type, string obj_id, List<Quyen> quyen,
           long ngay_het_tu, long ngay_het_den, string nguoi_tao, long ngay_tao_tu, long ngay_tao_den,
           string nguoi_sua, long ngay_sua_tu, long ngay_sua_den, int page, int recs, out long total)
        {
            total = 0;
            List<QueryContainer> mustQuery = new List<QueryContainer>();
            List<QueryContainer> mustNotQuery = new List<QueryContainer>();

            try
            {
                #region Term

                if (!String.IsNullOrWhiteSpace(tu_khoa))
                {
                    mustQuery.Add(new QueryStringQuery()
                    {
                        Query = tu_khoa
                    });
                }
                #endregion Term

                #region Rule

                if (rule != PhanQuyenRule.ALL)
                {
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "rule",
                        Value = rule
                    });
                }

                #endregion Rule

                #region Type

                if (type != PhanQuyenType.ALL)
                {
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "type",
                        Value = type
                    });
                }

                #endregion Type

                #region User

                if (!String.IsNullOrWhiteSpace(user) && user.ToLower() != "all")
                {
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "user.keyword",
                        Value = user
                    });
                }

                #endregion User

                #region Type

                if (obj_type != PhanQuyenObjType.ALL)
                {
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "obj_type",
                        Value = obj_type
                    });
                }

                #endregion Type

                #region ObjId

                if (!String.IsNullOrWhiteSpace(obj_id))
                {
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "obj_id.keyword",
                        Value = obj_id
                    });
                }

                #endregion ObjId

                #region Quyền

                if (quyen.Count > 0)
                {
                    mustQuery.Add(new TermsQuery()
                    {
                        Field = "quyen",
                        Terms = quyen.Select(x => (object)x)
                    });
                }

                #endregion Quyền

                #region Ngày hết

                if (ngay_het_tu > 0 && ngay_het_den > 0)
                    mustQuery.Add(new LongRangeQuery()
                    {
                        Field = "ngay_het",
                        GreaterThanOrEqualTo = ngay_het_tu,
                        LessThanOrEqualTo = ngay_het_den
                    });

                #endregion Ngày hết

                #region Người tạo

                if (nguoi_tao.ToLower() != "all")
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "nguoi_tao.keyword",
                        Value = nguoi_tao
                    });

                #endregion Người tạo

                #region Ngày tạo

                if (ngay_tao_tu > 0 && ngay_tao_den > 0)
                    mustQuery.Add(new LongRangeQuery()
                    {
                        Field = "ngay_tao",
                        GreaterThanOrEqualTo = ngay_tao_tu,
                        LessThanOrEqualTo = ngay_tao_den
                    });

                #endregion Ngày tạo

                #region Người sửa

                if (nguoi_sua.ToLower() != "all")
                    mustQuery.Add(new TermQuery()
                    {
                        Field = "nguoi_sua.keyword",
                        Value = nguoi_sua
                    });

                #endregion Người sửa

                #region Ngày sửa

                if (ngay_sua_tu > 0 && ngay_sua_den > 0)
                    mustQuery.Add(new LongRangeQuery()
                    {
                        Field = "ngay_sua",
                        GreaterThanOrEqualTo = ngay_sua_tu,
                        LessThanOrEqualTo = ngay_sua_den
                    });

                #endregion Ngày sửa

                page = page < 1 ? 1 : page;
                QueryContainer main_query = new QueryContainer(new BoolQuery()
                {
                    Must = mustQuery,
                    MustNot = mustNotQuery
                });
                SearchRequest request = new SearchRequest
                {
                    Query = main_query,
                    Size = recs,
                    From = (page - 1) * recs
                };

                var results = client.Search<PhanQuyen>(request);
                total = results.Total;
                return results.Documents.ToList();
            }
            catch
            {
            }
            return new List<PhanQuyen>();
        }

        public bool IsExistQuyen(string user, int group, PhanQuyenObjType obj_type, string obj_id, string owner, List<int> quyen)
        {
            try
            {
                var res = client.Search<PhanQuyen>(s => s
                         .Query(q =>
                             q.Term(t => t.Field("obj_type").Value(obj_type))
                             && q.LongRange(r => r.Field("ngay_het").GreaterThan(XMedia.XUtil.TimeInEpoch(DateTime.Now)))
                             &&
                             ((
                                q.Term(t => t.Field("rule").Value(PhanQuyenRule.OBJECT))
                                && q.Term(t => t.Field("obj_id.keyword").Value(obj_id))
                                && q.Terms(t => t.Field("quyen").Terms(quyen))
                                && ((q.Term(t => t.Field("type").Value(PhanQuyenType.USERS))
                                     && q.Term(t => t.Field("user.keyword").Value(user)))
                                     || (q.Term(t => t.Field("type").Value(PhanQuyenType.GROUP_USERS))
                                     && q.Term(t => t.Field("user.keyword").Value(group)))
                                 ))
                                 ||
                                 (q.Term(t => t.Field("rule").Value(PhanQuyenRule.USER))
                                && q.Term(t => t.Field("obj_id.keyword").Value(owner))
                                && ((q.Term(t => t.Field("type").Value(PhanQuyenType.USERS))
                                     && q.Term(t => t.Field("user.keyword").Value(user)))
                                     || (q.Term(t => t.Field("type").Value(PhanQuyenType.GROUP_USERS))
                                     && q.Term(t => t.Field("user.keyword").Value(group)))
                                 )
                                 )
                             )
                         ).Size(0));
                return res.Total > 0;
            }
            catch
            {
            }
            return false;
        }

        public void RemoveByListId(List<string> lst_id)
        {
            int spl = 100;
            for (int i = 0; i <= lst_id.Count / spl; i++)
            {
                var tmp = lst_id.Skip(i * spl).Take(spl);
                if (tmp.Count() > 0)
                {
                    var bulk = new BulkDescriptor();
                    foreach (var id in tmp)
                    {
                        bulk.Delete<PhanQuyen>(b => b.Id(id));
                    }
                    var re = client.Bulk(bulk);
                }
            }
            RemoveByQuery(lst_id);
        }

        private void RemoveByQuery(List<string> lst_id)
        {
            var re = client.DeleteByQuery<PhanQuyen>(s => s.Query(q => q.Terms(t => t.Field("id.keyword").Terms(lst_id))));
        }

        public bool RemovePhanQuyenByUser(string user, List<string> obj_id)
        {
            return RemovePhanQuyenByUser<PhanQuyen>(user, obj_id);
        }

        public void DeleteAll()
        {
            var re = client.DeleteByQuery<PhanQuyen>(d => d.MatchAll());
        }
    }
}