using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLCUNL.API.Models;
using QLCUNL.BL;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongTyController : APIBase, IAPIBase
    {
        // GET : api/CongTy/all
        [HttpPost]
        [Route("all")]
        public IActionResult All([FromBody] BaseGetAll req)
        {
            List<CongTy> congtys = QLCUNL.BL.CongTyBL.GetAll(app_id, req.term, req.page_index, req.page_size).ToList();
            return Ok(new DataResponse() { data = congtys, success = congtys != null, msg = req.page_size.ToString() });
        }

        // GET: ?id=abc123
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var cong_ty = QLCUNL.BL.CongTyBL.GetById(id);
            if (!IsInAppId(cong_ty)) return BadRequest();
            var lst_id_thuoc_tinh = cong_ty.thuoc_tinh != null ? cong_ty.thuoc_tinh : new List<int>();
            var thuoc_tinh_chung = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.CONG_TY, ThuocTinhType.SHARED);

            var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, cong_ty.id_cong_ty, user);
            if (lst_thuoc_tinh_rieng.Count() > 0)
            {
                var thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.CONG_TY, (is_sys_admin || is_app_admin));
                thuoc_tinh_chung.AddRange(thuoc_tinh_rieng);
            }
            Models.CongTyMap ct_map = new Models.CongTyMap(cong_ty, thuoc_tinh_chung);
            return Ok(new DataResponse() { data = ct_map, success = cong_ty != null, msg = "" });
        }

        // POST: api/CongTy
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var cong_ty = JsonConvert.DeserializeObject<CongTy>(value.ToString());
                var obj = JToken.Parse(value.ToString());
                SetMetaData(cong_ty, false);
                if (string.IsNullOrEmpty(cong_ty.ten_cong_ty))
                {
                    res.success = false; res.msg = "Chưa nhập tên công ty";
                }
                else
                {
                    if (!CongTyBL.IsExistTenCongTy(app_id, cong_ty.ten_cong_ty))
                    {
                        string id_cong_ty = CongTyBL.Index(cong_ty);
                        res.success = !string.IsNullOrEmpty(id_cong_ty);
                        UpsertThuocTinhRieng(value, id_cong_ty, LoaiThuocTinh.CONG_TY);

                        if (!string.IsNullOrEmpty(obj["ghi_chu"]?.ToString()))
                        {
                            var note = new Note
                            {
                                noi_dung = obj["ghi_chu"].ToString(),
                                id_obj = id_cong_ty,
                                loai = LoaiNote.CONG_TY,
                                loai_du_lieu = LoaiDuLieu.NGUOI_DUNG,
                                thuoc_tinh = new List<int>()
                            };
                            SetMetaData(note, false);
                            res.success = NoteBL.Index(note);
                        }
                    }
                    else
                    {
                        res.success = false; res.msg = "Tên công ty này đã tồn tại";
                    }
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        [HttpPost]
        [Route("indexcongty")]
        public IActionResult IndexCongTy([FromBody] object value, string id)
        {
            DataResponse res = new DataResponse();
            try
            {
                string id_cong_ty = "";
                var cong_ty = Newtonsoft.Json.JsonConvert.DeserializeObject<CongTy>(value.ToString());
                cong_ty.id_cong_ty = id_cong_ty;
                cong_ty.id_cong_ty = id;
                SetMetaData(cong_ty, false);
                if (string.IsNullOrEmpty(cong_ty.ten_cong_ty))
                {
                    res.success = false; res.msg = "Chưa nhập tên công ty";
                }
                else
                {
                    var cong_ty_tmp = BL.CongTyBL.GetCongTyByName(app_id, cong_ty.ten_cong_ty);
                    if (cong_ty_tmp != null)
                    {
                        res.success = true; res.data = cong_ty_tmp.id_cong_ty;
                    }
                    else
                    {
                        res.success = QLCUNL.BL.CongTyBL.IndexCongTy(cong_ty, out id_cong_ty);
                        res.data = id_cong_ty;
                    }
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/CongTy/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var congty = Newtonsoft.Json.JsonConvert.DeserializeObject<CongTy>(value.ToString());
                congty.id_cong_ty = id;
                SetMetaData(congty, true);

                res.success = QLCUNL.BL.CongTyBL.Update(congty);
                UpsertThuocTinhRieng(value, id, LoaiThuocTinh.CONG_TY);
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
            bool del = CongTyBL.Delete(id);
            return del;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search(string term, string thuoc_tinh, string thuoc_tinh_rieng, int page, int page_size)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<CongTy> data = new List<CongTy>();
            long total_recs = 0;
            string msg = "";
            if (!string.IsNullOrEmpty(thuoc_tinh))
            {
                foreach (var tt in thuoc_tinh.Split(','))
                {
                    int t = -1;
                    if (int.TryParse(tt, out t))
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
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.CONG_TY, lst_thuoc_tinh_rieng, page, out long total_recs_thuoc_tinh, out _, 9999);
                    lst_id = lst_id_obj.Select(x => x.id_obj).Where(o => !string.IsNullOrEmpty(o)).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0 && lst_thuoc_tinh.Count == 0)
            {
            }
            else
            {
                var lst_id_job_duoc_giao = await UserJobBL.GetListIdJobByUser(app_id, new List<string>() { user });
                var lst_id_cong_ty_phan_theo_job = await JobBL.GetListIdCongTyByIdJob(app_id, lst_id_job_duoc_giao);
                

                data = QLCUNL.BL.CongTyBL.Search(app_id, user, group, term, lst_thuoc_tinh, lst_id, lst_id_cong_ty_phan_theo_job, page, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin));
            }

            return Ok(new DataResponsePaging() { data = data, total = total_recs, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.CONG_TY, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }

        [HttpGet]
        [Route("thongkethuoctinh")]
        public async Task<IActionResult> ThongKeThuocTinh(string term, string thuoc_tinh, string thuoc_tinh_rieng, int page, int page_size)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            Tuple<List<CongTy>, Dictionary<int, long>> data = null;
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
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.CONG_TY, lst_thuoc_tinh_rieng, page, out long total_recs_thuoc_tinh, out _, 9999);
                    lst_id = lst_id_obj.Select(x => x.id_obj).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0 && lst_thuoc_tinh.Count == 0)
            {
            }
            else
            {
                var lst_id_job_duoc_giao = await UserJobBL.GetListIdJobByUser(app_id, new List<string>() { user });
                var lst_id_cong_ty_phan_theo_job = await JobBL.GetListIdCongTyByIdJob(app_id, lst_id_job_duoc_giao);
                
                data = QLCUNL.BL.CongTyBL.Search(app_id, user, group, term, lst_thuoc_tinh, lst_id, lst_id_cong_ty_phan_theo_job, 1, out total_recs, out msg, 0, (is_sys_admin || is_app_admin), true);
            }

            return Ok(new DataResponse() { data = data.Item2.Select(x => new { k = x.Key, v = x.Value }), success = data != null, msg = msg });
        }

        [HttpPost]
        [Route("themlienhe")]
        public IActionResult IndexManyLienHe([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            List<CongTy> lst = new List<CongTy>();
            //lst.Add(new CongTy() { ten_cong_ty = "#Không rõ", lien_he = new List<NguoiLienHe>() { new NguoiLienHe() { chuc_vu = "oong A" } } });
            //lst.Add(new CongTy() { ten_cong_ty = "#Không rõ", lien_he = new List<NguoiLienHe>() { new NguoiLienHe() { chuc_vu = "oong B" } } });
            try
            {
                var list_cong_ty = JsonConvert.DeserializeObject<List<CongTy>>(value.ToString());

                foreach (var n in list_cong_ty)
                {
                    foreach (var item in n.lien_he)
                    {
                        if (!string.IsNullOrEmpty(item.chuc_vu))
                        {
                            n.thuoc_tinh = new List<int>() { -1 };
                            SetMetaData(n, false);
                            lst.Add(n);
                        }
                    }
                }
                var count = CongTyBL.IndexMany(lst);
                res.success = count > 0;
            }
            catch (Exception ex)
            {
                res.msg = ex.Message;
                res.success = false;
            }
            return Ok(res);
        }

        [HttpPost]
        [Route("logsendmail")]
        public IActionResult LogSendMail([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            if (value != null)
            {
                var list_log = JsonConvert.DeserializeObject<List<LogSendMail>>(value.ToString());
                foreach (var item in list_log)
                {
                    SetMetaData(item, false);
                }
                var count = LogSendMailBL.IndexMany(list_log);
                res.success = count > 0;
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("getallmail")]
        public IActionResult AllMail(string status)
        {
            long total_recs = 0;
            string msg = "";
            var all_mail = LogSendMailBL.Search(app_id, status, 1, out total_recs, out msg, 9999);
            if (all_mail.Count > 0)
            {
                return Ok(new DataResponsePaging() { data = all_mail, total = total_recs, msg = "", success = all_mail.Count > 0 });
            }
            else
            {
                return Ok(new DataResponse() { data = null, msg = "Bạn chưa có email nào", success = false });
            }
        }

        [HttpGet]
        [Route("updatestatusmail")]
        public IActionResult UpdateStatusMail()
        {
            
            var all_log_mail_new = LogSendMailBL.TimMailMoi(app_id);
            var id_mail = all_log_mail_new.Select(x => x.Id);
            return Ok(new DataResponse() { data = id_mail, success = all_log_mail_new.Count > 0, msg = "" });        
        }

        [HttpPut]
        [Route("updatelog")]
        public IActionResult UpdateLog([FromBody] List<LogSendMail> value)
        {
            DataResponse res = new DataResponse();
            try
            {
                //var list_cong_ty = JsonConvert.DeserializeObject<List<LogSendMail>>(value.ToString());
                //var log = JsonConvert.DeserializeObject<LogSendMail>(value.ToString());
                if(value.Count > 0)
                {
                    foreach (var item in value)
                    {
                        SetMetaData(item, true);
                        res.success = QLCUNL.BL.LogSendMailBL.UpdateStatus(item.Id, item.Status);
                    }
                }
                
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }
    }
}