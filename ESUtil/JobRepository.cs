using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job = QLCUNL.Models.Job;

namespace ESUtil
{
    public class JobRepository : IESRepository
    {
        #region Init

        private int max_id_job = 5;
        protected static string _default_index;

        //protected new ElasticClient client;
        private static JobRepository instance;

        public JobRepository(string modify_index)
        {
            _default_index = !string.IsNullOrEmpty(modify_index) ? modify_index : _default_index;
            ConnectionSettings settings = new ConnectionSettings(connectionPool).DefaultIndex(_default_index).DisableDirectStreaming(true);

            settings.MaximumRetries(10);
            client = new ElasticClient(settings);
            var ping = client.Ping(p => p.Pretty(true));
            if (ping.ServerError != null && ping.ServerError.Error != null)
            {
                throw new Exception("START ES FIRST");
            }
        }

        public static JobRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_job";
                    instance = new JobRepository(_default_index);
                }
                return instance;
            }
        }

        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(3)).Map<Job>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }

        #endregion Init

        public IEnumerable<Job> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<Job>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        private bool IsAutoIdExist(string auto_id, string app_id)
        {
            var re = client.Count<Job>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("auto_id").Value(auto_id))));
            return re.Count > 0;
        }

        public bool IsIdAutoExist(string id_auto, string app_id)
        {
            var re = client.Count<Job>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("id_auto.keyword").Value(id_auto))));
            return re.Count > 0;
        }

        public string Index(Job data, out string auto_id)
        {
            auto_id = "";
            int retry = 0;

            if (string.IsNullOrEmpty(data.id_auto))
            {
                string _auto_id = Nanoid.Nanoid.Generate("0123456789", max_id_job);

                while (IsAutoIdExist(_auto_id, data.app_id) && retry++ < 10)
                {
                    Task.Delay(TimeSpan.FromSeconds(1));
                    _auto_id = Nanoid.Nanoid.Generate("0123456789", max_id_job);
                }
                if (retry >= 10)
                {
                    return "";
                }
                data.id_auto = _auto_id;
            }
            else
            {
                while (IsIdAutoExist(data.id_auto, data.app_id) && retry++ < 10)
                {
                    Task.Delay(TimeSpan.FromSeconds(1));
                    data.id_auto = Nanoid.Nanoid.Generate("0123456789", max_id_job);
                }
                if (retry >= 10)
                {
                    return "";
                }
            }
            auto_id = data.id_auto;
            return Index(_default_index, data);
        }

        public bool Update(Job data)
        {
            string id = $"{data.id_job}";
            data.id_job = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            data.auto_id = -Int32.MaxValue;
            data.ngay_tao = -Int32.MaxValue;
            data.id_auto = null;
            return Update(_default_index, data, id);
        }

        public bool Delete(string id)
        {
            return Delete<Job>(_default_index, id);
        }

        public bool DelUp(string id)
        {
            var re = client.Update<Job, object>(id, u => u.Doc(new { id_auto = string.Empty, trang_thai = TrangThai.DELETED }));
            //var del = Delete<Job>(_default_index, id);
            return re.Result == Result.Updated || re.Result == Result.Noop;
        }

        public Job GetById(string id)
        {
            var obj = GetById<Job>(_default_index, id);
            if (obj != null)
            {
                obj.id_job = id;
                return obj;
            }
            return null;
        }

        private Job ConvertDoc(IHit<Job> hit)
        {
            Job u = new Job();

            try
            {
                u = hit.Source;
                u.id_job = hit.Id;
            }
            catch
            {
            }
            return u;
        }

        public List<Job> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from,
            long ngay_di_lam_to, double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
            long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null)
        {
            msg = "";
            total_recs = 0;
            List<Job> lst = new List<Job>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> should = new List<QueryContainer>();

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
                        PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.JOB, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });
                    //Lấy tất cả người dùng cùng team để lấy JOB của những người dùng này
                    var lst_user_name_in_team = UserRepository.Instance.GetAllUserNameByTeam(app_id, group).Select(x => x.user_name);

                    if (lst_should_id.Count() > 0)
                    {
                        must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "nguoi_tao.keyword", Terms = lst_user_name_in_team }
                        || new TermQuery() { Field = "owner.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "owner.keyword", Terms = lst_user_name_in_team }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                        || new IdsQuery() { Values = lst_should_id.Select(x => (Id)x) }
                        );
                    }
                    else
                    {
                        must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "nguoi_tao.keyword", Terms = lst_user_name_in_team }
                        || new TermQuery() { Field = "owner.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "owner.keyword", Terms = lst_user_name_in_team }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                    }
                }
                if (!string.IsNullOrEmpty(id_cong_ty))
                {
                    must.Add(new TermQuery() { Field = "cong_ty.id_cong_ty.keyword", Value = id_cong_ty });
                }
                if (!string.IsNullOrEmpty(term))
                {
                    must.Add(new QueryStringQuery() { Fields = new string[] { "id_auto", "chuc_danh", "nguoi_lien_he", "noi_dung", "ghi_chu", "cong_ty.ten_cong_ty" }, Query = term });
                }
                if (!string.IsNullOrEmpty(id_ung_vien))
                {
                    must.Add(new TermQuery() { Field = "id_ung_vien", Value = id_ung_vien });
                }
                if (ngay_tao_from > 0)
                {
                    must.Add(new LongRangeQuery() { Field = "ngay_tao", GreaterThanOrEqualTo = ngay_tao_from });
                }
                if (ngay_tao_to > 0)
                {
                    DateTime dt = Convert.ToDateTime(XMedia.XUtil.EpochToTimeString(ngay_tao_to));
                    var date2 = dt.AddHours(23).AddMinutes(59).AddSeconds(59);
                    ngay_tao_to = XMedia.XUtil.TimeInEpoch(date2);
                    must.Add(new LongRangeQuery() { Field = "ngay_tao", LessThanOrEqualTo = ngay_tao_to });
                }
                if (don_gia_from > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "don_gia", GreaterThanOrEqualTo = don_gia_from });
                }
                if (don_gia_to > 0)
                {
                    must.Add(new NumericRangeQuery() { Field = "don_gia", LessThanOrEqualTo = don_gia_to });
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

                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                if (fields != null)
                    req.Source = new SourceFilter() { Includes = fields };
                req.TrackTotalHits = true;
                var res = client.Search<Job>(req);
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

        private Tuple<List<QueryContainer>, List<QueryContainer>, List<QueryContainer>> BuildQueryForSearch(string app_id, string nguoi_tao, int group, string value_filter, long ngay_nhan_hd, long ngay_tao, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from,
            long ngay_di_lam_to, double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<int> thuoc_tinh2, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
            long ngay_tao_from, long ngay_tao_to, bool is_admin = false, string[] fields = null, string op = "0", string op2 = "0")
        {
            List<QueryContainer> must = new List<QueryContainer>();
            List<QueryContainer> should = new List<QueryContainer>();

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
                    PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.JOB, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });
                //Lấy tất cả người dùng cùng team để lấy JOB của những người dùng này
                var lst_user_name_in_team = UserRepository.Instance.GetAllUserNameByTeam(app_id, group).Select(x => x.user_name);
                if (value_filter == "1")
                {
                    if (lst_should_id.Count() > 0)
                    {
                        must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        //|| new TermsQuery() { Field = "nguoi_tao.keyword", Terms = lst_user_name_in_team }
                        || new TermQuery() { Field = "owner.keyword", Value = nguoi_tao }
                        //|| new TermsQuery() { Field = "owner.keyword", Terms = lst_user_name_in_team }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                        || new IdsQuery() { Values = lst_should_id.Select(x => (Id)x) }
                        );
                    }
                    else
                    {
                        must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new TermQuery() { Field = "owner.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                    }
                }
                else
                {
                    if (lst_should_id.Count() > 0)
                    {
                        must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "nguoi_tao.keyword", Terms = lst_user_name_in_team }
                        || new TermQuery() { Field = "owner.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "owner.keyword", Terms = lst_user_name_in_team }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                        || new IdsQuery() { Values = lst_should_id.Select(x => (Id)x) }
                        );
                    }
                    else
                    {
                        must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "nguoi_tao.keyword", Terms = lst_user_name_in_team }
                        || new TermQuery() { Field = "owner.keyword", Value = nguoi_tao }
                        || new TermsQuery() { Field = "owner.keyword", Terms = lst_user_name_in_team }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                    }
                }
            }
            if (!string.IsNullOrEmpty(id_cong_ty))
            {
                must.Add(new TermQuery() { Field = "cong_ty.id_cong_ty.keyword", Value = id_cong_ty });
            }
            if (!string.IsNullOrEmpty(term))
            {
                must.Add(new QueryStringQuery() { Fields = new string[] { "id_auto", "chuc_danh", "nguoi_lien_he", "noi_dung", "ghi_chu", "cong_ty.ten_cong_ty" }, Query = term });
            }
            if (!string.IsNullOrEmpty(id_ung_vien))
            {
                must.Add(new TermQuery() { Field = "id_ung_vien", Value = id_ung_vien });
            }

            if (ngay_nhan_hd > 0)
            {
                must.Add(new TermQuery() { Field = "ngay_nhan_hd", Value = ngay_nhan_hd });
                //must.Add(new LongRangeQuery() { Field = "ngay_nhan_hd", LessThanOrEqualTo = ngay_nhan_hd });
            }
            if (ngay_tao > 0)
            {
                must.Add(new LongRangeQuery() { Field = "ngay_tao", GreaterThanOrEqualTo = ngay_tao });

                DateTime dt = Convert.ToDateTime(XMedia.XUtil.EpochToTimeString(ngay_tao));
                var date2 = dt.AddHours(23).AddMinutes(59).AddSeconds(59);
                ngay_tao = XMedia.XUtil.TimeInEpoch(date2);
                must.Add(new LongRangeQuery() { Field = "ngay_tao", LessThanOrEqualTo = ngay_tao });
            }

            if (ngay_tao_from > 0)
            {
                must.Add(new LongRangeQuery() { Field = "ngay_tao", GreaterThanOrEqualTo = ngay_tao_from });
            }
            else
            {
                ngay_tao_from = 946684800;
                must.Add(new LongRangeQuery() { Field = "ngay_tao", GreaterThanOrEqualTo = ngay_tao_from });
            }

            if (ngay_tao_to > 0)
            {
                must.Add(new LongRangeQuery() { Field = "ngay_tao", LessThanOrEqualTo = ngay_tao_to });
            }
            else
            {
                ngay_tao_to = 2524608000;
                must.Add(new LongRangeQuery() { Field = "ngay_tao", LessThanOrEqualTo = ngay_tao_to });
            }
            if (don_gia_from > 0)
            {
                must.Add(new NumericRangeQuery() { Field = "don_gia", GreaterThanOrEqualTo = don_gia_from });
            }
            if (don_gia_to > 0)
            {
                must.Add(new NumericRangeQuery() { Field = "don_gia", LessThanOrEqualTo = don_gia_to });
            }
            if (thuoc_tinh.Count() > 0 && thuoc_tinh2.Count() > 0)
            {
                List<QueryContainer> all_thuoc_tinh = new List<QueryContainer>();
                switch (op)
                {
                    case "0": // CHỨA BẤT KỲ
                        all_thuoc_tinh.Add(new TermsQuery()
                        {
                            Field = "thuoc_tinh",
                            Terms = thuoc_tinh.Select(x => (object)x)
                        });
                        break;

                    case "1": // CHỨA TẤT CẢ
                        List<QueryContainer> all_thuoc_tinh2 = new List<QueryContainer>();
                        foreach (var item in thuoc_tinh)
                        {
                            all_thuoc_tinh2.Add(new TermQuery() { Field = "thuoc_tinh", Value = item });
                        }
                        all_thuoc_tinh.Add(new BoolQuery()
                        {
                            Must = all_thuoc_tinh2
                        });
                        break;

                    case "2": //KHÔNG CHỨA
                        all_thuoc_tinh.Add(!new TermsQuery()
                        {
                            Field = "thuoc_tinh",
                            Terms = thuoc_tinh.Select(x => (object)x)
                        });
                        break;
                }
                switch (op2)
                {
                    case "0": // CHỨA BẤT KỲ
                        all_thuoc_tinh.Add(new TermsQuery()
                        {
                            Field = "thuoc_tinh",
                            Terms = thuoc_tinh2.Select(x => (object)x)
                        });
                        break;

                    case "1": // CHỨA TẤT CẢ
                        List<QueryContainer> all_thuoc_tinh2 = new List<QueryContainer>();
                        foreach (var item in thuoc_tinh2)
                        {
                            all_thuoc_tinh2.Add(new TermQuery() { Field = "thuoc_tinh", Value = item });
                        }
                        all_thuoc_tinh.Add(new BoolQuery()
                        {
                            Must = all_thuoc_tinh2
                        });
                        break;

                    case "2": //KHÔNG CHỨA
                        all_thuoc_tinh.Add(!new TermsQuery()
                        {
                            Field = "thuoc_tinh",
                            Terms = thuoc_tinh2.Select(x => (object)x)
                        });
                        break;
                }
                must.AddRange(all_thuoc_tinh);
            }
            else
            {
                if (thuoc_tinh.Count() > 0)
                {
                    switch (op)
                    {
                        case "0": // CHỨA BẤT KỲ
                            if (lst_should_id.Count() > 0)
                            {
                                should.Add(new TermsQuery()
                                {
                                    Field = "thuoc_tinh",
                                    Terms = thuoc_tinh.Select(x => (object)x)
                                });
                            }
                            else
                            {
                                must.Add(new TermsQuery()
                                {
                                    Field = "thuoc_tinh",
                                    Terms = thuoc_tinh.Select(x => (object)x)
                                });
                            }
                            break;

                        case "1": // CHỨA TẤT CẢ
                            List<QueryContainer> all_thuoc_tinh = new List<QueryContainer>();
                            foreach (var item in thuoc_tinh)
                            {
                                all_thuoc_tinh.Add(new TermQuery() { Field = "thuoc_tinh", Value = item });
                            }
                            must.Add(new BoolQuery()
                            {
                                Must = all_thuoc_tinh
                            });
                            break;

                        case "2": //KHÔNG CHỨA
                            must_not.Add(new TermsQuery()
                            {
                                Field = "thuoc_tinh",
                                Terms = thuoc_tinh.Select(x => (object)x)
                            });
                            break;
                    }
                }
                if (thuoc_tinh2.Count() > 0)
                {
                    switch (op2)
                    {
                        case "0": // CHỨA BẤT KỲ
                            must.Add(new TermsQuery()
                            {
                                Field = "thuoc_tinh",
                                Terms = thuoc_tinh2.Select(x => (object)x)
                            });
                            break;

                        case "1": // CHỨA TẤT CẢ
                            List<QueryContainer> all_thuoc_tinh = new List<QueryContainer>();
                            foreach (var item in thuoc_tinh2)
                            {
                                all_thuoc_tinh.Add(new TermQuery() { Field = "thuoc_tinh", Value = item });
                            }
                            must.Add(new BoolQuery()
                            {
                                Must = all_thuoc_tinh
                            });
                            break;

                        case "2": //KHÔNG CHỨA
                            must_not.Add(new TermsQuery()
                            {
                                Field = "thuoc_tinh",
                                Terms = thuoc_tinh2.Select(x => (object)x)
                            });
                            break;
                    }
                }
            }

            if (lst_id.Count() > 0)
            {
                must.Add(new IdsQuery()
                {
                    Values = lst_id.Select(x => (Id)x)
                });
            }
            return new Tuple<List<QueryContainer>, List<QueryContainer>, List<QueryContainer>>(must, should, must_not);
        }

        public List<Job> SearchDefault(string app_id, string nguoi_tao, int group, string value_filter, long ngay_nhan_hd, long ngay_tao, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from,
            long ngay_di_lam_to, double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<int> thuoc_tinh2, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
            long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null, string op = "0", string op2 = "0", Dictionary<string, bool> sort_order = null)
        {
            msg = "";
            total_recs = 0;
            List<Job> lst = new List<Job>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> should = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                var tupe = BuildQueryForSearch(app_id, nguoi_tao, group, value_filter, ngay_nhan_hd, ngay_tao, term, id_ung_vien, id_cong_ty, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, thuoc_tinh, thuoc_tinh2, lst_id, lst_should_id, ngay_tao_from,
                    ngay_tao_to, is_admin, fields, op, op2);
                must = tupe.Item1;
                should = tupe.Item2;
                must_not = tupe.Item3;

                List<ISort> sort = new List<ISort>();
                SearchRequest req = new SearchRequest(_default_index);
                if (sort_order == null)
                {
                    //sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                    //sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                }
                else
                {
                    foreach (var item in sort_order)
                    {
                        if (item.Key == "do_uu_tien")
                        {
                            sort.Add(new FieldSort() { Field = item.Key, Order = item.Value ? SortOrder.Descending : SortOrder.Ascending });
                        }
                        else if (item.Key == "id_auto.keyword" && !item.Value)
                        {
                            //sort.Add(new FieldSort() { Field = item.Key, Order = item.Value ? SortOrder.Descending : SortOrder.Ascending });
                            sort.Add(new FieldSort() { Field = "_score", Order = SortOrder.Ascending });
                        }
                        else
                        {
                            sort.Add(new FieldSort() { Field = "_score", Order = SortOrder.Descending });
                        }
                    }
                }

                req.Query = new QueryContainer(
                   new ScriptScoreQuery()
                   {
                       Query = new BoolQuery() { Must = must, MustNot = must_not, Should = should },
                       Script = new InlineScript("try { Integer.parseInt(doc['id_auto.keyword'].value);}catch(Exception e){ return 0;}")
                   });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;

                if (fields != null)
                    req.Source = new SourceFilter() { Includes = fields };
                req.TrackTotalHits = true;
                var res = client.Search<Job>(req);
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

        public List<Job> Search(string app_id, int page, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Job> lst = new List<Job>();
            try
            {
                List<ISort> sort = new List<ISort>();

                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });

                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Job>(req);
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

        public List<Job> GetMany(string app_id, IEnumerable<string> lst_id, string[] fields)
        {
            List<Job> lst = new List<Job>();
            lst_id = lst_id.Where(x => !string.IsNullOrEmpty(x));
            if (lst_id.Count() > 0)
            {
                var re = client.GetMany<Job>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        var job = item.Source;
                        job.id_job = item.Id;
                        if (job.app_id == app_id)
                            lst.Add(job);
                    }
                }
            }
            return lst;
        }

        public List<Job> GetMany(IEnumerable<string> lst_id)
        {
            List<Job> lst = new List<Job>();
            if (lst_id != null && lst_id.Count() > 0)
            {
                var re = client.GetMany<Job>(lst_id);

                foreach (var item in re)
                {
                    if (item.Found)
                    {
                        item.Source.id_job = item.Id;
                        lst.Add(item.Source);
                    }
                }
            }
            return lst;
        }

        public bool IsOwner(string id, string owner)
        {
            var re = client.Get<Job>(id, g => g.Index(_default_index).SourceIncludes(new string[] { "owner" }));
            if (re.Found)
            {
                return re.Source.owner == owner;
            }
            return false;
        }

        public Job GetByAutoID(string app_id, string id_auto)
        {
            var re = client.Search<Job>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && !q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))
           && q.Term(t => t.Field("id_auto.keyword").Value(id_auto))
           ).Size(1));

            if (re.Total > 0)
            {
                Job job = re.Hits.First().Source;
                job.id_job = re.Hits.First().Id;
                return re.Documents.First();
            }
            return null;
        }

        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<Job>(id, thuoc_tinh);
        }

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<Job>(app_id);
        }

        public async Task<List<string>> GetListIdCongTyByIdJob(string app_id, IEnumerable<string> ids_job)
        {
            QueryContainer qc = new QueryContainer(new TermQuery() { Field = "app_id.keyword", Value = app_id }

            && new TermQuery() { Field = "trang_thai", Value = TrangThai.ACTIVE }
            && new IdsQuery() { Values = ids_job.Select(x => (Id)x) }
            );
            SourceFilter so = new SourceFilter();
            so.Includes = new string[] { "cong_ty.id_cong_ty" };

            var all = await BackupAndScroll<Job>(_default_index, qc, so);

            return all.Where(o => !string.IsNullOrEmpty(o.Source.cong_ty.id_cong_ty)).Select(x => x.Source.cong_ty.id_cong_ty).Distinct().ToList();
        }

        public Tuple<Dictionary<int, long>, List<string>> ThongKeTheoThuocTinh(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from,
            long ngay_di_lam_to, double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
            long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null, string op = "0", IEnumerable<int> thuoc_tinh2 = null, string op2 = "0")
        {
            Dictionary<int, long> dic_thong_ke = new Dictionary<int, long>();
            List<string> lst_id_out = new List<string>();
            msg = "";
            total_recs = 0;
            List<Job> lst = new List<Job>();
            try
            {
                SearchRequest req = new SearchRequest(_default_index);
                var tupe = BuildQueryForSearch(app_id, nguoi_tao, group, string.Empty, 0, 0, term, id_ung_vien, id_cong_ty, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, thuoc_tinh, thuoc_tinh2, lst_id, lst_should_id, ngay_tao_from,
                    ngay_tao_to, is_admin, fields, op, op2);
                var must = tupe.Item1;
                var should = tupe.Item2;
                var must_not = tupe.Item3;
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not, Should = should });
                req.From = 0;
                req.Size = 9999;
                req.Source = new SourceFilter() { Excludes = "*" };
                req.Aggregations = new AggregationDictionary
                {
                    {
                    "thuoc_tinh", new TermsAggregation("tt")
                    {
                        Field="thuoc_tinh",Size=9999
                    }
                    }
                };

                if (fields != null)
                    req.Source = new SourceFilter() { Includes = fields };
                req.TrackTotalHits = true;
                var res = client.Search<Job>(req);
                lst_id_out = res.Hits.Select(x => x.Id).ToList();
                if (res.IsValid)
                {
                    foreach (var item in res.Aggregations.Terms("thuoc_tinh").Buckets)
                    {
                        dic_thong_ke.Add(Convert.ToInt32(item.Key), item.DocCount.GetValueOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                msg = $"{ex.Message}. Trace: {ex.StackTrace}";
            }
            return new Tuple<Dictionary<int, long>, List<string>>(dic_thong_ke, lst_id_out);
        }

        public void MoveData()
        {
            var re = client.Search<Job>(s => s.MatchAll().Size(9999));
            foreach (var item in re.Hits)
            {
                var rex = client.Update<Job, object>(item.Id, u => u.Doc(new { id_auto = item.Source.auto_id.ToString() }));
            }
        }

        public void UpdateAutoId(string app_id, string auto_id_from, string auto_id_to)
        {
            ThuocTinhRepository.Instance.UpdateTest("kfrlRCfAseqb4c_H", 155);

            //var re = client.UpdateByQuery<Job>(u => u.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
            //&& q.Term(t => t.Field("id_auto.keyword").Value(auto_id_from))
            //).Script(s=>new InlineScript("ctx._source.id_auto=params.auto_id_to") { Params = new Dictionary<string, object> { { "auto_id_to", auto_id_to } } }));
        }

        public List<string> GetIdDeleted(string app_id)
        {
            var re = client.Search<Job>(s => s.Source(so => so.ExcludeAll()).Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))

          ).Size(9999));
            return re.Hits.Select(x => x.Id).ToList();
        }

        public List<Job> GetAllJobsIsOwner(string app_id, string nguoi_tao, string[] fields = null, bool is_admin = false)
        {
            List<Job> lst = new List<Job>();
            if (fields == null)
                fields = new string[] { "id_job" };

            if (!is_admin)
            {
                var re = client.Search<Job>(s => s.Source(so => so.Includes(ic => ic.Fields(fields))).Size(1000).Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id))
                    && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                    && q.Term(t => t.Field("owner.keyword").Value(nguoi_tao))
                    && q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))));
                if (re.Total > 0)
                    //var lst_id_out = re.Hits.Select(x => x.Id).ToList();

                    lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            }
            else
            {
                var re = client.Search<Job>(s => s.Source(so => so.Includes(ic => ic.Fields(fields))).Size(1000).Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id))
                    && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                    ));

                if (re.Total > 0)
                    lst = re.Hits.Select(x => ConvertDoc(x)).ToList();
            }
            return lst;
        }
    }
}