using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QLCUNL.API.Models;
using QLCUNL.BL;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Job = QLCUNL.Models.Job;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserJobController : APIBase, IAPIBase
    {
        // GET: api/UserJob/5
        [HttpGet()]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var user_job = QLCUNL.BL.UserJobBL.GetById(id);
            var job_user = BL.JobBL.GetMany(app_id, new List<string>() { user_job.id_job }, new string[] { "chuc_danh", "so_luong", "cong_ty" }).ToDictionary(x => x.id_job, y => y);

            Models.UserJobMap data = new Models.UserJobMap();
            data.id_user_job = user_job.id_user_job;
            data.id_job = user_job.id_job;
            data.ngay_nhan_job = user_job.ngay_nhan_job;

            var lst_id_thuoc_tinh = user_job.thuoc_tinh != null ? user_job.thuoc_tinh : new List<int>();
            var data_thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.USER_JOB, ThuocTinhType.SHARED).ToDictionary(x => x.gia_tri, y => y);

            data.thuoc_tinh = user_job.thuoc_tinh;
            data.thuoc_tinh = user_job.thuoc_tinh != null ? user_job.thuoc_tinh.Select(x => new
            {
                gia_tri = x,
                ten = data_thuoc_tinh.ContainsKey(x) ? data_thuoc_tinh[x].ten : x.ToString(),
                nhom = data_thuoc_tinh.ContainsKey(x) ? data_thuoc_tinh[x].nhom : 0
            }).ToArray() : new dynamic[] { };
            var cong_ty = new CongTy();
            var jd = new Job();
            if (job_user.TryGetValue(user_job.id_job, out jd))
            {
                data.so_luong = jd.so_luong;
                data.chuc_danh = jd.chuc_danh;
                cong_ty = jd.cong_ty != null ? CongTyBL.GetById(jd.cong_ty.id_cong_ty) : new CongTy();
                jd.cong_ty = cong_ty;
                data.cong_ty = cong_ty.ten_cong_ty;
            }
            return Ok(new { data = jd, extra = new {  user_job.id_user, data.ngay_nhan_job, data.thuoc_tinh, user_job.nguoi_tao }, success = user_job != null, msg = "" });
        }

        [HttpGet()]
        [Route("viewbyjob")]
        public IActionResult GetByJob(string id)
        {
            var data = QLCUNL.BL.UserJobBL.GetByIdJob(app_id, id);
            return Ok(new DataResponse() { data = data, success = data != null, msg = "" });
        }

        [HttpGet()]
        [Route("userjobbyidjob")]
        public IActionResult GetUserJobByIdJob(string id)
        {

            var data = QLCUNL.BL.UserJobBL.GetUserJobByIdJob(app_id, id, user);
            return Ok(new DataResponse() { data = data, success = data != null, msg = "" });
        }

        // POST: api/UserJob
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var userjob = Newtonsoft.Json.JsonConvert.DeserializeObject<UserJob>(value.ToString());
                SetMetaData(userjob, false);
                res.success = QLCUNL.BL.UserJobBL.Index(userjob);
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
                
                if (obj != null)
                {
                    var users = obj["users"].ToObject<List<string>>();

                    var id_job = obj["id"].ToString();
                    
                    // cũ
                    var data = QLCUNL.BL.UserJobBL.GetByIdJob(app_id, id_job);

                    var lst_id_user = data.Select(x => x.id_user).ToList();

                    var lst_id_user_job = data.Select(o => o.id_user_job);

                    // list id cần xóa
                    var lst_user_need_delete = lst_id_user.Except(users);

                    var lst_can_xoa = data.Where(x => lst_user_need_delete.Contains(x.id_user)).Select(x => x.id_user_job);
                    

                    var lst_id_ung_vien = NoteUngVienJobBL.GetListIdUngVienByIdUserJobNew(app_id, lst_can_xoa).Select(x => x.Key);
                    
                    var lst_phai_xoa = lst_can_xoa.Except(lst_id_ung_vien);
                    if(lst_phai_xoa.Any() == true)
                    {
                        BL.UserJobBL.DeleteByIdUserJob(lst_phai_xoa);
                    }                    
                    //Check quyền xem User hiện tại có phải owner của Job hoặc admin hay không. Chỉ cho phép chủ sở hữu hoặc admin gán

                    if (JobBL.IsOwner(id_job, user) || is_app_admin)
                    {
                        
                        List<UserJob> lst = new List<UserJob>();
                        foreach (var id_user in users)
                        {
                            var user_job = new UserJob();
                            user_job.id_job = id_job;
                            user_job.ngay_nhan_job = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                            user_job.id_user = id_user;
                            user_job.nguoi_sua = user;
                            user_job.nguoi_tao = user;
                            SetMetaData(user_job, false);
                            lst.Add(user_job);
                        }

                        int count_success = QLCUNL.BL.UserJobBL.IndexMany(lst);
                        res.success = count_success > 0;
                        res.msg = $"Đã gán JOB cho: {string.Join(" ", users)}";
                    }
                    else
                    {
                        res.success = false; res.msg = "Bạn không có quyền trên JOB này";
                    }
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        [HttpPut()]
        [Route("update/{id}")]
        public IActionResult UpdateThuocTinh(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var userjob = Newtonsoft.Json.JsonConvert.DeserializeObject<UserJob>(value.ToString());
                userjob.id_user_job = id;
                SetMetaData(userjob, true);
                res.success = QLCUNL.BL.UserJobBL.UpdateThuocTinh(userjob);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/UserJob/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var userjob = Newtonsoft.Json.JsonConvert.DeserializeObject<UserJob>(value.ToString());
                userjob.id_user_job = id;
                SetMetaData(userjob, true);
                res.success = QLCUNL.BL.UserJobBL.Update(userjob);
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
            bool del = UserJobBL.Delete(id);
            return del;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, string id_job, string id_ung_vien, long ngay_nhan_job_from, long ngay_nhan_job_to, string thuoc_tinh, string thuoc_tinh_rieng,
            int page, int page_size)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            List<string> lst_id_job = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<UserJob> data = new List<UserJob>();
            List<string> lst_id_job_filter = new List<string>();
            List<Models.UserJobMap> lst = new List<Models.UserJobMap>();
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
                    lst_id = lst_id_obj.Select(x => x.id_obj).ToList();
                }
            }
            if (!string.IsNullOrEmpty(term))
            {
                var data_job_by_term = QLCUNL.BL.JobBL.Search(app_id, user, group, term, string.Empty, string.Empty, 0, 0, 0, 0, new List<int>(), new List<string>(), new List<string>(), 0, 0, 1, out _, out _, 200, (is_sys_admin || is_app_admin), new string[] { "id_job" });
                lst_id_job = data_job_by_term.Select(x => x.id_job).ToList();
            }
            if (!string.IsNullOrEmpty(id_job))
                lst_id_job.Add(id_job);
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0)
            {
            }
            else
                data = QLCUNL.BL.UserJobBL.Search(app_id, user, group, lst_id_job, id_ung_vien, ngay_nhan_job_from, ngay_nhan_job_to, lst_thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin));

            if (data.Count > 0)
            {
                var lst_id_thuoc_tinh = data.Where(x => x.thuoc_tinh != null).SelectMany(x => x.thuoc_tinh);
                var data_thuoc_tinh = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.USER_JOB, ThuocTinhType.SHARED).ToDictionary(x => x.gia_tri, y => y);

                var job_user = BL.JobBL.GetMany(app_id, data.Select(x => x.id_job), new string[] { "chuc_danh", "so_luong", "cong_ty" }).ToDictionary(x => x.id_job, y => y);

                var cong_ty = BL.CongTyBL.GetMany(job_user.Where(x => x.Value.cong_ty != null).Select(x => x.Value.cong_ty.id_cong_ty).Distinct()).ToDictionary(x => x.id_cong_ty, y => y.ten_cong_ty);

                foreach (var item in data)
                {
                    Models.UserJobMap m = new Models.UserJobMap();

                    m.id_user_job = item.id_user_job;
                    m.id_job = item.id_job;
                    m.ngay_nhan_job = item.ngay_nhan_job;
                    m.thuoc_tinh = item.thuoc_tinh != null ? item.thuoc_tinh.Select(x => new
                    {
                        gia_tri = x,
                        ten = data_thuoc_tinh.ContainsKey(x) ? data_thuoc_tinh[x].ten : x.ToString(),
                        nhom = data_thuoc_tinh.ContainsKey(x) ? data_thuoc_tinh[x].nhom : 0
                    }).ToArray() : new dynamic[] { };
                    var jd = new Job();
                    if (job_user.TryGetValue(item.id_job, out jd))
                    {
                        m.so_luong = jd.so_luong;
                        m.chuc_danh = jd.chuc_danh;
                        if (jd.cong_ty != null)
                        {
                            if (cong_ty.TryGetValue(jd.cong_ty.id_cong_ty, out string ten_cong_ty))
                            {
                                m.cong_ty = ten_cong_ty;
                            }
                        }
                        m.auto_id = jd.auto_id;
                    }
                    lst.Add(m);
                }
            }
            return Ok(new DataResponsePaging() { data = lst, total = total_recs, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("myjob")]
        public IActionResult MyJob(string id_job, string id_ung_vien, long ngay_nhan_job_from, long ngay_nhan_job_to, string thuoc_tinh, string thuoc_tinh_rieng,
            int page, int page_size)
        {
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<UserJob> data = new List<UserJob>();
            List<Models.UserJobMap> lst = new List<Models.UserJobMap>();
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
                    lst_id = lst_id_obj.Select(x => x.id_obj).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0)
            {
            }
            else
                data = QLCUNL.BL.UserJobBL.Search(app_id, user, group, new List<string>() { id_job }, id_ung_vien, ngay_nhan_job_from, ngay_nhan_job_to, lst_thuoc_tinh, lst_id, page, out total_recs, out msg, page_size);
            if (data.Count > 0)
            {
                var job_user = BL.JobBL.GetMany(app_id, data.Select(x => x.id_job), new string[] { "chuc_danh", "so_luong", "cong_ty" }).ToDictionary(x => x.id_job, y => y);

                foreach (var item in data)
                {
                    Models.UserJobMap m = new Models.UserJobMap();
                    m.id_user_job = item.id_user_job;
                    m.id_job = item.id_job;
                    m.ngay_nhan_job = item.ngay_nhan_job;
                    m.thuoc_tinh = item.thuoc_tinh;
                    var jd = new Job();
                    if (job_user.TryGetValue(item.id_job, out jd))
                    {
                        m.so_luong = jd.so_luong;
                        m.chuc_danh = jd.chuc_danh;
                        m.cong_ty = jd.cong_ty.ten_cong_ty;
                        m.auto_id = jd.auto_id;
                    }
                    lst.Add(m);
                }
            }
            return Ok(new DataResponsePaging() { data = lst, total = total_recs, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.USER_JOB, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }

    }
}