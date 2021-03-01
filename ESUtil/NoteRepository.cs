using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESUtil
{
    public class NoteRepository : IESRepository
    {
        #region Init

        protected static string _default_index;

        //protected new ElasticClient client;
        private static NoteRepository instance;

        public NoteRepository(string modify_index)
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

        public static NoteRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    _default_index = "hh_note";
                    instance = new NoteRepository(_default_index);
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

        #endregion Init

        public IEnumerable<NoteUngVien> GetAll()
        {
            List<QueryContainer> must = new List<QueryContainer>();
            must.Add(new MatchAllQuery());
            return GetObjectScroll<NoteUngVien>(_default_index, new QueryContainer(new BoolQuery() { Must = must }), new SourceFilter());
        }

        public bool Index(Note data)
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

        public bool Update(Note data)
        {
            string id = $"{data.id}";
            data.id = id;
            data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            return Update(_default_index, data, id);
        }

        public bool Delete(string id)
        {
            return Delete<Note>(_default_index, id);
        }

        public Note GetById(string id)
        {
            var obj = GetById<Note>(_default_index, id);
            if (obj != null)
            {
                obj.id = id;
                return obj;
            }
            return null;
        }

        private Note ConvertDoc(IHit<Note> hit)
        {
            Note u = new Note();

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

        public List<Note> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
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
                    must.Add(new TermQuery() { Value = id_ung_vien, Field = "id_ung_vien.keyword" });
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
                var res = client.Search<Note>(req);
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

        public List<Note> Search(IEnumerable<int> thuoc_tinh, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NHA_TUYEN_DUNG });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = LoaiDuLieu.HE_THONG });
                if (thuoc_tinh.Count() > 0)
                {
                    must.Add(new TermsQuery()
                    {
                        Field = "thuoc_tinh",
                        Terms = thuoc_tinh.Select(x => (object)x)
                    });
                }

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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

        public List<Note> GetAllLogNhaTuyenDung(string id_obj, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NHA_TUYEN_DUNG });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = LoaiDuLieu.HE_THONG });
                must.Add(new TermQuery() { Field = "id_obj.keyword", Value = id_obj });

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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

        public List<Note> GetAllNhaTuyenDungXemCvUngVien(IEnumerable<string> id_obj, string nguoi_tao, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NOTE_UNG_VIEN_JOB });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = LoaiDuLieu.HE_THONG });
                must.Add(new TermsQuery() { Field = "id_obj.keyword", Terms = id_obj });
                must.Add(new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao });
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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

        public List<Note> GetLogXemCvTuyenDung(IEnumerable<string> id_obj, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NOTE_UNG_VIEN_JOB });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = LoaiDuLieu.HE_THONG });
                must.Add(new TermsQuery() { Field = "id_obj.keyword", Terms = id_obj });

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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

        public List<Note> NhaTuyenDungNoteUngVien(IEnumerable<string> id_obj, string nguoi_tao, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NOTE_UNG_VIEN_JOB });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = LoaiDuLieu.NGUOI_DUNG });
                must.Add(new TermsQuery() { Field = "id_obj.keyword", Terms = id_obj });
                must.Add(new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao });
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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

        public Note GetNoteByIdObj(string id_obj)
        {
            var re = client.Search<Note>(s => s.Query(q => q.Term(t => t.Field("id_obj.keyword").Value(id_obj))
            && !q.Term(t => t.Field("trang_thai").Value(TrangThai.DELETED))
            && q.Term(t => t.Field("loai_du_lieu").Value(LoaiDuLieu.NGUOI_DUNG))
            && q.Term(t => t.Field("loai").Value(LoaiNote.NOTE_UNG_VIEN_GUI_NHA_TUYEN_DUNG))
           ).Size(1));
            if (re.Total > 0)
            {
                Note note = re.Hits.First().Source;
                note.id = re.Hits.First().Id;
                return re.Documents.First();
            }
            return null;
        }

        public List<Note> GetListNoteByIdObj(IEnumerable<string> id_obj, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "loai", Value = LoaiNote.NOTE_UNG_VIEN_GUI_NHA_TUYEN_DUNG });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = LoaiDuLieu.NGUOI_DUNG });
                must.Add(new TermsQuery() { Field = "id_obj.keyword", Terms = id_obj });

                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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
            return IsOwner<NoteUngVien>(_default_index, id, nguoi_tao);
        }

        public bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UpdateThuocTinh<NoteUngVien>(id, thuoc_tinh);
        }

        public bool RemoveDataByAppId(string app_id)
        {
            return RemoveDataByAppId<NoteUngVien>(app_id);
        }

        public List<Note> GetNoteByObject(string app_id, IEnumerable<string> id_obj, LoaiNote loai, LoaiDuLieu loai_du_lieu, string nguoi_tao, out long total_recs, out string msg, int page_size = 50)
        {
            msg = "";
            total_recs = 0;
            List<Note> lst = new List<Note>();
            try
            {
                List<QueryContainer> must = new List<QueryContainer>();
                List<QueryContainer> must_not = new List<QueryContainer>();
                must_not.Add(new TermQuery() { Field = "trang_thai", Value = TrangThai.DELETED });
                must.Add(new TermQuery() { Field = "app_id.keyword", Value = app_id });
                must.Add(new TermsQuery() { Field = "id_obj.keyword", Terms = id_obj });
                must.Add(new TermQuery() { Field = "loai", Value = loai });
                must.Add(new TermQuery() { Field = "loai_du_lieu", Value = loai_du_lieu });
                must.Add(new TermQuery() { Field = "nguoi_tao.keyword", Value = nguoi_tao });
                List<ISort> sort = new List<ISort>();
                sort.Add(new FieldSort() { Field = "ngay_sua", Order = SortOrder.Descending });
                sort.Add(new FieldSort() { Field = "ngay_tao", Order = SortOrder.Descending });
                SearchRequest req = new SearchRequest(_default_index);
                req.Query = new QueryContainer(new BoolQuery() { Must = must, MustNot = must_not });
                req.From = 0;
                req.Size = page_size;
                req.Sort = sort;
                req.TrackTotalHits = true;
                var res = client.Search<Note>(req);
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

        public int IndexMany(List<Note> data)
        {
            var bulk = new BulkDescriptor();
            foreach (var item in data)
            {
                string id = item.id_obj;

                if (client.Search<Note>(s => s.Size(0).Query(q =>
                          q.Term((t => t.Field("trang_thai").Value((TrangThai.ACTIVE)))) &&
                          q.Ids(i => i.Values(id)))).Total <= 0)
                {
                    item.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    item.ngay_tao = item.ngay_sua;
                    //item.id = id;
                    //bulk.Index<Note>(i => i.Id(id).Document(item));
                    bulk.Index<Note>(i => i.Document(item));
                }
            }
            var re = client.Bulk(bulk);
            var count = re.ItemsWithErrors.Count();
            return data.Count - count;
        }
    }
}