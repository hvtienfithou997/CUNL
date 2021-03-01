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
    public class JobController : APIBase, IAPIBase
    {
        // GET: api/Job
        [HttpGet]
        [Route("viewdetail")]
        public IActionResult ViewDetail(string id)
        {
            var job = QLCUNL.BL.JobBL.GetById(id);
            if (!IsInAppId(job)) return BadRequest();

            var all_user_job = QLCUNL.BL.UserJobBL.GetByIdJob(app_id, id);
            var thuoc_tinh_job = QLCUNL.BL.ThuocTinhBL.GeSharedtByLoaiGiaTri(app_id, job.thuoc_tinh, LoaiThuocTinh.JOB);
            var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, job.id_job, user);
            if (lst_thuoc_tinh_rieng.Count() > 0)
            {
                var thuoc_tinh_rieng_job = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.JOB, (is_sys_admin || is_app_admin));
                thuoc_tinh_job.AddRange(thuoc_tinh_rieng_job);
            }
            JobThuocTinhMap jm = new JobThuocTinhMap(job, thuoc_tinh_job);
            if (job.cong_ty != null)
            {
                var cong_ty = QLCUNL.BL.CongTyBL.GetMany(new List<string>() { job.cong_ty.id_cong_ty });
                if (cong_ty != null && cong_ty.Count > 0)
                {
                    jm.cong_ty = cong_ty.First();
                }
            }
            jm.is_owner = user == jm.owner;
            return Ok(new { data = jm, extra = all_user_job.Select(x => new { id_user = x.id_user, ngay_nhan_job = x.ngay_nhan_job }), success = job != null, msg = "" });
        }

        // GET: api/Job/5
        [HttpGet()]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var job = QLCUNL.BL.JobBL.GetById(id);
            if (!IsInAppId(job)) return BadRequest();
            var thuoc_tinh_job = QLCUNL.BL.ThuocTinhBL.GeSharedtByLoaiGiaTri(app_id, job.thuoc_tinh, LoaiThuocTinh.JOB);

            var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, job.id_job, user);

            if (lst_thuoc_tinh_rieng.Count() > 0)
            {
                var thuoc_tinh_rieng_job = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.JOB, (is_sys_admin || is_app_admin));
                thuoc_tinh_job.AddRange(thuoc_tinh_rieng_job);
            }
            //
            long total_recs = 0;
            string msg = "";
            List<string> lst = new List<string>();
            lst.Add(id);
            var all_note = NoteBL.GetNoteByObject(app_id, lst, LoaiNote.JOB, LoaiDuLieu.NGUOI_DUNG, user, out total_recs, out msg);
            if (all_note != null)
            {
                foreach (var note in all_note.OrderByDescending(x => x.ngay_tao).Take(1))
                {
                    job.ghi_chu = note.noi_dung;
                }
            }
            JobThuocTinhMap jm = new JobThuocTinhMap(job, thuoc_tinh_job, id);
            if (job.cong_ty != null)
            {
                var cong_ty = QLCUNL.BL.CongTyBL.GetMany(new List<string>() { job.cong_ty.id_cong_ty });
                if (cong_ty != null && cong_ty.Count > 0)
                {
                    jm.cong_ty = cong_ty.First();
                }
            }
            jm.is_owner = user == jm.owner;
            return Ok(new { data = jm, success = job != null, msg = "", all_note = all_note });
        }

        // POST: api/Job
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var job = Newtonsoft.Json.JsonConvert.DeserializeObject<Job>(value.ToString());
                var obj = JToken.Parse(value.ToString());
                SetMetaData(job, false);
                string id_job = QLCUNL.BL.JobBL.Index(job, out string auto_id);
                res.success = !string.IsNullOrEmpty(id_job);
                res.data = auto_id;
                if (!string.IsNullOrEmpty(obj["ghi_chu"].ToString()))
                {
                    var note = new Note();
                    note.noi_dung = obj["ghi_chu"].ToString();
                    note.id_obj = id_job;
                    note.loai = LoaiNote.JOB;
                    note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                    note.thuoc_tinh = new List<int>();
                    SetMetaData(note, false);
                    res.success = NoteBL.Index(note);
                }
                //Rename file job.link_job = job.chuc_danh + job.auto_id. job.link_job_upload="";
                UpsertThuocTinhRieng(value, id_job, LoaiThuocTinh.JOB);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }
            return Ok(res);
        }

        // PUT: api/Job/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var job = Newtonsoft.Json.JsonConvert.DeserializeObject<Job>(value.ToString());
                var obj = JToken.Parse(value.ToString());
                job.id_job = id;
                SetMetaData(job, true);
                if (!(is_sys_admin || is_app_admin))
                {
                    job.owner = null;
                }
                if (!string.IsNullOrEmpty(obj["ghi_chu"].ToString()))
                {
                    var note = new Note();
                    note.noi_dung = obj["ghi_chu"].ToString();
                    note.id_obj = id;
                    note.loai = LoaiNote.JOB;
                    note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                    note.thuoc_tinh = new List<int>();
                    SetMetaData(note, false);
                    res.success = NoteBL.Index(note);
                }
                res.success = QLCUNL.BL.JobBL.Update(job);
                //Nếu update thành công và link_job != Chức danh + auto_id thì nghĩa là mới upload link_job => cần rename lại file link_job
                UpsertThuocTinhRieng(value, id, LoaiThuocTinh.JOB);
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
            if (JobBL.IsOwner(id, user) || (is_sys_admin || is_app_admin))
            {
                bool del = JobBL.DelUp(id);
                return del;
            }
            return false;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string value_filter, long ngay_nhan_hd, long ngay_tao, string term, string id_ung_vien,
            string id_cong_ty, long ngay_di_lam_from, long ngay_di_lam_to, double don_gia_from, double don_gia_to, string thuoc_tinh,
            string thuoc_tinh_rieng, string op, string thuoc_tinh2, string thuoc_tinh_rieng2, string op2, long ngay_tao_from, long ngay_tao_to, int page, int page_size,
           string field_sort = "", string sort = "")
        {
            Dictionary<string, bool> sort_order = new Dictionary<string, bool>();

            if (!string.IsNullOrEmpty(field_sort) && !string.IsNullOrEmpty(sort))
            {
                sort_order.Add(field_sort, sort == "0" ? true : false);
            }
            else
            {
                sort_order.Add("id_auto", true);
            }
            var default_settings = QLCUNL.BL.UserBL.GetDefaultSettingByAppId(app_id);
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();

            List<int> lst_thuoc_tinh2 = new List<int>();
            List<int> lst_thuoc_tinh_rieng2 = new List<int>();

            List<string> lst_id = new List<string>();
            List<string> lst_id_should = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<Job> data_job = new List<Job>();
            Dictionary<string, UserJob> data_user_job = new Dictionary<string, UserJob>();
            Dictionary<string, Dictionary<int, long>> dic_thong_ke_trang_thai_ung_vien_theo_job = new Dictionary<string, Dictionary<int, long>>();
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
            if (!string.IsNullOrEmpty(thuoc_tinh_rieng) || !string.IsNullOrEmpty(thuoc_tinh_rieng2))
            {
                if (string.IsNullOrEmpty(thuoc_tinh_rieng))
                    thuoc_tinh_rieng = "";
                foreach (var tt in thuoc_tinh_rieng.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh_rieng.Add(t);
                    }
                }
                if (string.IsNullOrEmpty(thuoc_tinh_rieng2))
                    thuoc_tinh_rieng2 = "";
                foreach (var tt in thuoc_tinh_rieng2.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh_rieng2.Add(t);
                    }
                }

                is_find_thuoc_tinh_rieng = true;
                var lst_id_obj = ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.JOB, lst_thuoc_tinh_rieng, 1, out long total_recs_thuoc_tinh, out _, 9999, op, lst_thuoc_tinh_rieng2, op2);

                if (lst_id_obj.Count > 0)
                {
                    if (op == "0")
                    {
                        lst_id_should = lst_id_obj.Select(x => x.id_obj).Distinct().ToList();
                    }
                    else
                        lst_id = lst_id_obj.Select(x => x.id_obj).Distinct().ToList();
                }
                else
                {
                    lst_id = new List<string>() { "__NULL__" };
                }
            }

            if (!string.IsNullOrEmpty(thuoc_tinh2))
            {
                foreach (var tt in thuoc_tinh2.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh2.Add(t);
                    }
                }
            }

            ///lấy tất cả các UserJob đã được gán cho người dùng này, trộn cùng các Job họ tạo ra (hoặc của team mình) order lại theo ngày nhận Job và ngày Tạo Job
            var user_job = UserJobBL.Search(app_id, user, group, new List<string>(), string.Empty, 0, 0, new List<int>(), new List<string>(), 1, out long total_user_job, out string msg_user_job, 100, (is_app_admin || is_app_admin));
            List<string> lst_id_job = new List<string>();
            if (user_job.Count > 0)
            {
                lst_id_job = user_job.Select(x => x.id_job).Distinct().ToList();

                foreach (var item in user_job)
                {
                    if (!data_user_job.ContainsKey(item.id_job))
                        data_user_job.Add(item.id_job, item);
                }
                lst_id_should.AddRange(lst_id_job);
            }
            var company = string.Empty;
            if (!string.IsNullOrEmpty(id_cong_ty))
            {
                company = CongTyBL.GetCongTyByNameV2(app_id, id_cong_ty)?.id_cong_ty;               
            }

            data_job = JobBL.SearchDefault(app_id, user, group, value_filter, ngay_nhan_hd, ngay_tao, term, id_ung_vien, company, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, lst_thuoc_tinh, lst_thuoc_tinh2, lst_id, lst_id_should,
                 ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin || is_super_user), null, op, op2, sort_order);

            dic_thong_ke_trang_thai_ung_vien_theo_job = NoteUngVienJobBL.ThongKeTrangThaiUngVien(app_id, user, default_settings.trang_thai_thong_ke_ung_vien_job,
                data_job.Select(x => x.id_job), data_job.Where(x => x.owner == user).Select(x => x.id_job),
                (is_app_admin | is_super_user | is_sys_admin));

            //
            var lst_id_thuoc_tinh = data_job.Where(x => x.thuoc_tinh != null).SelectMany(x => x.thuoc_tinh).ToList();

            List<JobThuocTinhMap> lst_job_map = new List<JobThuocTinhMap>();
            var dic_thuoc_tinh_rieng = ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, data_job.Select(x => x.id_job), user);
            if (dic_thuoc_tinh_rieng.Count > 0)
            {
                foreach (var tt_r in dic_thuoc_tinh_rieng)
                {
                    lst_id_thuoc_tinh.AddRange(tt_r.Value);
                }
            }

            var data_thuoc_tinh = ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.JOB, -1);
            //
            var all_thuoc_tinh_job = ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, (int)LoaiThuocTinh.JOB, -1);
            var all_thuoc_tinh_note_ung_vien_job = ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, (int)LoaiThuocTinh.NOTE_UNG_VIEN_JOB, 0);
            var lst_id_cong_ty = data_job.Where(x => x.cong_ty != null && !string.IsNullOrEmpty(x.cong_ty.id_cong_ty)).Select(x => x.cong_ty.id_cong_ty);
            var lst_cong_ty = QLCUNL.BL.CongTyBL.GetMany(lst_id_cong_ty);

            Dictionary<string, string> dic_cong_ty = new Dictionary<string, string>();
            Dictionary<string, long> dic_count_user_job_theo_job = UserJobBL.ThongKeUserJob(app_id, data_job.Select(x => x.id_job));
            if (lst_cong_ty != null)
            {
                dic_cong_ty = lst_cong_ty.ToDictionary(x => x.id_cong_ty, y => y.ten_cong_ty);
            }
            foreach (var item in data_job)
            {
                dic_thuoc_tinh_rieng.TryGetValue(item.id_job, out List<int> tt_rieng);
                if (tt_rieng == null)
                    tt_rieng = new List<int>();

                if (item.cong_ty != null && !string.IsNullOrEmpty(item.cong_ty.id_cong_ty))
                {
                    dic_cong_ty.TryGetValue(item.cong_ty.id_cong_ty, out string ten_cong_ty);
                    item.cong_ty.ten_cong_ty = ten_cong_ty;
                }

                var jo = new JobThuocTinhMap(item, all_thuoc_tinh_job, tt_rieng);

                // tìm nhà tuyển dụng bằng id_job để lấy số ứng viên đã gửi
                var get_note_uv_job_by_ntd = NhaTuyenDungBL.GetListNtdByIdJob(item.id_job);

                var list_share = new List<string>();
                if (get_note_uv_job_by_ntd != null)
                {
                    foreach (var ntd in get_note_uv_job_by_ntd)
                    {
                        list_share.AddRange(ntd.lst_id_share);
                    }
                    list_share = list_share.Distinct().ToList();
                    if (list_share.Count > 0)
                    {
                        jo.ung_vien_da_gui = list_share.Count;
                    }
                }

                // tìm nhà tuyển dụng đã vào xem job

                IList<int> lst = new List<int>();
                lst.Add(-1);
                long total_seen = 0;
                var lst_ntd_seen = NoteBL.Search(lst, out total_seen, out msg, 9999);
                var lst_ntd_get_id = lst_ntd_seen.Select(x => x.id_obj).Distinct();
                var all_nha_tuyen_dung = BL.NhaTuyenDungBL.GetMany(lst_ntd_get_id).Where(x => x.id_job == item.id_job);
                jo.ntd_da_xem = all_nha_tuyen_dung.Count() > 0;

                //tìm xem nhà tuyển dụng đã phản hồi hay chưa
                List<string> list_id_share = new List<string>();
                var note_ung_vien_da_shared = all_nha_tuyen_dung.Select(x => x.lst_id_share);
                foreach (var id in note_ung_vien_da_shared)
                {
                    list_id_share.AddRange(id);
                }

                var id_nha_tuyen_dung = all_nha_tuyen_dung.Select(x => x.id_nha_tuyen_dung);
                var nguoi_tao = string.Join(",", id_nha_tuyen_dung);

                var all_note_cua_ntd = NoteBL.NhaTuyenDungNoteUngVien(list_id_share, nguoi_tao, out total_seen, out msg, 9999).Where(x => list_id_share.Contains(x.id_obj));
                if (all_note_cua_ntd.Count() > 0)
                {
                    jo.ntd_da_phan_hoi = all_note_cua_ntd.Count() > 0;
                    var id = all_nha_tuyen_dung.Select(x => x.id).FirstOrDefault();
                    jo.id_nha_tuyen_dung = string.Join(",", id);
                }

                jo.is_user_job = data_user_job.ContainsKey(item.id_job);
                jo.is_owner = (user == (string.IsNullOrEmpty(item.owner) ? item.nguoi_tao : item.owner) || is_app_admin);
                if (jo.is_user_job)
                {
                    jo.ngay_tao = data_user_job[item.id_job].ngay_nhan_job;
                    jo.id_user_job = data_user_job[item.id_job].id_user_job;
                }
                dic_count_user_job_theo_job.TryGetValue(item.id_job, out long _so_luong_user_job);
                jo.so_luong_user_job = _so_luong_user_job;
                if (jo.is_owner || jo.is_user_job)
                {
                    var thong_ke_thuoc_tinh = new List<dynamic>();
                    if (dic_thong_ke_trang_thai_ung_vien_theo_job.ContainsKey(item.id_job))
                    {
                        foreach (var tt in dic_thong_ke_trang_thai_ung_vien_theo_job[item.id_job])
                        {
                            var tv = all_thuoc_tinh_note_ung_vien_job.Find(f => f.gia_tri == tt.Key);
                            if (tv != null)
                            {
                                thong_ke_thuoc_tinh.Add(new { ten = tv.ten, gia_tri = tt.Key, total = tt.Value });
                            }
                            else
                            {
                                if (tt.Key == 0)
                                {
                                    thong_ke_thuoc_tinh.Add(new { ten = "<b>Tổng số U/V</b>", gia_tri = -1, total = tt.Value });
                                }
                            }
                        }
                    }
                    else
                    {
                        thong_ke_thuoc_tinh.Add(new { ten = "<b>Tổng số U/V</b>", gia_tri = -1, total = 0 });
                    }
                    jo.ung_vien_thong_ke = thong_ke_thuoc_tinh;
                }
                lst_job_map.Add(jo);
            }

            return Ok(new DataResponsePaging() { data = lst_job_map, total = total_recs, success = data_job != null, msg = msg });
        }

        [HttpGet]
        [Route("alljob")]
        
        public IActionResult Search()
        {
            long total_recs = 0;
            string msg = "";

            var data = QLCUNL.BL.JobBL.Search(app_id, user, group, string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, new List<int>(), new List<string>(), new List<string>(), 0, 0, 1, out total_recs, out msg, 9999);

            var lst_id_cong_ty = data.Where(x => x.cong_ty != null && !string.IsNullOrEmpty(x.cong_ty.id_cong_ty)).Select(x => x.cong_ty.id_cong_ty);
            var lst_cong_ty = QLCUNL.BL.CongTyBL.GetMany(lst_id_cong_ty);
            Dictionary<string, string> dic_cong_ty = new Dictionary<string, string>();

            if (lst_cong_ty != null)
            {
                dic_cong_ty = lst_cong_ty.ToDictionary(x => x.id_cong_ty, y => y.ten_cong_ty);
            }

            foreach (var item in data)
            {
                if (item.cong_ty != null && !string.IsNullOrEmpty(item.cong_ty.id_cong_ty))
                {
                    dic_cong_ty.TryGetValue(item.cong_ty.id_cong_ty, out string ten_cong_ty);
                    item.cong_ty.ten_cong_ty = ten_cong_ty;
                }
            }

            return Ok(new DataResponse() { data = data, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.JOB, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }

        [HttpGet]
        [Route("thongkethuoctinh")]
        public IActionResult ThongKeThuocTinh(string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from, long ngay_di_lam_to,
           double don_gia_from, double don_gia_to, string thuoc_tinh, string thuoc_tinh_rieng, string op, string thuoc_tinh2, string thuoc_tinh_rieng2, string op2, long ngay_tao_from, long ngay_tao_to, int page)
        {
            List<string> lst_id = new List<string>();
            List<string> lst_id_obj_thuoc_tinh_rieng = new List<string>();
            List<string> lst_id_should = new List<string>();
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<int> lst_thuoc_tinh2 = new List<int>();
            List<int> lst_thuoc_tinh_rieng2 = new List<int>();

            bool is_find_thuoc_tinh_rieng = false;
            List<Job> data_job = new List<Job>();
            Dictionary<string, UserJob> data_user_job = new Dictionary<string, UserJob>();
            Dictionary<string, Dictionary<int, long>> dic_thong_ke_trang_thai_ung_vien_theo_job = new Dictionary<string, Dictionary<int, long>>();
            long total_recs = 0;
            string msg = "";
            var dic_thong_ke = new Dictionary<int, long>();
            var dic_thong_ke_thuoc_tinh_rieng = new Dictionary<int, long>();
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
            if (!string.IsNullOrEmpty(thuoc_tinh2))
            {
                foreach (var tt in thuoc_tinh2.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh2.Add(t);
                    }
                }
            }

            if (!string.IsNullOrEmpty(thuoc_tinh_rieng) || !string.IsNullOrEmpty(thuoc_tinh_rieng2))
            {
                if (string.IsNullOrEmpty(thuoc_tinh_rieng))
                    thuoc_tinh_rieng = "";
                foreach (var tt in thuoc_tinh_rieng.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh_rieng.Add(t);
                    }
                }
                if (string.IsNullOrEmpty(thuoc_tinh_rieng2))
                    thuoc_tinh_rieng2 = "";
                foreach (var tt in thuoc_tinh_rieng2.Split(','))
                {
                    int t = -1;
                    if (Int32.TryParse(tt, out t))
                    {
                        lst_thuoc_tinh_rieng2.Add(t);
                    }
                }

                is_find_thuoc_tinh_rieng = true;
                var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.JOB, lst_thuoc_tinh_rieng, 1, out long total_recs_thuoc_tinh, out _, 9999, op, lst_thuoc_tinh_rieng2, op2);

                if (lst_id_obj.Count > 0)
                {
                    if (op == "0")
                    {
                        lst_id_should = lst_id_obj.Select(x => x.id_obj).Distinct().ToList();
                    }
                    else
                        lst_id = lst_id_obj.Select(x => x.id_obj).Distinct().ToList();

                    lst_id_obj_thuoc_tinh_rieng = lst_id_obj.Select(x => x.id).ToList();
                }
                else
                {
                    lst_id = new List<string>() { "__NULL__" };
                    lst_id_obj_thuoc_tinh_rieng = new List<string>() { "__NULL__" };
                }
            }

            var user_job = UserJobBL.Search(app_id, user, group, new List<string>(), string.Empty, 0, 0, new List<int>(), new List<string>(), 1, out long total_user_job, out string msg_user_job, 100, (is_app_admin || is_app_admin));
            List<string> lst_id_job = new List<string>();
            if (user_job.Count > 0)
            {
                lst_id_job = user_job.Select(x => x.id_job).Distinct().ToList();
                lst_id_should.AddRange(lst_id_job);
            }

            var tuple = BL.JobBL.ThongKeTheoThuocTinh(app_id, user, group, term, id_ung_vien, id_cong_ty, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, lst_thuoc_tinh, lst_id, lst_id_should,
                ngay_tao_from, ngay_tao_to, page, out long _total_recs, out string _msg, 9999, (is_sys_admin || is_app_admin || is_super_user), null, op, lst_thuoc_tinh2, op2);

            dic_thong_ke = tuple.Item1;
            var lst_id_out = tuple.Item2;
            if (lst_id_out.Count == 0)
            {
                lst_id_out = new List<string>() { "__NULL__" };
            }
            //Nếu có thuộc tính chung và điều kiện là chứa tất cả thì phải tìm lại các bản ghi Job trước sau đó truyền vào bảng ThuocTinhDuLieu để tìm tiếp với các điều kiện của thuộc tính riêng

            var lst_must_not_id = JobBL.GetIdDeleted(app_id);

            dic_thong_ke_thuoc_tinh_rieng = ThuocTinhDuLieuBL.ThongKeTheoThuocTinh(app_id, user, LoaiThuocTinh.JOB, lst_thuoc_tinh_rieng, lst_id_out, new List<string>(), lst_must_not_id, (is_app_admin || is_app_admin), op);

            return Ok(new { data_rieng = dic_thong_ke_thuoc_tinh_rieng.Select(x => new { k = x.Key, v = x.Value }), data = dic_thong_ke.Select(x => new { k = x.Key, v = x.Value }), success = true, msg = "" });
        }

        [HttpGet]
        [Route("checkidexist")]
        public IActionResult IsIdAutoExist(string id_auto)
        {
            if (id_auto != null)
            {
                var is_exist = JobBL.IsIdAutoExist(id_auto, app_id);
                return Ok(new DataResponse() { data = is_exist, success = true, msg = "" });
            }
            return Ok(new DataResponse());
        }
    }
}