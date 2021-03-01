using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUtil
{
    public class ThuocTinhDuLieuRepository : IESRepository
    {
        #region Init
        protected static string _default_index;
        //protected new ElasticClient client;
        private static ThuocTinhDuLieuRepository instance;
        public ThuocTinhDuLieuRepository(string modify_index)
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
        public static ThuocTinhDuLieuRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_thuoc_tinh_du_lieu";
                    instance = new ThuocTinhDuLieuRepository(_default_index);
                }
                return instance;
            }
        }
        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(1)).Map<ThuocTinhDuLieu>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }
        #endregion
        public IEnumerable<ThuocTinhDuLieu> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<ThuocTinhDuLieu>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public bool Index(ThuocTinhDuLieu data)
        {
            string id = $"{data.loai_obj}_{data.id_obj}_{data.nguoi_tao}";
            return Index(_default_index, data, "", id);

        }
        public bool Update(ThuocTinhDuLieu data)
        {
            string id = $"{data.id}";
            data.id = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }
        public bool Delete(string id)
        {
            return Delete<ThuocTinhDuLieu>(_default_index, id);
        }
        public ThuocTinhDuLieu GetById(string id)
        {
            var obj = GetById<ThuocTinhDuLieu>(_default_index, id);
            if (obj != null)
            {
                obj.id = id;
                return obj;
            }
            return null;
        }
        public List<ThuocTinhDuLieu> GetThuocTinhByIdObj(string app_id, string id_obj, string nguoi_tao)
        {
            var re = client.Search<ThuocTinhDuLieu>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
            && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
            && q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))
                && q.Term(t => t.Field("id_obj.keyword").Value(id_obj))).Source(so => so.Includes(ic => ic.Fields(new string[] { "thuoc_tinh" }))).Size(9999));


            if (re.Total > 0)
                return re.Hits.Select(x => ConvertDoc(x)).ToList();
            return new List<ThuocTinhDuLieu>();
        }
        public IEnumerable<int> GetIdThuocTinhByIdObj(string app_id, string id_obj, string nguoi_tao)
        {
            var re = client.Search<ThuocTinhDuLieu>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
            && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
            && q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))
                && q.Term(t => t.Field("id_obj.keyword").Value(id_obj))).Source(so => so.Includes(ic => ic.Fields(new string[] { "thuoc_tinh" }))).Size(9999));


            if (re.Total > 0)
                return re.Documents.SelectMany(x => x.thuoc_tinh);
            return new List<int>();
        }
        public Dictionary<string, List<int>> GetIdThuocTinhByIdObj(string app_id, IEnumerable<string> ids_obj, string nguoi_tao)
        {
            Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
            if (ids_obj != null && ids_obj.Count() > 0)
            {
                var re = client.Search<ThuocTinhDuLieu>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
                && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
                && q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))
                    && q.Terms(t => t.Field("id_obj.keyword").Terms(ids_obj))).Source(so => so.Includes(ic => ic.Fields(new string[] { "thuoc_tinh", "id_obj" }))).Size(9999));


                if (re.Total > 0)
                    foreach (var item in re.Documents)
                    {
                        if (item.id_obj != null)
                        {
                            if (!dic.ContainsKey(item.id_obj))
                            {
                                dic.Add(item.id_obj, new List<int>());
                            }
                            dic[item.id_obj].AddRange(item.thuoc_tinh);
                        }
                    }
            }
            return dic;
        }
        public IEnumerable<int> GetIdThuocTinhByIdObj(string app_id, string id_obj, LoaiThuocTinh loai_obj, string nguoi_tao)
        {
            var re = client.Search<ThuocTinhDuLieu>(s => s.Query(q => q.Term(t => t.Field("app_id.keyword").Value(app_id))
            && q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))
            && q.Term(t => t.Field("loai_obj").Value(loai_obj))
            && q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))
                && q.Term(t => t.Field("id_obj.keyword").Value(id_obj))).Source(so => so.Includes(ic => ic.Fields(new string[] { "thuoc_tinh" }))).Size(9999));


            if (re.Total > 0)
                return re.Documents.SelectMany(x => x.thuoc_tinh);
            return new List<int>();
        }
        ThuocTinhDuLieu ConvertDoc(IHit<ThuocTinhDuLieu> hit)
        {
            ThuocTinhDuLieu u = new ThuocTinhDuLieu();

            try
            {
                u = hit.Source;
                u.id = hit.Id;
            }
            catch
            {
            }
            return u;
        }
        public List<ThuocTinhDuLieu> Search(string app_id, string nguoi_tao, LoaiThuocTinh loai, List<int> thuoc_tinh, int page, out long total_recs, out string msg, int page_size = 50, string op = "0", List<int> thuoc_tinh2 = null, string op2 = "0")
        {

            msg = "";
            total_recs = 0;
            List<ThuocTinhDuLieu> lst = new List<ThuocTinhDuLieu>();
            try
            {

                List<QueryContainer> must = new List<QueryContainer>();
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery()
                {
                    Field = "loai_obj",
                    Value = loai
                } && new TermQuery()
                {
                    Field = "nguoi_tao.keyword",
                    Value = nguoi_tao
                });
                if (thuoc_tinh.Count > 0 && (thuoc_tinh2 != null && thuoc_tinh2.Count > 0))
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
                    if (thuoc_tinh.Count > 0)
                    {
                        switch (op)
                        {
                            case "0":
                                must.Add(new TermsQuery()
                                {
                                    Field = "thuoc_tinh",
                                    Terms = thuoc_tinh.Select(x => (object)x)
                                });
                                break;
                            case "1":
                                List<QueryContainer> all_thuoc_tinh = new List<QueryContainer>();
                                foreach (var item in thuoc_tinh)
                                {
                                    all_thuoc_tinh.Add(new TermQuery()
                                    {
                                        Field = "thuoc_tinh",
                                        Value = item
                                    });
                                }
                                must.Add(new BoolQuery()
                                {
                                    Must = all_thuoc_tinh
                                });
                                break;
                            case "2":
                                must.Add(!new TermsQuery()
                                {
                                    Field = "thuoc_tinh",
                                    Terms = thuoc_tinh.Select(x => (object)x)
                                });
                                break;
                        }
                    }
                    if (thuoc_tinh2 != null && thuoc_tinh2.Count > 0)
                    {
                        switch (op2)
                        {
                            case "0":
                                must.Add(new TermsQuery()
                                {
                                    Field = "thuoc_tinh",
                                    Terms = thuoc_tinh2.Select(x => (object)x)
                                });
                                break;
                            case "1":
                                List<QueryContainer> all_thuoc_tinh = new List<QueryContainer>();
                                foreach (var item in thuoc_tinh2)
                                {
                                    all_thuoc_tinh.Add(new TermQuery()
                                    {
                                        Field = "thuoc_tinh",
                                        Value = item
                                    });
                                }
                                must.Add(new BoolQuery()
                                {
                                    Must = all_thuoc_tinh
                                });
                                break;
                            case "2":
                                must.Add(!new TermsQuery()
                                {
                                    Field = "thuoc_tinh",
                                    Terms = thuoc_tinh2.Select(x => (object)x)
                                });
                                break;
                        }
                    }
                }
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SourceFilter so = new SourceFilter();
                so.Includes = new string[] { "id_obj", "thuoc_tinh", "loai_obj" };
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Source = so;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<ThuocTinhDuLieu>(req);
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
            return IsOwner<ThuocTinhDuLieu>(_default_index, id, nguoi_tao);
        }
        public List<ThuocTinhDuLieu> GetByLoaiGiaTri(string app_id, string nguoi_tao, IEnumerable<int> thuoc_tinh, LoaiThuocTinh loai, bool is_admin)
        {
            if (thuoc_tinh.Count() == 0)
                return new List<ThuocTinhDuLieu>();
            if (is_admin)
            {
                var re = client.Search<ThuocTinhDuLieu>(s => s.Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("loai_obj").Value(loai)) &&
                     q.Terms(t => t.Field("thuoc_tinh").Terms(thuoc_tinh)) &&
                      q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE))

                    ).Size(100));

                return re.Documents.ToList();
            }
            else
            {

                var re = client.Search<ThuocTinhDuLieu>(s => s.Query(q =>
                    q.Term(t => t.Field("app_id.keyword").Value(app_id)) && q.Term(t => t.Field("loai_obj").Value(loai)) &&
                     q.Terms(t => t.Field("thuoc_tinh").Terms(thuoc_tinh)) &&
                      q.Term(t => t.Field("trang_thai").Value(TrangThai.ACTIVE)) &&
                       q.Term(t => t.Field("nguoi_tao.keyword").Value(nguoi_tao))

                    ).Size(100));

                return re.Documents.ToList();
            }
        }
        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<ThuocTinhDuLieu>(app_id);
        }
        public Dictionary<int, long> ThongKeTheoThuocTinh(string app_id, string nguoi_tao, LoaiThuocTinh loai, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
            IEnumerable<string> lst_should_id, IEnumerable<string> lst_must_not_id, bool is_admin = false, string op = "0")
        {
            Dictionary<int, long> dic_thong_ke = new Dictionary<int, long>();
            List<QueryContainer> must = new List<QueryContainer>();
            List<QueryContainer> should = new List<QueryContainer>();
            List<QueryContainer> must_not = new List<QueryContainer>();
            must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });

            must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
            if (lst_must_not_id.Count() > 0)
            {
                must_not.Add(new TermsQuery()
                {
                    Field = "id_obj.keyword",
                    Terms = lst_must_not_id
                });
            }
            if (!is_admin)
            {
                if (lst_should_id.Count() > 0)
                    must.Add(new TermQuery()
                    {
                        Field = "nguoi_tao.keyword",
                        Value = nguoi_tao
                    } || new TermsQuery()
                    {
                        Field = "id_obj.keyword",
                        Terms = lst_should_id
                    });
                else
                    must.Add(new TermQuery()
                    {
                        Field = "nguoi_tao.keyword",
                        Value = nguoi_tao
                    });
            }
            else
            {
                if (lst_should_id.Count() > 0)
                    must.Add(new TermsQuery()
                    {
                        Field = "id_obj.keyword",
                        Terms = lst_should_id
                    });
            }
            must.Add(new TermQuery()
            {
                Field = "loai_obj",
                Value = loai
            });
            if (thuoc_tinh.Count() > 0)
            {
                switch (op)
                {
                    case "0":
                        must.Add(new TermsQuery()
                        {
                            Field = "thuoc_tinh",
                            Terms = thuoc_tinh.Select(x => (object)x)
                        });
                        break;
                    case "1":
                        List<QueryContainer> all_thuoc_tinh = new List<QueryContainer>();
                        foreach (var item in thuoc_tinh)
                        {
                            all_thuoc_tinh.Add(new TermQuery() { Field = "thuoc_tinh", Value = item });
                        }
                        must.Add(new BoolQuery() { Must = all_thuoc_tinh });
                        break;
                    case "2":
                        must.Add(!new TermsQuery()
                        {
                            Field = "thuoc_tinh",
                            Terms = thuoc_tinh.Select(x => (object)x)
                        });
                        break;
                }

            }

            if (lst_id.Count() > 0)
            {
                must.Add(new TermsQuery()
                {
                    Field = "id_obj.keyword",
                    Terms = lst_id
                });
            }
            SearchRequest req = new SearchRequest(_default_index);
            req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
            req.From = 0;
            req.Size = 0;
            //req.Aggregations = new AggregationDictionary
            //    {
            //        {
            //        "thuoc_tinh", new TermsAggregation("tt")
            //        {
            //            Field="thuoc_tinh",Size=9999
            //        }
            //        }
            //    };
            req.Aggregations = new FilterAggregation("thuoc_tinh")
            {
                Filter = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not }),
                Aggregations =  new TermsAggregation("tt") { Field = "thuoc_tinh" }
            };
            var res = client.Search<ThuocTinhDuLieu>(req);
            if (res.IsValid)
            {
                foreach (var item in res.Aggregations.Filter("thuoc_tinh").Terms("tt").Buckets)
                {
                    dic_thong_ke.Add(Convert.ToInt32(item.Key), item.DocCount.GetValueOrDefault());
                }
            }
            return dic_thong_ke;
        }
    }
}
