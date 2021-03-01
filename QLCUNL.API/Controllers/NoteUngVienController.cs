using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCUNL.API.Models;
using QLCUNL.BL;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteUngVienController : APIBase,IAPIBase
    {
        [HttpPost]
        [Route("get/all")]
        public IActionResult All([FromBody] BaseGetAll res)
        {
            List<NoteUngVien> note_ung_vien = QLCUNL.BL.NoteUngVienBL.GetAll(res.page_index, res.page_size).ToList();
            return Ok(new DataResponse() { data = note_ung_vien, success = note_ung_vien != null, msg = "" });
        }

        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            long total_recs = 0;
            string msg = "";
            var note_uv = QLCUNL.BL.NoteUngVienBL.GetById(id);
            if (!IsInAppId(note_uv)) return BadRequest();
            List<UngVien> uv = QLCUNL.BL.UngVienBL.Search(app_id, 1, out total_recs, out msg, 9999);
            string ten_ung_vien = uv.Find(item => item.id_ung_vien == note_uv.id_ung_vien) != null ? uv.Find(item => item.id_ung_vien == note_uv.id_ung_vien).ho_ten_ung_vien : "";
            note_uv.ten_ung_vien = ten_ung_vien;
            var thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GeSharedtByLoaiGiaTri(app_id, note_uv.thuoc_tinh, LoaiThuocTinh.NOTE_UNG_VIEN).ToDictionary(x => x.id, y => y.ten);
            var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, note_uv.id_note_ung_vien, user);
            List<KeyValuePair<string, string>> lst_thuoc_tinh = thuoc_tinh.ToList();
            if (lst_thuoc_tinh_rieng.Count() > 0)
            {
                var thuoc_tinh_rieng_note = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.JOB, (is_sys_admin || is_app_admin));
            }
            return Ok(new DataResponse() { data = note_uv, success = note_uv != null, msg = "" });
        }
        // POST: api/noteungvien
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_uv = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteUngVien>(value.ToString());
                SetMetaData(note_uv, false);
                res.success = QLCUNL.BL.NoteUngVienBL.Index(note_uv);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/noteungvien/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_uv = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteUngVien>(value.ToString());
                note_uv.id_note_ung_vien = id;
                SetMetaData(note_uv, true);
                res.success = QLCUNL.BL.NoteUngVienBL.Update(note_uv);
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
            bool del = NoteUngVienBL.Delete(id);
            return del;
        }
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, string id_ung_vien, string thuoc_tinh, string thuoc_tinh_rieng, int page,int page_size=10)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<NoteUngVien> data = new List<NoteUngVien>();
            long total_recs = 0;
            string msg = ""; page = page < 1 ? 1 : page;
            List<Models.NoteUngVienUngVienMap> lst = new List<Models.NoteUngVienUngVienMap>();


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
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.NOTE_UNG_VIEN, lst_thuoc_tinh_rieng, page, out long total_recs_thuoc_tinh, out _, 9999);
                    lst_id = lst_id_obj.Select(x => x.id_obj).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0)
            {

            }
            else
            {
                data = QLCUNL.BL.NoteUngVienBL.Search(app_id, user, group, term, id_ung_vien, lst_thuoc_tinh, lst_id, page, out total_recs, out msg, page_size,(is_sys_admin || is_app_admin));
                var lst_id_ung_vien = data.Select(x => x.id_ung_vien);
                var data_ung_vien = QLCUNL.BL.UngVienBL.GetMany(lst_id_ung_vien).ToDictionary(x => x.id_ung_vien, y => y);
                foreach (var item in data)
                {
                    Models.NoteUngVienUngVienMap note = new Models.NoteUngVienUngVienMap(item);
                    var ung_vien = new UngVien();
                    if (data_ung_vien.TryGetValue(note.id_ung_vien, out ung_vien))
                    {
                        note.ho_ten_ung_vien = ung_vien.ho_ten_ung_vien;
                        note.so_dien_thoai = ung_vien.so_dien_thoai;
                        note.email = ung_vien.email;
                        note.zalo = ung_vien.zalo;
                    }
                    else
                    {
                        note.ho_ten_ung_vien = note.id_ung_vien;
                        note.so_dien_thoai = "";
                        note.email = "";
                        note.zalo = "";
                    }
                    lst.Add(note);
                }
            }
            return Ok(new DataResponsePaging() { data = lst, total = total_recs, success = data != null, msg = msg });

        }
        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.NOTE_UNG_VIEN, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }
    }
}