﻿using System;
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
    public class NoteController : APIBase
    {
        [HttpPost]
        [Route("get/all")]
        public IActionResult All([FromBody] BaseGetAll res)
        {
            List<NoteUngVien> note_ung_vien = QLCUNL.BL.NoteBL.GetAll(res.page_index, res.page_size).ToList();

            return Ok(new DataResponse() { data = note_ung_vien, success = note_ung_vien != null, msg = "" });
        }

        [HttpGet]
        [Route("view")]
        //public IActionResult Get(string id)
        //{
        //    long total_recs = 0;
        //    string msg = "";
        //    var note_uv = QLCUNL.BL.NoteBL.GetById(id);
        //    if (!IsInAppId(note_uv)) return BadRequest();
        //    List<UngVien> uv = QLCUNL.BL.UngVienBL.Search(app_id, 1, out total_recs, out msg, 9999);
        //    string ten_ung_vien = uv.Find(item => item.id_ung_vien == note_uv.id_ung_vien) != null ? uv.Find(item => item.id_ung_vien == note_uv.id_ung_vien).ho_ten_ung_vien : "";
        //    note_uv.ten_ung_vien = ten_ung_vien;
        //    var thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GeSharedtByLoaiGiaTri(app_id, note_uv.thuoc_tinh, LoaiThuocTinh.NOTE_UNG_VIEN).ToDictionary(x => x.id, y => y.ten);
        //    var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, note_uv.id_note_ung_vien, user);
        //    List<KeyValuePair<string, string>> lst_thuoc_tinh = thuoc_tinh.ToList();
        //    if (lst_thuoc_tinh_rieng.Count() > 0)
        //    {
        //        var thuoc_tinh_rieng_note = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.JOB, (is_sys_admin || is_app_admin));
        //    }
        //    return Ok(new DataResponse() { data = note_uv, success = note_uv != null, msg = "" });
        //}

        // POST: api/noteungvien
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var note_uv = Newtonsoft.Json.JsonConvert.DeserializeObject<Note>(value.ToString());
                
                //SetMetaData(note_uv, false);
                res.success = QLCUNL.BL.NoteBL.Index(note_uv);
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
                var note_uv = Newtonsoft.Json.JsonConvert.DeserializeObject<Note>(value.ToString());
                note_uv.id = id;
                SetMetaData(note_uv, true);
                res.success = QLCUNL.BL.NoteBL.Update(note_uv);
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
            bool del = NoteBL.Delete(id);
            return del;
        }
      
        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.NOTE_UNG_VIEN, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }

        [HttpGet]
        [Route("getnotebyobject")]
        public IActionResult GetNoteByObject(string id_obj)
        {
            long total_recs = 0;
            string msg = "";
            List<string> lst_id = new List<string>();
            lst_id.Add(id_obj);
            var note_ung_vien = NoteBL.GetNoteByObject(app_id, lst_id,LoaiNote.UNG_VIEN, LoaiDuLieu.NGUOI_DUNG, user, out total_recs, out msg);
            
            return Ok(new DataResponse() { data = note_ung_vien, success = true, msg = "" });
        }
    }
}