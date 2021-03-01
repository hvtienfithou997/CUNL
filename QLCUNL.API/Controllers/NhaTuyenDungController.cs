using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using QLCUNL.API.Models;
using QLCUNL.BL;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaTuyenDungController : APIBase
    {
        protected readonly int seen = -1;

        public NhaTuyenDungController()
        {
        }

        [HttpGet]
        [Route("ungvien")]
        public IActionResult UngVien(string id, string token, string id_ung_vien)
        {
            List<string> lst_id = new List<string>();
            if (!string.IsNullOrEmpty(id_ung_vien))
                lst_id = id_ung_vien.Split(',').ToList();
            List<UngVien> data = new List<UngVien>();
            List<UngVienNhaTuyenDung> list_uv = new List<UngVienNhaTuyenDung>();

            string msg = "";
            int stt = 1;
            var lst_pq = PhanQuyenBL.GetQuyenActive($"{id}|{token}", 0, PhanQuyenObjType.UNG_VIEN, new List<int>() { (int)Quyen.VIEW }, new string[] { "obj_id" });
            var lst_id_ung_vien_shared = lst_pq.Select(x => x.obj_id).ToList();
            lst_id_ung_vien_shared.AddRange(lst_id);

            data = UngVienBL.GetMany(lst_id_ung_vien_shared);

            list_uv = data.Select(x => new UngVienNhaTuyenDung(x) { stt = stt++ }).ToList();
            return Ok(new DataResponse() { data = list_uv, success = data != null, msg = msg });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ungvienjob")]
        public IActionResult UngVienJob(string id, string token, string id_ung_vien)
        {
            List<string> lst_id = new List<string>();
            if (!string.IsNullOrEmpty(id_ung_vien))
                lst_id = id_ung_vien.Split(',').ToList();
            List<NoteUngVienJob> data = new List<NoteUngVienJob>();
            List<NoteUngVienJobNhaTuyenDung> lst_uv = new List<NoteUngVienJobNhaTuyenDung>();
            long total_recs = 0;
            string msg = "";
            var lst_pq = PhanQuyenBL.GetQuyenActive($"{id}|{token}", -1, PhanQuyenObjType.NOTE_UNG_VIEN_JOB, new List<int>() { (int)Quyen.VIEW }, new string[] { "obj_id", "obj_type" });
            var lst_id_note_ung_vien_job_shared = lst_pq.Select(x => x.obj_id).ToList();
            lst_id_note_ung_vien_job_shared.AddRange(lst_id);
            data = NoteUngVienJobBL.GetMany(lst_id_note_ung_vien_job_shared);

            // lấy note ứng viên
            var nha_tuyen_dung = NhaTuyenDungBL.GetByToken(token);
            var lst_note_uv_job = data.Select(x => x.id_note_ung_vien_job);
            var all_note_cua_ntd = NoteBL.NhaTuyenDungNoteUngVien(lst_note_uv_job, nha_tuyen_dung.id_nha_tuyen_dung, out total_recs, out msg, 9999);
            var lst_id_ung_vien = data.Select(x => x.id_ung_vien).ToList();

            var data_ung_vien = UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
            int stt = 1;
            foreach (var item in data)
            {
                data_ung_vien.TryGetValue(item.id_ung_vien, out UngVien ung_vien);
                if (ung_vien != null)
                {
                    foreach (var temp in all_note_cua_ntd.Where(x => x.id_obj == item.id_note_ung_vien_job).OrderByDescending(o => o.ngay_tao).Take(1))
                    {
                        item.ghi_chu_nha_tuyen_dung = temp.noi_dung;
                    }
                    lst_uv.Add(new NoteUngVienJobNhaTuyenDung(item, ung_vien) { stt = stt++ });
                }
            }
            return Ok(new DataResponse() { data = lst_uv, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("viewungvien")]
        public IActionResult NhaTuyenDungViewUngVien(string id_ung_vien)
        {
            var ung_vien = BL.UngVienBL.GetById(id_ung_vien);
            if (ung_vien != null)
            {
                UngVienNhaTuyenDung uv_tuyen_dung = new UngVienNhaTuyenDung(ung_vien);
                return Ok(new DataResponse() { data = uv_tuyen_dung, success = ung_vien != null, msg = "" });
            }
            return Ok(new DataResponse() { data = ung_vien, success = ung_vien != null, msg = "" });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ghichunhatuyendung")]
        public IActionResult UpdateGhiChuNhaTuyenDung([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = JToken.Parse(value.ToString());

                if (obj != null)
                {
                    string id_note_ung_vien_job = obj["id_note_ung_vien_job"].ToString();
                    //string ghi_chu_nha_tuyen_dung = obj["ghi_chu_nha_tuyen_dung"].ToString();
                    var ghi_chu_nha_tuyen_dung = obj["ghi_chu_nha_tuyen_dung"].ToObject<Note>();

                    long ngay_gio_phong_van = 0;
                    try
                    {
                        ngay_gio_phong_van = Convert.ToInt64(obj["ngay_gio_phong_van"]);
                    }
                    catch (Exception)
                    {
                    }
                    if (ngay_gio_phong_van > 0)
                    {
                        res.success = QLCUNL.BL.NoteUngVienJobBL.UpdateGioPhongVan(new NoteUngVienJob() { id_note_ung_vien_job = id_note_ung_vien_job, ngay_gio_phong_van = ngay_gio_phong_van });
                    }
                    if (ghi_chu_nha_tuyen_dung != null)
                    {
                        ghi_chu_nha_tuyen_dung.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                        res.success = QLCUNL.BL.NoteBL.Index(ghi_chu_nha_tuyen_dung);
                    }
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }
            return Ok(res);
        }

        [HttpPost]
        [Route("ghichuungvien")]
        public IActionResult UpdateGhiChuUngVien([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = JToken.Parse(value.ToString());
                if (obj != null)
                {
                    string id_note_ung_vien_job = obj["id_note_ung_vien_job"].ToString();
                    string ghi_chu_ung_vien = obj["ghi_chu_ung_vien"].ToString();
                    res.success = QLCUNL.BL.NoteUngVienJobBL.UpdateGhiChuUngVien(id_note_ung_vien_job, ghi_chu_ung_vien, out string msg);
                    res.msg = msg;
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var employer = QLCUNL.BL.NhaTuyenDungBL.GetById(id);
            if (employer != null)
            {
                if (employer.lst_id_share != null)
                {
                    var lst_id_note_ung_vien_job = NoteUngVienJobBL.GetMany(employer.lst_id_share);
                    var lst_id_ung_vien = lst_id_note_ung_vien_job.Select(x => x.id_ung_vien);
                    var ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
                    List<NoteUngVienJobMap> lst = new List<NoteUngVienJobMap>();

                    foreach (var item in lst_id_note_ung_vien_job)
                    {
                        NoteUngVienJobMap map = new NoteUngVienJobMap(item, ung_vien);
                        lst.Add(map);
                    }
                    return Ok(new { value = lst, data = employer, success = employer != null, msg = "" });
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            //value = {'nha_tuyen_dung':{'id_nha_tuyen_dung':'tuyendun@xmedi'},'ung_vien_share':{'id':['uv_1','uv_2'],'obj_type':4}}
            var obj = JToken.Parse(value.ToString());
            DataResponse res = new DataResponse();
            if (obj == null)
            {
                res.success = false;
                res.msg = "Đối tượng bị bỏ trống";
                return Ok(res);
            }
            try
            {
                List<string> note_uv_gui_nha_tuyen_dung = obj["note_ung_vien_gui_nha_tuyen_dung"].ToObject<List<string>>();
                List<Note> lst = new List<Note>();
                var user = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
                //var note_nha_tuyen_dung = obj["note_ung_vien_gui_nha_tuyen_dung"].ToObject<Note>();

                var ip_add = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                string browser = Request.Headers["User-Agent"];
                var add_employer = obj["nha_tuyen_dung"].ToObject<NhaTuyenDung>();
                SetMetaData(add_employer, false);
                add_employer.ip = ip_add;
                add_employer.browser = browser;
                var note_ung_vien_share = obj["note_ung_vien_share"];
                var ids = note_ung_vien_share["id"].ToObject<List<string>>();
                add_employer.lst_id_share = ids;
                var employer = QLCUNL.BL.NhaTuyenDungBL.Index(add_employer, out string token);
                res.success = !string.IsNullOrEmpty(employer);

                List<string> lst_id_nha_tuyen_dung = new List<string>() { add_employer.id_nha_tuyen_dung };

                int type = 1;
                int rule = 1;
                int obj_type = note_ung_vien_share["obj_type"].ToObject<int>();
                long ngay_het = 0;

                List<Quyen> quyen = new List<Quyen>();

                ngay_het = XMedia.XUtil.TimeInEpoch(DateTime.Now.AddMonths(1));
                quyen = new List<Quyen>() { Quyen.VIEW };

                var obj_type_check = (PhanQuyenObjType)obj_type;

                var numbersAndWords = ids.Zip(note_uv_gui_nha_tuyen_dung, (n, w) => new { id_obj = n, noi_dung = w });

                foreach (var notes in numbersAndWords)
                {
                    if (!string.IsNullOrEmpty(notes.noi_dung))
                    {
                        Note note = new Note();
                        note.id_obj = notes.id_obj;
                        note.loai = LoaiNote.NOTE_UNG_VIEN_GUI_NHA_TUYEN_DUNG;
                        note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                        note.nguoi_tao = user;
                        note.nguoi_sua = user;
                        note.noi_dung = notes.noi_dung;
                        SetMetaData(note, false);
                        lst.Add(note);
                    }
                }
                var count = QLCUNL.BL.NoteBL.IndexMany(lst);
                res.success = count > 0;

                foreach (var id in ids)
                {
                    var has_share_permission = false;
                    switch (obj_type_check)
                    {
                        case PhanQuyenObjType.UNG_VIEN:
                            has_share_permission = UngVienBL.IsOwner(id, user);
                            break;

                        case PhanQuyenObjType.NOTE_UNG_VIEN_JOB:
                            has_share_permission = NoteUngVienJobBL.IsOwner(id, user) || (is_app_admin);
                            break;
                    }
                    if (has_share_permission)
                    {
                        #region Xóa shared của các đối tượng cũ

                        try
                        {
                            List<string> lst_id_phan_quyen_can_xoa = new List<string>();
                            var menu_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty, PhanQuyenObjType.ALL, id,
                                quyen, 0, 0, user, 0, 0, string.Empty, 0, 0, 1, 9999, out _);
                            foreach (var item_shared in menu_shared)
                            {
                                if (item_shared.type == PhanQuyenType.USERS)
                                {
                                    if (!lst_id_nha_tuyen_dung.Contains(item_shared.user))
                                    {
                                        lst_id_phan_quyen_can_xoa.Add(item_shared.id);
                                    }
                                    else
                                    {
                                        if (item_shared.ngay_het == ngay_het && item_shared.quyen.All(quyen.Contains) && quyen.All(item_shared.quyen.Contains))
                                            lst_id_phan_quyen_can_xoa.Remove(item_shared.user);
                                    }
                                }
                            }
                            PhanQuyenBL.RemoveByListId(lst_id_phan_quyen_can_xoa);
                        }
                        catch (Exception)
                        {
                        }

                        #endregion Xóa shared của các đối tượng cũ

                        #region Shared cho các đối tượng được chọn

                        try
                        {
                            if (lst_id_nha_tuyen_dung.Count == 0)
                            {
                                res.success = true;
                                res.msg = "Không có thay đổi nào được thực hiện";
                            }
                            else
                            {
                                foreach (var u in lst_id_nha_tuyen_dung)
                                {
                                    PhanQuyen pq = new PhanQuyen();
                                    pq.ngay_het = ngay_het;
                                    pq.user = u + "|" + add_employer.token;
                                    pq.type = PhanQuyenType.USERS;
                                    pq.rule = (PhanQuyenRule)rule;
                                    pq.quyen = quyen;
                                    pq.obj_type = (PhanQuyenObjType)obj_type;
                                    pq.nguoi_tao = user;
                                    pq.obj_id = id;
                                    res.success = QLCUNL.BL.PhanQuyenBL.Index(pq);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        #endregion Shared cho các đối tượng được chọn
                    }
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, int page, int page_size, string thuoc_tinh)
        {
            long total_recs = 0;
            string msg = "";
            List<JobThuocTinhMap> lst_job_map = new List<JobThuocTinhMap>();
            List<NhaTuyenDung> data = new List<NhaTuyenDung>();
            var user = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            List<int> lst_thuoc_tinh = new List<int>();

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
                var lst_ntd_seen = NoteBL.Search(lst_thuoc_tinh, out total_recs, out msg, 9999);
                var lst_ntd_get_id = lst_ntd_seen.Select(x => x.id_obj).Distinct().Where(i => i != null);
                var all_nha_tuyen_dung = BL.NhaTuyenDungBL.GetMany(lst_ntd_get_id).Where(x => x.nguoi_tao == user && x.trang_thai != TrangThai.DELETED);
                var lst_id_job = all_nha_tuyen_dung.Select(x => x.id_job);
                //var data_job = QLCUNL.BL.JobBL.GetMany(lst_id_job).Where(x => x.owner == user && all_nha_tuyen_dung.Select(y => y.id_job).Contains(x.id_job));
                var data_job = QLCUNL.BL.JobBL.GetMany(lst_id_job);
                var list_job = data_job.ToDictionary(x => x.id_job, y => y);
                all_nha_tuyen_dung = all_nha_tuyen_dung.Where(o => data_job.Select(u => u.id_job).Contains(o.id_job)).ToList();
                foreach (var item in all_nha_tuyen_dung)
                {
                    JobThuocTinhMap job_map = new JobThuocTinhMap(item, list_job);
                    lst_job_map.Add(job_map);
                }
                return Ok(new { data = lst_job_map, total = all_nha_tuyen_dung.Count(), success = lst_job_map != null, msg = msg });
            }
            else
            {
                var user_job = UserJobBL.Search(app_id, user, group, new List<string>(), string.Empty, 0, 0, new List<int>(), new List<string>(), 1, out long total_user_job, out string msg_user_job, 100, (is_app_admin || is_app_admin));
                var lst_job_in_user_job = user_job.Select(x => x.id_job);
                string[] fields = new string[] { "id_job", "chuc_danh" };
                List<NhaTuyenDung> result = new List<NhaTuyenDung>();

                var all_job = JobBL.GetAllJobsIsOwner(app_id, user, fields, (is_app_admin || is_sys_admin));

                var lst_id_job_is_owner = all_job.Select(x => x.id_job);

                // viet query get all nha tuyen dung

                var all_nha_tuyen_dung = BL.NhaTuyenDungBL.GetAllNhaTuyenDung(term, new List<int>(), app_id, page, out long total_rec, out msg, 9999, (is_app_admin || is_app_admin)).Where(x => x.nguoi_tao == user || lst_id_job_is_owner.Contains(x.id_job));

                //all_nha_tuyen_dung = NhaTuyenDungBL.Search(term, lst_thuoc_tinh, app_id, user, page, "", out total_recs, out msg, page_size, (is_sys_admin || is_app_admin));

                var lst_id_job = all_nha_tuyen_dung.Select(x => x.id_job);

                var data_job = JobBL.GetMany(lst_id_job).Where(x => x.owner == user || (is_app_admin || is_sys_admin) || lst_job_in_user_job.Contains(x.id_job));
                var list_job = data_job.ToDictionary(x => x.id_job, y => y);

                //.Where(x => data_job.Any(e => e.id_job == x.id_job) && x.nguoi_tao == user || (is_app_admin || is_sys_admin))
                foreach (var item in all_nha_tuyen_dung)
                {
                    JobThuocTinhMap job_map = new JobThuocTinhMap(item, list_job);
                    lst_job_map.Add(job_map);
                }
                return Ok(new DataResponsePaging { data = lst_job_map, total = lst_job_map.Count, success = lst_job_map != null, msg = msg });
            }
        }

        [HttpGet]
        [Route("getnhatuyendungseen")]
        public IActionResult GetNhaTuyenDungDaXem()
        {
            long total_recs = 0;
            string msg = "";
            IList<int> lst = new List<int>();
            lst.Add(-1);
            var lst_ntd_seen = NoteBL.Search(lst, out total_recs, out msg, 50);
            var lst_ntd_get_id = lst_ntd_seen.Select(x => x.id_obj).Distinct();
            var all_nha_tuyen_dung = BL.NhaTuyenDungBL.GetMany(lst_ntd_get_id);
            return Ok(new DataResponsePaging() { data = all_nha_tuyen_dung, success = true, msg = "", total = all_nha_tuyen_dung.Count });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("searchview")]
        public IActionResult Search(string term, string token, int page, int page_size)
        {
            long total_recs = 0;
            string msg = "";
            List<JobThuocTinhMap> lst_job_map = new List<JobThuocTinhMap>();
            var user = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            List<NhaTuyenDung> data = new List<NhaTuyenDung>();
            IList<int> thuoc_tinh = new List<int>();
            data = NhaTuyenDungBL.Search(term, thuoc_tinh, app_id, user, page, token, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin));
            var lst_id_job = data.Select(x => x.id_job);
            var data_job = QLCUNL.BL.JobBL.GetMany(lst_id_job).ToDictionary(x => x.id_job, y => y);
            foreach (var item in data)
            {
                JobThuocTinhMap job_map = new JobThuocTinhMap(item, data_job);
                lst_job_map.Add(job_map);
            }
            return Ok(new { data = lst_job_map, total = total_recs, success = lst_job_map != null, msg = msg });
        }

        [HttpDelete]
        [Route("delete")]
        public bool Delete(string id)
        {
            bool del = NhaTuyenDungBL.Delete(id);
            return del;
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            var obj = JToken.Parse(value.ToString());
            DataResponse res = new DataResponse();
            if (obj == null)
            {
                res.success = false;
                res.msg = "Đối tượng bị bỏ trống";
                return Ok(res);
            }
            try
            {
                List<string> note_uv_gui_nha_tuyen_dung = obj["note_ung_vien_gui_nha_tuyen_dung"].ToObject<List<string>>();
                List<Note> lst = new List<Note>();
                var get_all_ntd = BL.NhaTuyenDungBL.GetById(id);
                var old_id_share = QLCUNL.BL.NhaTuyenDungBL.GetById(id)?.lst_id_share;
                var edit_employer = obj["nha_tuyen_dung"].ToObject<NhaTuyenDung>();
                var note_ung_vien_share = obj["note_ung_vien_share"];
                var new_id_share = note_ung_vien_share["id"].ToObject<List<string>>();

                //Theem quyen
                var added_id_share = new_id_share.Except(old_id_share);
                //xoa quyen
                var remove_id_share = old_id_share.Except(new_id_share);
                if (remove_id_share.Any() == true)
                {
                    PhanQuyenBL.RemovePhanQuyenByUser($"{get_all_ntd.id_nha_tuyen_dung}|{get_all_ntd.token}", remove_id_share.ToList());
                }
                edit_employer.lst_id_share = new_id_share;
                edit_employer.id = id;
                res.success = NhaTuyenDungBL.Update(edit_employer);

                List<string> lst_id_nha_tuyen_dung = new List<string>() { edit_employer.id_nha_tuyen_dung };
                int type = 1;
                int rule = 1;
                int obj_type = note_ung_vien_share["obj_type"].ToObject<int>();
                long ngay_het = 0;

                List<Quyen> quyen = new List<Quyen>();

                ngay_het = XMedia.XUtil.TimeInEpoch(DateTime.Now.AddMonths(1));
                quyen = new List<Quyen>() { Quyen.VIEW };

                var obj_type_check = (PhanQuyenObjType)obj_type;
                var user = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var numbersAndWords = new_id_share.Zip(note_uv_gui_nha_tuyen_dung, (n, w) => new { id_obj = n, noi_dung = w });

                foreach (var notes in numbersAndWords)
                {
                    if (!string.IsNullOrEmpty(notes.noi_dung))
                    {
                        Note note = new Note();
                        note.id_obj = notes.id_obj;
                        note.loai = LoaiNote.NOTE_UNG_VIEN_GUI_NHA_TUYEN_DUNG;
                        note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                        note.nguoi_tao = user;
                        note.nguoi_sua = user;
                        note.noi_dung = notes.noi_dung;
                        SetMetaData(note, false);
                        lst.Add(note);
                    }
                }
                var count = QLCUNL.BL.NoteBL.IndexMany(lst);
                res.success = count > 0;
                if (new_id_share.Count > 0)
                {
                    foreach (var ids in new_id_share)
                    {
                        var has_share_permission = false;
                        switch (obj_type_check)
                        {
                            case PhanQuyenObjType.UNG_VIEN:
                                has_share_permission = UngVienBL.IsOwner(ids, user);
                                break;

                            case PhanQuyenObjType.NOTE_UNG_VIEN_JOB:
                                has_share_permission = NoteUngVienJobBL.IsOwner(ids, user);
                                break;
                        }
                        if (has_share_permission)
                        {
                            #region Xóa shared của các đối tượng cũ

                            try
                            {
                                List<string> lst_id_phan_quyen_can_xoa = new List<string>();
                                var menu_shared = PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty, PhanQuyenObjType.ALL, id,
                                    quyen, 0, 0, user, 0, 0, string.Empty, 0, 0, 1, 9999, out _);
                                foreach (var item_shared in menu_shared)
                                {
                                    if (item_shared.type == PhanQuyenType.USERS)
                                    {
                                        if (!lst_id_nha_tuyen_dung.Contains(item_shared.user))
                                        {
                                            lst_id_phan_quyen_can_xoa.Add(item_shared.id);
                                        }
                                        else
                                        {
                                            if (item_shared.ngay_het == ngay_het && item_shared.quyen.All(quyen.Contains) && quyen.All(item_shared.quyen.Contains))
                                                lst_id_phan_quyen_can_xoa.Remove(item_shared.user);
                                        }
                                    }
                                }
                                PhanQuyenBL.RemoveByListId(lst_id_phan_quyen_can_xoa);
                            }
                            catch (Exception)
                            {
                            }

                            #endregion Xóa shared của các đối tượng cũ

                            #region Shared cho các đối tượng được chọn

                            try
                            {
                                if (lst_id_nha_tuyen_dung.Count == 0)
                                {
                                    res.success = true;
                                    res.msg = "Không có thay đổi nào được thực hiện";
                                }
                                else
                                {
                                    foreach (var u in lst_id_nha_tuyen_dung)
                                    {
                                        PhanQuyen pq = new PhanQuyen();
                                        pq.ngay_het = ngay_het;
                                        pq.user = u + "|" + get_all_ntd.token;
                                        pq.type = PhanQuyenType.USERS;
                                        pq.rule = (PhanQuyenRule)rule;
                                        pq.quyen = quyen;
                                        pq.obj_type = (PhanQuyenObjType)obj_type;
                                        pq.nguoi_tao = user;
                                        pq.obj_id = ids;

                                        res.success = QLCUNL.BL.PhanQuyenBL.Index(pq);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }

                            #endregion Shared cho các đối tượng được chọn
                        }
                    }
                }
                else
                {
                    res.msg = "Không có đối tượng nào được chọn";
                    res.success = false;
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("addlogntd")]
        public IActionResult LogNhaTuyenDung([FromBody] object value)
        {
            // thuộc tính = -1 được định nghĩa là nhà tuyển dụng đã vào xem
            DataResponse res = new DataResponse();
            try
            {
                var note_uv = Newtonsoft.Json.JsonConvert.DeserializeObject<Note>(value.ToString());
                var ip_add = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                string browser = Request.Headers["User-Agent"];
                note_uv.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                note_uv.noi_dung = $"Nhà tuyển dụng {note_uv.nguoi_tao} " +
                    $"đã {note_uv.noi_dung} " +
                    $"vào: {XMedia.XUtil.EpochToTimeString(note_uv.ngay_tao)} " +
                    $"<br> Ip: {ip_add} " +
                    $"Browser: {browser}";
                res.success = NoteBL.Index(note_uv);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }
            return Ok(res);
        }

        [HttpGet]
        [Route("getlog")]
        public IActionResult GetLog(string id_obj)
        {
            long total_recs = 0;
            string msg = "";
            // Số lần nhà tuyển dụng đã truy cập
            var all_log_truy_cap = NoteBL.GetAllLogNhaTuyenDung(id_obj, out total_recs, out msg, 9999);
            var nha_tuyen_dung = NhaTuyenDungBL.GetById(id_obj);
            var note_ung_vien_da_shared = nha_tuyen_dung?.lst_id_share;
            // Nhà tuyển dụng đã xem ứng viên nào
            var all_log_xem_cv_ung_vien = NoteBL.GetAllNhaTuyenDungXemCvUngVien(note_ung_vien_da_shared, nha_tuyen_dung.id_nha_tuyen_dung, out total_recs, out msg, 9999);
            // Note của nhà tuyển dụng cho ứng viên
            var all_note_cua_ntd = NoteBL.NhaTuyenDungNoteUngVien(note_ung_vien_da_shared, nha_tuyen_dung.id_nha_tuyen_dung, out total_recs, out msg, 9999);
            //
            var get_id_note_from_note = all_note_cua_ntd.Select(x => x.id_obj);
            // get list note ứng viên job từ note của ứng viên
            var lst_id_note_ung_vien_job = NoteUngVienJobBL.GetMany(get_id_note_from_note);
            // lấy list id ứng viên từ list note ứng viên job
            var lst_id_ung_vien = lst_id_note_ung_vien_job.Select(x => x.id_ung_vien);
            // lấy được list ứng viên
            var ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
            List<NoteUngVienJobMap> lst = new List<NoteUngVienJobMap>();
            foreach (var item in lst_id_note_ung_vien_job)
            {
                NoteUngVienJobMap map = new NoteUngVienJobMap(item, ung_vien);
                lst.Add(map);
            }

            List<NoteUngVienJobMap> lst_cv_log = new List<NoteUngVienJobMap>();
            foreach (var item in all_log_xem_cv_ung_vien)
            {
                NoteUngVienJobMap map_note_ung_vien = new NoteUngVienJobMap(item, lst);
                lst_cv_log.Add(map_note_ung_vien);
            }

            List<NoteUngVienJobMap> lst_map = new List<NoteUngVienJobMap>();
            foreach (var item in all_note_cua_ntd)
            {
                NoteUngVienJobMap map_note_ung_vien = new NoteUngVienJobMap(item, lst);
                lst_map.Add(map_note_ung_vien);
            }
            return Ok(new { data_log = all_log_truy_cap, data_cv = lst_cv_log, data_note = lst_map, success = true, msg = msg });
        }

        [HttpGet]
        [Route("getungviennoted")]
        public IActionResult GetUngVienDaNote(string id_obj)
        {
            long total_recs = 0;
            string msg = "";
            var nha_tuyen_dung = NhaTuyenDungBL.GetById(id_obj);
            var note_ung_vien_da_shared = nha_tuyen_dung?.lst_id_share;
            var all_note_cua_ntd = NoteBL.NhaTuyenDungNoteUngVien(note_ung_vien_da_shared, nha_tuyen_dung.id_nha_tuyen_dung, out total_recs, out msg, 9999);

            var get_id_note_from_note = all_note_cua_ntd.Select(x => x.id_obj);
            // get list note ứng viên job từ note của ứng viên
            var lst_id_note_ung_vien_job = NoteUngVienJobBL.GetMany(get_id_note_from_note);
            // lấy list id ứng viên từ list note ứng viên job
            var lst_id_ung_vien = lst_id_note_ung_vien_job.Select(x => x.id_ung_vien);
            // lấy được list ứng viên
            var ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
            List<NoteUngVienJobMap> lst_note_map = new List<NoteUngVienJobMap>();
            foreach (var item in lst_id_note_ung_vien_job)
            {
                NoteUngVienJobMap map = new NoteUngVienJobMap(item, ung_vien);
                lst_note_map.Add(map);
            }
            return Ok(new { data = lst_note_map, success = ung_vien.Count > 0, msg = msg });
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet]
        [Route("tomtatungvien")]
        public IActionResult TomTatUngVien(string id)
        {
            long total_recs = 0;
            string msg = "";
            List<string> lst = new List<string>();
            lst.Add(id);
            var tom_tat = NoteBL.GetListNoteByIdObj(lst, out total_recs, out msg).OrderByDescending(x => x.ngay_tao).Take(1);
            if (tom_tat != null)
            {
                return Ok(new DataResponse() { data = tom_tat, success = true, msg = "" });
            }
            else
            {
                return Ok(new DataResponse() { data = null, success = false, msg = "Không tìm thấy kết quả nào!" });
            }
        }
    }
}