using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QLCUNL.API.Models;
using QLCUNL.BL;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteUngVienJobController : APIBase, IAPIBase
    {
        private string[] fields = new string[] { "id_note_ung_vien_job", "id_ung_vien", "id_job", "id_user_job", "thuoc_tinh", "luong_chinh_thuc", "luong_thu_viec", "ngay_di_lam", "ngay_gio_phong_van", "ghi_chu" };

        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.GetById(id);
            if (!IsInAppId(note_ung_vien_job)) return BadRequest();
            if (note_ung_vien_job != null)
            {
                var lst_id_job = new List<string>() { note_ung_vien_job.id_job };
                var lst_id_ung_vien = new List<string>() { note_ung_vien_job.id_ung_vien };
                var data_job = JobBL.GetMany(app_id, lst_id_job, new string[] { "chuc_danh", "cong_ty", "nguoi_lien_he" }).ToDictionary(x => x.id_job, y => y);
                var data_ung_vien = UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);

                var lst_id_thuoc_tinh = note_ung_vien_job.thuoc_tinh != null ? note_ung_vien_job.thuoc_tinh : new List<int>();
                var lst_thuoc_tinh_rieng = ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, note_ung_vien_job.id_note_ung_vien_job, user);

                var data_thuoc_tinh = ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, ThuocTinhType.SHARED);
                if (lst_thuoc_tinh_rieng.Count() > 0)
                {
                    var thuoc_tinh_rieng = ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, (is_sys_admin || is_app_admin));
                    data_thuoc_tinh.AddRange(thuoc_tinh_rieng);
                }

                Models.NoteUngVienJobMap note = new Models.NoteUngVienJobMap(note_ung_vien_job, data_job, data_ung_vien, data_thuoc_tinh, false);

                return Ok(new DataResponse() { data = note, success = note_ung_vien_job != null, msg = "" });
            }

            return Ok(new DataResponse() { data = new { }, success = false, msg = "" });
        }

        // POST: api/NoteUngVienJob
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_ung_vien_job = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteUngVienJob>(value.ToString());
                SetMetaData(note_ung_vien_job, false);
                string id_note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.Index(note_ung_vien_job);
                res.success = !string.IsNullOrEmpty(id_note_ung_vien_job);
                UpsertThuocTinhRieng(value, id_note_ung_vien_job, LoaiThuocTinh.NOTE_UNG_VIEN_JOB);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        [HttpPost]
        [Route("grant")]
        public IActionResult Grant([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = JToken.Parse(value.ToString());
                List<NoteUngVienJob> lst = new List<NoteUngVienJob>();

                if (obj != null)
                {
                    string id_user_job = obj["id_user_job"].ToString();
                    string id_job = obj["id_job"].ToString();
                    List<string> id_ung_viens = obj["id_ung_viens"].ToObject<List<string>>();

                    foreach (var id_ung_vien in id_ung_viens)
                    {
                        var ung_vien = BL.UngVienBL.GetById(id_ung_vien)?.noi_dung;
                        NoteUngVienJob note = new NoteUngVienJob();
                        note.id_job = id_job;
                        note.id_user_job = id_user_job;
                        note.id_ung_vien = id_ung_vien;
                        if (!string.IsNullOrEmpty(ung_vien))
                        {
                            note.ghi_chu = ung_vien;
                        }
                        note.nguoi_tao = user;
                        note.nguoi_sua = user;
                        note.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                        note.ngay_sua = note.ngay_tao;
                        SetMetaData(note, false);
                        lst.Add(note);
                    }
                }
                var count = QLCUNL.BL.NoteUngVienJobBL.IndexMany(lst);
                res.success = count > 0;
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }
            return Ok(res);
        }

        [HttpPost]
        [Route("getautoid")]
        public IActionResult GetByAutoID(string id_auto, string id_ung_vien)
        {
            var get_job = QLCUNL.BL.JobBL.GetByAutoID(app_id, id_auto);
            DataResponse res = new DataResponse();
            if (get_job != null)
            {
                var id_job = get_job.id_job;
                var user_job = QLCUNL.BL.UserJobBL.GetUserJobByIdJob(app_id, id_job, user);

                if (user_job.Count > 0)
                {
                    var id_user_job = "";
                    foreach (var id_userj in user_job)
                    {
                        if (id_userj.id_user_job != null)
                            id_user_job = id_userj.id_user_job;
                    }
                    try
                    {
                        List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
                        List<string> id_ung_viens = id_ung_vien.Split(",").ToList();
                        foreach (var ung_vien in id_ung_viens)
                        {
                            var tt_ungvien = BL.UngVienBL.GetById(ung_vien)?.noi_dung;
                            NoteUngVienJob note = new NoteUngVienJob();
                            note.id_job = id_job;
                            note.id_user_job = id_user_job;
                            note.id_ung_vien = ung_vien;
                            if (!string.IsNullOrEmpty(tt_ungvien))
                            {
                                note.ghi_chu = tt_ungvien;
                            }
                            note.nguoi_tao = user;
                            note.nguoi_sua = user;
                            note.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                            note.ngay_sua = note.ngay_tao;
                            SetMetaData(note, false);
                            lst.Add(note);
                        }
                        var count = QLCUNL.BL.NoteUngVienJobBL.IndexMany(lst);
                        res.success = count > 0;
                    }
                    catch (Exception ex)
                    {
                        res.msg = ex.Message; res.success = false;
                    }

                    return Ok(res);
                }

                res.msg = "Bạn phải gán JOB cho mình trước!";
                res.success = false;
                return Ok(res);
            }
            res.msg = "Không tìm thấy JOB theo Auto ID này!";
            res.success = false;
            return Ok(res);
        }

        // PUT: api/NoteUngVienJob/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_ung_vien_job = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteUngVienJob>(value.ToString());
                var obj = JToken.Parse(value.ToString());
                if (!string.IsNullOrEmpty(obj["note_note_uv_job"].ToString()))
                {
                    var note = new Note();
                    note.noi_dung = obj["note_note_uv_job"].ToString();
                    note.id_obj = id;
                    note.loai = LoaiNote.NOTE_UNG_VIEN_JOB;
                    note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                    note.thuoc_tinh = new List<int>();
                    SetMetaData(note, false);
                    res.success = NoteBL.Index(note);
                }
                note_ung_vien_job.id_note_ung_vien_job = id;
                SetMetaData(note_ung_vien_job, true);
                res.success = QLCUNL.BL.NoteUngVienJobBL.Update(note_ung_vien_job);
                UpsertThuocTinhRieng(value, id, LoaiThuocTinh.NOTE_UNG_VIEN_JOB);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        [HttpPut()]
        [Route("trangthai")]
        public IActionResult UpdateTrangThai(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_ung_vien_job = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteUngVienJob>(value.ToString());
                note_ung_vien_job.id_note_ung_vien_job = id;
                res.success = QLCUNL.BL.NoteUngVienJobBL.UpdateThuocTinh(note_ung_vien_job);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        [HttpPut()]
        [Route("giophongvan")]
        public IActionResult UpdateGioPhongVan(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_ung_vien_job = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteUngVienJob>(value.ToString());
                note_ung_vien_job.id_note_ung_vien_job = id;
                res.success = QLCUNL.BL.NoteUngVienJobBL.UpdateGioPhongVan(note_ung_vien_job);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        [Route("delete")]
        public bool Delete(string id)
        {
            bool del = NoteUngVienJobBL.Delete(id);
            return del;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, string id_user, string id_job, string id_ung_vien, long ngay_gio_phong_van_from, long ngay_gio_phong_van_to,
            long ngay_di_lam_from, long ngay_di_lam_to, double luong_thu_viec_from, long luong_thu_viec_to, double luong_chinh_thuc_from, long luong_chinh_thuc_to, string thuoc_tinh, string thuoc_tinh_rieng
            , int page, int page_size)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<NoteUngVienJob> data_note_ung_vien_job = new List<NoteUngVienJob>();
            long total_recs = 0;
            string msg = "";

            if (!string.IsNullOrEmpty(thuoc_tinh))
            {
                foreach (var tt in thuoc_tinh.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh.Add(t);
                    }
                }
            }
            if (!string.IsNullOrEmpty(thuoc_tinh_rieng))
            {
                foreach (var tt in thuoc_tinh_rieng.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh_rieng.Add(t);
                    }
                }
                if (lst_thuoc_tinh_rieng.Count > 0)
                {
                    is_find_thuoc_tinh_rieng = true;
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, lst_thuoc_tinh_rieng, page, out long total_recs_thuoc_tinh, out _, 9999);
                    lst_id = lst_id_obj.Select(x => x.id_obj).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0 && lst_thuoc_tinh.Count == 0)
            {
            }
            else
                data_note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.Search(app_id, user, group, "", id_user, id_job, id_ung_vien, ngay_gio_phong_van_from, ngay_gio_phong_van_to, ngay_di_lam_from, ngay_di_lam_to,
            luong_thu_viec_from, luong_thu_viec_to, luong_chinh_thuc_from, luong_chinh_thuc_to, lst_thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin), fields);
            var lst_id_job = data_note_ung_vien_job.Select(x => x.id_job);
            var lst_id_ung_vien = data_note_ung_vien_job.Select(x => x.id_ung_vien);
            var data_job = QLCUNL.BL.JobBL.GetMany(app_id, lst_id_job, new string[] { "chuc_danh", "cong_ty", "nguoi_lien_he" }).ToDictionary(x => x.id_job, y => y);
            var data_ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
            var lst_id_thuoc_tinh = data_note_ung_vien_job.Where(x => x.thuoc_tinh != null).SelectMany(x => x.thuoc_tinh).ToList();

            // tìm note nhà tuyển dụng của từng ứng viên.
            long total_seen = 0;
            var list_id_note_uv_job = data_note_ung_vien_job.Select(x => x.id_note_ung_vien_job);

            var tim_ntd_by_list_note = NhaTuyenDungBL.GetNhaTuyenDungByLstIdShare(app_id, list_id_note_uv_job, 1, out total_seen, out msg, 9999);
            var id_ntd = tim_ntd_by_list_note.Select(x => x.id);
            var id_nha_tuyen_dung = tim_ntd_by_list_note.Select(x => x.id_nha_tuyen_dung);
            //var nguoi_tao = string.Join(",", id_nha_tuyen_dung);
            //var note_nha_tuyen_dung = NoteBL.NhaTuyenDungNoteUngVien(id_ntd, nguoi_tao, out total_seen, out msg, 9999);

            var list_name = new List<string>();
            var lst_note = new List<Note>();
            foreach (var nguoi_tao in tim_ntd_by_list_note)
            {
                list_name.Add(nguoi_tao.id_nha_tuyen_dung);
            }

            foreach (var item in list_name)
            {
                var ntd_xem_cv_ung_vien = NoteBL.GetAllNhaTuyenDungXemCvUngVien(list_id_note_uv_job, item, out total_seen, out msg, 9999);
                lst_note.AddRange(ntd_xem_cv_ung_vien);
            }

            List<NoteUngVienJobMap> lst = new List<NoteUngVienJobMap>();

            var dic_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, data_note_ung_vien_job.Select(x => x.id_note_ung_vien_job), user);
            if (dic_thuoc_tinh_rieng.Count > 0)
            {
                foreach (var tt_r in dic_thuoc_tinh_rieng)
                {
                    lst_id_thuoc_tinh.AddRange(tt_r.Value);
                }
            }
            var data_thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, -1);
            foreach (var item in data_note_ung_vien_job)
            {
                dic_thuoc_tinh_rieng.TryGetValue(item.id_note_ung_vien_job, out List<int> tt_rieng);
                if (tt_rieng == null)
                    tt_rieng = new List<int>();
                NoteUngVienJobMap note = new NoteUngVienJobMap(item, data_job, data_ung_vien, data_thuoc_tinh, tt_rieng);

                foreach (var em in lst_note.Where(x => x.id_obj.Contains(item.id_note_ung_vien_job)))
                {
                    if (em.id_obj == item.id_note_ung_vien_job)
                    {
                        note.xem_cv = true;
                    }
                    else
                    {
                        note.xem_cv = false;
                    }
                }

                lst.Add(note);
            }

            return Ok(new DataResponsePaging() { data = lst, total = total_recs, success = data_note_ung_vien_job != null, msg = msg });
        }

        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.NOTE_UNG_VIEN_JOB, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }

        [HttpGet]
        [Route("getnotebyid")]
        public IActionResult GetNoteUngVienJobByIdUngVien(string id_ung_vien, string id_job)
        {
            long total_recs = 0;
            string msg = "";

            var data_note_ung_vien = QLCUNL.BL.NoteUngVienJobBL.GetNoteUngVienJobByIdUngVien(app_id, id_ung_vien.Split(","), id_job, 1, out total_recs, out msg, 9999, (is_sys_admin || is_app_admin));
            var lst_id_job = data_note_ung_vien.Select(x => x.id_job);
            var data_job = QLCUNL.BL.JobBL.GetMany(app_id, lst_id_job, new string[] { "chuc_danh", "cong_ty", "nguoi_lien_he" }).ToDictionary(x => x.id_job, y => y);
            var lst_id_ung_vien = data_note_ung_vien.Select(x => x.id_ung_vien);

            var lst_id_cong_ty = data_job.Where(x => x.Value.cong_ty != null && !string.IsNullOrEmpty(x.Value.cong_ty.id_cong_ty)).Select(x => x.Value.cong_ty.id_cong_ty);
            var lst_cong_ty = QLCUNL.BL.CongTyBL.GetMany(lst_id_cong_ty);
            Dictionary<string, string> dic_cong_ty = new Dictionary<string, string>();

            if (lst_cong_ty != null)
            {
                dic_cong_ty = lst_cong_ty.ToDictionary(x => x.id_cong_ty, y => y.ten_cong_ty);
            }
            var data_ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
            List<Models.NoteUngVienJobMap> lst = new List<Models.NoteUngVienJobMap>();
            foreach (var item in data_note_ung_vien)
            {
                var lst_id_thuoc_tinh = item.thuoc_tinh != null ? item.thuoc_tinh : new List<int>();
                var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, item.id_note_ung_vien_job, user);

                var data_thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, ThuocTinhType.SHARED);
                if (lst_thuoc_tinh_rieng.Count() > 0)
                {
                    var thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, (is_sys_admin || is_app_admin));
                    data_thuoc_tinh.AddRange(thuoc_tinh_rieng);
                }
                Models.NoteUngVienJobMap note = new Models.NoteUngVienJobMap(item, data_job, dic_cong_ty, data_ung_vien, data_thuoc_tinh);
                lst.Add(note);
            }
            return Ok(new DataResponsePaging() { data = lst, total = total_recs, success = data_note_ung_vien != null, msg = msg });
        }

        [HttpGet]
        [Route("getnotebyidjob")]
        public IActionResult GetNoteUngVienByIdJob(string id_job)
        {
            List<Models.NoteUngVienJobMap> lst = new List<Models.NoteUngVienJobMap>();
            //Check Owner theo ID_JOB truoc roi chay doan code duoi
            if (JobBL.IsOwner(id_job, user) || (is_app_admin || is_sys_admin))
            {
                var data_note_ung_vien = BL.NoteUngVienJobBL.GetNoteUngVienJobByIdJobOwner(app_id, id_job);
                var lst_id_job = data_note_ung_vien.Select(x => x.id_job);
                var data_job = QLCUNL.BL.JobBL.GetMany(app_id, lst_id_job, new string[] { "chuc_danh", "cong_ty", "nguoi_lien_he" }).ToDictionary(x => x.id_job, y => y);
                var lst_id_ung_vien = data_note_ung_vien.Select(x => x.id_ung_vien);
                var data_ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
                foreach (var item in data_note_ung_vien)
                {
                    var lst_id_thuoc_tinh = item.thuoc_tinh != null ? item.thuoc_tinh : new List<int>();
                    var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, item.id_note_ung_vien_job, user);

                    var data_thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, ThuocTinhType.SHARED);
                    if (lst_thuoc_tinh_rieng.Count() > 0)
                    {
                        var thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, (is_sys_admin || is_app_admin));
                        data_thuoc_tinh.AddRange(thuoc_tinh_rieng);
                    }
                    Models.NoteUngVienJobMap note = new Models.NoteUngVienJobMap(item, data_job, data_ung_vien, data_thuoc_tinh);

                    lst.Add(note);
                }
            }
            return Ok(new DataResponsePaging() { data = lst, success = true, msg = "", total = lst.Count });
        }

        [HttpPost]
        [Route("savethuoctinh")]
        public IActionResult SaveThuocTinh([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                //{"id_ung_vien":"123123123123", "thuoc_tinh_ung_vien":[], "ghi_chu": "gi do", "thuoc_tinh_ghi_chu":[]}
                var obj = JToken.Parse(value.ToString());
                if (obj != null)
                {
                    var is_ok = false;
                    string id_ung_vien = obj["id_ung_vien"].ToString();
                    string id_note_uv_job = obj["id_obj"].ToString();
                    List<int> thuoc_tinh_uv_chung = obj["thuoc_tinh_uv_chung"].ToObject<List<int>>();
                    List<int> thuoc_tinh_uv_rieng = obj["thuoc_tinh_uv_rieng"].ToObject<List<int>>();
                    List<int> thuoc_tinh_job = obj["thuoc_tinh"].ToObject<List<int>>();
                    List<int> thuoc_tinh_job_rieng = obj["thuoc_tinh_rieng"].ToObject<List<int>>();
                    //thuộc tính ứng viên
                    is_ok = QLCUNL.BL.UngVienBL.SetThuocTinh(id_ung_vien, thuoc_tinh_uv_chung);
                    if (thuoc_tinh_uv_rieng.Count > 0)
                    {
                        ThuocTinhDuLieu thuoc_tinh_rieng = new ThuocTinhDuLieu();

                        thuoc_tinh_rieng.id_obj = id_ung_vien;
                        thuoc_tinh_rieng.loai_obj = LoaiThuocTinh.UNG_VIEN;
                        thuoc_tinh_rieng.thuoc_tinh = thuoc_tinh_uv_rieng;
                        SetMetaData(thuoc_tinh_rieng, false);
                        is_ok = is_ok | QLCUNL.BL.ThuocTinhDuLieuBL.Index(thuoc_tinh_rieng);
                    }

                    if (thuoc_tinh_job_rieng != null)
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ThuocTinhDuLieu>(value.ToString());
                        data.thuoc_tinh = thuoc_tinh_job_rieng;
                        if (thuoc_tinh_job_rieng.Count > 0)
                        {
                            bool is_valid = true;
                            if (((int)data.loai_obj) == -1)
                            {
                                res.msg = "Cần chọn đối tượng";
                                is_valid = is_valid & false;
                            }
                            else
                            {
                                is_valid = is_valid & true;

                                if (data.thuoc_tinh == null || (data.thuoc_tinh != null && data.thuoc_tinh.Count == 0))
                                {
                                    res.msg = "Cần chọn thuộc tính";
                                    is_valid = is_valid & false;
                                }
                                else
                                {
                                    is_valid = is_valid & true;

                                    if (string.IsNullOrEmpty(data.id_obj))
                                    {
                                        res.msg = "Thiếu ID đối tượng";
                                        is_valid = is_valid & false;
                                    }
                                    else
                                    {
                                        is_valid = is_valid & true;
                                    }
                                }
                            }

                            if (is_valid)
                            {
                                SetMetaData(data, false);
                                res.success = QLCUNL.BL.ThuocTinhDuLieuBL.Index(data);
                            }
                        }
                    }
                    if (thuoc_tinh_job != null)
                    {
                        string id = obj["id_obj"].ToString();
                        string loai_obj = obj["loai_obj"].ToString();
                        var thuoc_tinh = thuoc_tinh_job;

                        switch (loai_obj)
                        {
                            case "JOB":
                                res.success = JobBL.UpdateThuocTinh(id, thuoc_tinh);
                                break;

                            case "CONG_TY":
                                res.success = CongTyBL.UpdateThuocTinh(id, thuoc_tinh);
                                break;

                            case "NOTE_UNG_VIEN":
                                res.success = NoteUngVienBL.UpdateThuocTinh(id, thuoc_tinh);
                                break;

                            case "UNG_VIEN":
                                res.success = UngVienBL.UpdateThuocTinh(id, thuoc_tinh);
                                break;

                            case "NOTE_UNG_VIEN_JOB":
                                res.success = NoteUngVienJobBL.UpdateThuocTinh(id, thuoc_tinh);
                                break;

                            case "USER_JOB":
                                res.success = UserJobBL.UpdateThuocTinh(id, thuoc_tinh);
                                break;
                        }
                    }
                    // thuộc tính note ứng viên theo job

                    res.success = is_ok;
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.msg = ex.Message;
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("thongkethuoctinh")]
        public IActionResult ThongKeThuocTinh(string term, string id_user, string id_job, string id_ung_vien, long ngay_gio_phong_van_from, long ngay_gio_phong_van_to,
            long ngay_di_lam_from, long ngay_di_lam_to, double luong_thu_viec_from, long luong_thu_viec_to, double luong_chinh_thuc_from, long luong_chinh_thuc_to, string thuoc_tinh, string thuoc_tinh_rieng
            , int page, int page_size)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            Tuple<List<NoteUngVienJob>, Dictionary<int, long>> data_note_ung_vien_job = null;
            long total_recs = 0;
            string msg = "";
            if (!string.IsNullOrEmpty(thuoc_tinh))
            {
                foreach (var tt in thuoc_tinh.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh.Add(t);
                    }
                }
            }
            if (!string.IsNullOrEmpty(thuoc_tinh_rieng))
            {
                foreach (var tt in thuoc_tinh_rieng.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh_rieng.Add(t);
                    }
                }
                if (lst_thuoc_tinh_rieng.Count > 0)
                {
                    is_find_thuoc_tinh_rieng = true;
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.NOTE_UNG_VIEN_JOB, lst_thuoc_tinh_rieng, page, out long total_recs_thuoc_tinh, out _, 9999);
                    lst_id = lst_id_obj.Select(x => x.id_obj).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0 && lst_thuoc_tinh.Count == 0)
            {
            }
            else
                data_note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.Search(app_id, user, group, "", id_user, id_job, id_ung_vien, ngay_gio_phong_van_from, ngay_gio_phong_van_to, ngay_di_lam_from, ngay_di_lam_to,
            luong_thu_viec_from, luong_thu_viec_to, luong_chinh_thuc_from, luong_chinh_thuc_to, lst_thuoc_tinh, lst_id, 1, out total_recs, out msg, 0, (is_sys_admin || is_app_admin), fields, true);

            return Ok(new DataResponse() { data = data_note_ung_vien_job.Item2.Select(x => new { k = x.Key, v = x.Value }), success = data_note_ung_vien_job != null, msg = msg });
        }

        [HttpGet]
        [Route("getnotebyobject")]
        public IActionResult GetNoteByObject(string id_obj)
        {
            long total_recs = 0;
            string msg = "";
            List<string> lst_id = new List<string>();
            lst_id.Add(id_obj);
            var note_ung_vien = NoteBL.GetNoteByObject(app_id, lst_id, LoaiNote.NOTE_UNG_VIEN_JOB, LoaiDuLieu.NGUOI_DUNG, user, out total_recs, out msg);

            return Ok(new DataResponse() { data = note_ung_vien, success = true, msg = "" });
        }
    }
}