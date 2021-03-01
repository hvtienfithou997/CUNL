using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCUNL.BL;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThuocTinhDuLieuController : APIBase
    {
        [HttpPost]
        [Route("all")]
        public IActionResult All([FromBody] BaseGetAll res)
        {
            List<ThuocTinhDuLieu> ThuocTinhDuLieu = QLCUNL.BL.ThuocTinhDuLieuBL.GetAll(res.page_index, res.page_size).ToList();

            return Ok(new DataResponse() { data = ThuocTinhDuLieu, success = ThuocTinhDuLieu != null, msg = "" });
        }

        //GET: api/ThuocTinhDuLieu
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var ThuocTinhDuLieu = QLCUNL.BL.ThuocTinhDuLieuBL.GetById(id);

            return Ok(new DataResponse() { data = ThuocTinhDuLieu, success = ThuocTinhDuLieu != null, msg = "" });
        }

        //POST: api/ThuocTinhDuLieu
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                res.success = false;
                string json = value.ToString();

                var obj_tk = Newtonsoft.Json.Linq.JToken.Parse(json);
                if (obj_tk != null)
                {
                    if (obj_tk["thuoc_tinh_rieng"] != null)
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ThuocTinhDuLieu>(json);
                        data.thuoc_tinh = obj_tk["thuoc_tinh_rieng"].ToObject<List<int>>();
                        if (data.thuoc_tinh.Count > 0)
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
                    if (obj_tk["thuoc_tinh"] != null)
                    {
                        string id = obj_tk["id_obj"].ToString();
                        string loai_obj = obj_tk["loai_obj"].ToString();
                        var thuoc_tinh = obj_tk["thuoc_tinh"].ToObject<List<int>>();
                        
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
                }


            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }
        
        // PUT: api/ThuocTinhDuLieu/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ThuocTinhDuLieu>(value.ToString());
                data.id = id;
                SetMetaData(data, true);
                res.success = QLCUNL.BL.ThuocTinhDuLieuBL.Update(data);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, int page)
        {
            List<ThuocTinhDuLieu> ThuocTinhDuLieu = QLCUNL.BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.CONG_TY, null, 1, out _, out _).ToList();

            return Ok(new DataResponse() { data = ThuocTinhDuLieu, success = ThuocTinhDuLieu != null, msg = "" });
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete()]
        [Route("delete")]
        public bool Delete(string id)
        {
            return false;
        }
    }
}