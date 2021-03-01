using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUtil
{
    public class NoteUngVienRepository : IESRepository
    {
        #region Init
        protected static string _default_index;
        //protected new ElasticClient client;
        private static NoteUngVienRepository instance;
        public NoteUngVienRepository(string modify_index)
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
        public static NoteUngVienRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_note_ung_vien";
                    instance = new NoteUngVienRepository(_default_index);
                }
                return instance;
            }
        }
        public bool CreateIndex(bool delete_if_exist = false)
        {
            if (delete_if_exist && client.Indices.Exists(_default_index).Exists)
                client.Indices.Delete(_default_index);

            var createIndexResponse = client.Indices.Create(_default_index, s => s.Settings(st => st.NumberOfReplicas(0).NumberOfShards(5)).Map<NoteUngVien>(mm => mm.AutoMap().Dynamic(true)));
            return createIndexResponse.IsValid;
        }
        #endregion
        public IEnumerable<NoteUngVien> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<NoteUngVien>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }
        public bool Index(NoteUngVien data)
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
        public bool Update(NoteUngVien data)
        {
            string id = $"{data.id_note_ung_vien}";
            data.id_note_ung_vien = string.Empty;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }
        public bool Delete(string id)
        {
            return Delete<NoteUngVien>(_default_index, id);
        }
        public NoteUngVien GetById(string id)
        {
            var obj = GetById<NoteUngVien>(_default_index, id);
            if (obj != null)
            {
                obj.id_note_ung_vien = id;
                return obj;
            }
            return null;
        }
        NoteUngVien ConvertDoc(IHit<NoteUngVien> hit)
        {
            NoteUngVien u = new NoteUngVien();

            try
            {
                u = hit.Source;
                u.id_note_ung_vien = hit.Id;
            }
            catch
            {
            }
            return u;
        }
        public List<NoteUngVien> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            
            msg = "";
            total_recs = 0;
            List<NoteUngVien> lst = new List<NoteUngVien>();
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
                       PhanQuyenRepository.Instance.GetQuyenActive(nguoi_tao, group, PhanQuyenObjType.NOTE_UNG_VIEN, new List<int>() { (int)Quyen.VIEW, (int)Quyen.EDIT }, new string[] { "obj_id" });

                    must.Add(
                        new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao }
                        || new IdsQuery() { Values = lst_pq.Select(x => (Id)x.obj_id) }
                    );
                    
                }
                if (!string.IsNullOrEmpty(term))
                {
                    if (ValidateQuery(term))

                        must.Add(new QueryStringQuery() { Fields = new string[] { "ghi_chu" }, Query = term });
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
                if (!string.IsNullOrEmpty(id_ung_vien))
                {
                    //must.Add(new IdsQuery() { Values = new List<Id>() { new Id(id_ung_vien) } });
                    must.Add(new TermQuery() { Value = id_ung_vien,Field="id_ung_vien.keyword" } );
                }
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot=must_not });
                req.From = (page - 1) * page_size;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<NoteUngVien>(req);
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
            return IsOwner<NoteUngVien>(_default_index, id,nguoi_tao);
        }
        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<NoteUngVien>(id, thuoc_tinh);
        }
        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<NoteUngVien>(app_id);
        }
    }
}
