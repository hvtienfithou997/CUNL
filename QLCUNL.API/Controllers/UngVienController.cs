using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Nest;
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
    public class UngVienController : APIBase, IAPIBase
    {
        private IConfiguration configuration;
        private List<int> gia_tri_thuoc_tinh_bao_cao = null;

        public UngVienController(IConfiguration conf)
        {
            configuration = conf;
            gia_tri_thuoc_tinh_bao_cao = configuration.GetSection("BaoCao:GiaTriThuocTinh").Get<List<int>>();
        }

        [HttpPost]
        [Route("all")]
        public IActionResult All([FromBody] BaseGetAll res)
        {
            List<UngVien> ung_vien = QLCUNL.BL.UngVienBL.GetAll(res.page_index, res.page_size).ToList();

            return Ok(new DataResponse() { data = ung_vien, success = ung_vien != null, msg = "" });
        }

        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            if (QLCUNL.BL.UngVienBL.CanView(id, user, group) || (is_sys_admin || is_app_admin))
            {
                var ung_vien = QLCUNL.BL.UngVienBL.GetById(id);
                if (ung_vien != null)
                {
                    var lst_id_thuoc_tinh = ung_vien.thuoc_tinh != null ? ung_vien.thuoc_tinh : new List<int>();
                    var thuoc_tinh_chung = ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.UNG_VIEN, ThuocTinhType.SHARED);
                    var thuoc_tinh_chung1 = ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.UNG_VIEN, ThuocTinhType.PRIVATE);
                    var list = thuoc_tinh_chung.Concat(thuoc_tinh_chung1).ToList();

                    var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, ung_vien.id_ung_vien, user);
                    if (lst_thuoc_tinh_rieng.Count() > 0)
                    {
                        var thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.UNG_VIEN, (is_sys_admin || is_app_admin));
                        list.AddRange(thuoc_tinh_rieng);
                    }
                    UngVienMap uv_map = new UngVienMap(ung_vien, list);

                    return Ok(new DataResponse() { data = uv_map, success = ung_vien != null, msg = "" });
                }
                return Ok(new DataResponse() { data = ung_vien, success = ung_vien != null, msg = "" });
            }
            return Ok(new DataResponse() { data = new { }, success = false, msg = "" });
        }

        [HttpGet]
        [Route("viewinjob")]
        public IActionResult GetInJob(string id, string id_job, string id_user_job)
        {
            if (QLCUNL.BL.UngVienBL.CanView(id, user, group) || (is_sys_admin || is_app_admin) || QLCUNL.BL.NoteUngVienJobBL.IsNoteUngVienJobExistInJob(app_id, id_job, id))
            {
                var ung_vien = QLCUNL.BL.UngVienBL.GetById(id);
                if (ung_vien != null)
                {
                    var user_job_name = BL.UserJobBL.GetById(id_user_job)?.id_user;

                    var lst_id_thuoc_tinh = ung_vien.thuoc_tinh != null ? ung_vien.thuoc_tinh : new List<int>();
                    var thuoc_tinh_chung = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.UNG_VIEN, ThuocTinhType.SHARED);
                    var thuoc_tinh_chung1 = QLCUNL.BL.ThuocTinhBL.GetManyByGiaTri(app_id, lst_id_thuoc_tinh, LoaiThuocTinh.UNG_VIEN, ThuocTinhType.PRIVATE);
                    var list = thuoc_tinh_chung.Concat(thuoc_tinh_chung1).ToList();

                    var lst_thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, ung_vien.id_ung_vien, user);
                    if (lst_thuoc_tinh_rieng.Count() > 0)
                    {
                        var thuoc_tinh_rieng = QLCUNL.BL.ThuocTinhBL.GetPrivateByLoaiGiaTri(app_id, user, lst_thuoc_tinh_rieng, LoaiThuocTinh.UNG_VIEN, (is_sys_admin || is_app_admin));
                        list.AddRange(thuoc_tinh_rieng);
                    }

                    Models.UngVienMap uv_map = new Models.UngVienMap(ung_vien, list);
                    uv_map.user_job_name = user_job_name;
                    return Ok(new DataResponse() { data = uv_map, success = ung_vien != null, msg = "" });
                }
                return Ok(new DataResponse() { data = ung_vien, success = ung_vien != null, msg = "" });
            }
            return Ok(new DataResponse() { data = new { }, success = false, msg = "" });
        }

        // POST: api/ungvien
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            NoteUngVienJobController j = new NoteUngVienJobController();

            try
            {
                var ung_vien = Newtonsoft.Json.JsonConvert.DeserializeObject<UngVien>(value.ToString());
                SetMetaData(ung_vien, false);

                string ung_vien_id = QLCUNL.BL.UngVienBL.IndexRetId(ung_vien);
                res.success = !string.IsNullOrEmpty(ung_vien_id);
                if (res.success)
                {
                    UpsertThuocTinhRieng(value, ung_vien_id, LoaiThuocTinh.UNG_VIEN);
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // add ứng viên khi có thêm auto id

        [HttpPost]
        [Route("addungvienbyautoid")]
        public IActionResult addUngVienByAutoID([FromBody] object value, string id_auto)
        {
            DataResponse res = new DataResponse();
            try
            {
                var ung_vien = Newtonsoft.Json.JsonConvert.DeserializeObject<UngVien>(value.ToString());
                SetMetaData(ung_vien, false);

                string ung_vien_id = QLCUNL.BL.UngVienBL.IndexRetId(ung_vien);
                res.success = !string.IsNullOrEmpty(ung_vien_id);

                if (res.success)
                {
                    UpsertThuocTinhRieng(value, ung_vien_id, LoaiThuocTinh.UNG_VIEN);
                }
                GetByIdAuto(id_auto, ung_vien_id);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        public IActionResult GetByIdAuto(string id_auto, string id_ung_vien)
        {
            var get_job = QLCUNL.BL.JobBL.GetByAutoID(app_id, id_auto);
            var id_job = get_job.id_job;
            var user_job = QLCUNL.BL.UserJobBL.GetUserJobByIdJob(app_id, id_job, user);
            var id_user_job = "";
            foreach (var id_userj in user_job)
            {
                if (id_userj.id_user_job != null)
                    id_user_job = id_userj.id_user_job;
            }
            DataResponse res = new DataResponse();
            try
            {
                List<NoteUngVienJob> lst = new List<NoteUngVienJob>();
                List<string> id_ung_viens = id_ung_vien.Split(",").ToList();
                foreach (var ung_vien in id_ung_viens)
                {
                    NoteUngVienJob note = new NoteUngVienJob();
                    note.id_job = id_job;
                    note.id_user_job = id_user_job;
                    note.id_ung_vien = ung_vien;
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

        // PUT: api/User/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var ung_vien = Newtonsoft.Json.JsonConvert.DeserializeObject<UngVien>(value.ToString());
                var obj = JToken.Parse(value.ToString());
                ung_vien.id_ung_vien = id;

                if (UngVienBL.CanEdit(id, user, group) || (is_sys_admin || is_app_admin))
                {
                    SetMetaData(ung_vien, true);
                    res.success = QLCUNL.BL.UngVienBL.Update(ung_vien);
                    UpsertThuocTinhRieng(value, id, LoaiThuocTinh.UNG_VIEN);
                    if (!string.IsNullOrEmpty(obj["note_ung_vien"].ToString()))
                    {
                        var note = new Note();
                        note.noi_dung = obj["note_ung_vien"].ToString();
                        note.id_obj = id;
                        note.loai = LoaiNote.UNG_VIEN;
                        note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                        note.thuoc_tinh = new List<int>();
                        SetMetaData(note, false);
                        res.success = NoteBL.Index(note);
                    }
                }
                else
                {
                    res.msg = "Không có quyền sửa đối tượng này";
                }
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
            if (QLCUNL.BL.UngVienBL.CanDelete(id, user, group) || (is_sys_admin || is_app_admin))
            {
                bool del = UngVienBL.Delete(id);
                return del;
            }
            return false;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, string id_ung_vien, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_mong_muon_from, double luong_mong_muon_to, string thuoc_tinh, string thuoc_tinh_rieng,
           long ngay_tao_from, long ngay_tao_to, bool tim_ung_vien_team_khac, int page, int page_size, string field_sort = "", string sort = "")
        {
            Dictionary<string, bool> sort_order = new Dictionary<string, bool>();

            if (!string.IsNullOrEmpty(field_sort) && !string.IsNullOrEmpty(sort))
            {
                sort_order.Add(field_sort, sort == "0" ? true : false);
            }
            else
            {
                sort_order.Add("ngay_tao", true);
            }

            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<UngVien> data = new List<UngVien>();
            List<UngVienMap> list_uv = new List<UngVienMap>();
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
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.UNG_VIEN, lst_thuoc_tinh_rieng, 1, out long total_recs_thuoc_tinh, out _, page_size * 2);
                    lst_id = lst_id_obj.Where(x => !string.IsNullOrEmpty(x.id_obj)).Select(x => x.id_obj).ToList();
                }
            }
            if (tim_ung_vien_team_khac)
            {
                //Tìm admin của app_id này và lấy default_settings xem có cho phép tìm trong team khác hay không, nếu có thì cho phép tìm
                var default_settings = UserBL.GetDefaultSettingByAppId(app_id);
                tim_ung_vien_team_khac = default_settings.tim_ung_vien_team_khac;
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0 && lst_thuoc_tinh.Count == 0)
            {
            }
            else

                data = QLCUNL.BL.UngVienBL.Search(app_id, user, group, term, id_ung_vien, ngay_di_lam_from, ngay_di_lam_to,
            luong_mong_muon_from, luong_mong_muon_to, lst_thuoc_tinh, lst_id, ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size,
            (is_sys_admin || is_app_admin || tim_ung_vien_team_khac), null, sort_order);
            var dic_thong_ke_ung_vien_job = NoteUngVienJobBL.ThongKeUngVienTheoJob(app_id, data.Select(x => x.id_ung_vien));

            var all_thuoc_tinh_uv = ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, (int)LoaiThuocTinh.UNG_VIEN, -1);

            foreach (var item in data)
            {
                var tt_theo_ung_vien = all_thuoc_tinh_uv.Where(x => item.thuoc_tinh.Contains(x.gia_tri));
                var uv_map = new UngVienMap(item, tt_theo_ung_vien);
                dic_thong_ke_ung_vien_job.TryGetValue(item.id_ung_vien, out long so_luong_job_da_gan);
                uv_map.so_luong_job_da_gan = so_luong_job_da_gan;
                list_uv.Add(uv_map);
            }
            return Ok(new DataResponsePaging() { data = list_uv, total = total_recs, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("allungvien")]
        public IActionResult GetAllUngVien()
        {
            List<UngVien> data = new List<UngVien>();
            long total_recs = 0;
            string msg = "";

            data = QLCUNL.BL.UngVienBL.Search(app_id, user, group, string.Empty, string.Empty, 0, 0, 0, 0, new List<int>(), new List<string>(), 0, 0, 1, out total_recs, out msg, 9999, (is_sys_admin || is_app_admin), null, null);
            return Ok(new DataResponse() { data = data, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("allungvienbyuser")]
        public IActionResult GetAllUVByUser(string id_user_job)
        {
            List<string> kq = new List<string>();
            List<UngVien> data = new List<UngVien>();
            long total_recs = 0;
            string msg = "";
            UngVien uv = new UngVien();

            kq = QLCUNL.BL.NoteUngVienJobBL.GetListIdUngVienByIdUserJob(app_id, id_user_job, 1, out total_recs, out msg, 9999, (is_sys_admin || is_app_admin)).ToList();
            foreach (var item in kq)
            {
                uv = QLCUNL.BL.UngVienBL.GetById(item);
                data.Add(uv);
            }
            return Ok(new DataResponsePaging() { data = data, total = total_recs, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("free")]
        public IActionResult SearchUngVienFree(string term, string id_user_job, string id_ung_vien, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_mong_muon_from, double luong_mong_muon_to, string thuoc_tinh, string thuoc_tinh_rieng,
           long ngay_tao_from, long ngay_tao_to, int page, int page_size)
        {
            var default_settings = QLCUNL.BL.UserBL.GetDefaultSettingByAppId(app_id);
            List<int> lst_thuoc_tinh = new List<int>();
            List<int> lst_thuoc_tinh_rieng = new List<int>();
            List<string> lst_id = new List<string>();
            bool is_find_thuoc_tinh_rieng = false;
            List<UngVien> data = new List<UngVien>();
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
                    var lst_id_obj = BL.ThuocTinhDuLieuBL.Search(app_id, user, LoaiThuocTinh.UNG_VIEN, lst_thuoc_tinh_rieng, 1, out long total_recs_thuoc_tinh, out _, page_size * 2);
                    lst_id = lst_id_obj.Select(x => x.id_obj).ToList();
                }
            }
            if (is_find_thuoc_tinh_rieng && lst_id.Count == 0)
            {
            }
            else
            {
                List<UngVienMap> uv_map = new List<UngVienMap>();

                var get_all_note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.GetListIdUngVienByIdUserJob(app_id, id_user_job, page, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin));
                var lst_id_ung_vien_da_di_lam = QLCUNL.BL.NoteUngVienJobBL.Search(app_id, user, group, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, 0, 0, 0, 0,
                    new List<int>() { default_settings.trang_thai_user_job_bao_cao }, new List<string>(), 1, out _, out _, 9999, (is_sys_admin || is_app_admin), null).Select(x => x.id_ung_vien);

                data = QLCUNL.BL.UngVienBL.Search(app_id, user, group, term, id_ung_vien, ngay_di_lam_from,
                    ngay_di_lam_to,
                    luong_mong_muon_from, luong_mong_muon_to, lst_thuoc_tinh, lst_id, ngay_tao_from, ngay_tao_to, page,
                    out total_recs, out msg, page_size, (is_sys_admin || is_app_admin), lst_id_ung_vien_da_di_lam, null).Where(x => !get_all_note_ung_vien_job.Contains(x.id_ung_vien)).ToList();
                foreach (var item in data)
                {
                    var all_thuoc_tinh_uv = ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, (int)LoaiThuocTinh.UNG_VIEN, -1).Where(x => item.thuoc_tinh.Contains(x.gia_tri));
                    var uv = new UngVienMap(item, all_thuoc_tinh_uv);
                    uv_map.Add(uv);
                }
                return Ok(new DataResponsePaging() { data = uv_map, total = total_recs, success = data != null, msg = msg });
            }

            return Ok(new DataResponsePaging() { data = data, total = total_recs, success = data != null, msg = msg });
        }

        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty,
                PhanQuyenObjType.UNG_VIEN, id, Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
            return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
        }
        [HttpPost]
        [Route("savenote")]
        public IActionResult SaveNote([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = JToken.Parse(value.ToString());
                if (obj != null)
                {
                    var is_ok = false;
                    string id_ung_vien = obj["id_ung_vien"].ToString();
                    List<int> thuoc_tinh_ung_vien = obj["thuoc_tinh"].ToObject<List<int>>();
                    List<int> thuoc_tinh_rieng_ung_vien = obj["thuoc_tinh_rieng"].ToObject<List<int>>();
                    List<int> thuoc_tinh_ghi_chu = obj["thuoc_tinh_ghi_chu"].ToObject<List<int>>();
                    //thuộc tính ứng viên
                    is_ok = QLCUNL.BL.UngVienBL.SetThuocTinh(id_ung_vien, thuoc_tinh_ung_vien);
                    if (thuoc_tinh_rieng_ung_vien.Count > 0)
                    {
                        ThuocTinhDuLieu thuoc_tinh_rieng = new ThuocTinhDuLieu();
                        thuoc_tinh_rieng.id_obj = id_ung_vien;
                        thuoc_tinh_rieng.loai_obj = LoaiThuocTinh.UNG_VIEN;
                        thuoc_tinh_rieng.thuoc_tinh = thuoc_tinh_rieng_ung_vien;
                        SetMetaData(thuoc_tinh_rieng, false);
                        is_ok = is_ok | QLCUNL.BL.ThuocTinhDuLieuBL.Index(thuoc_tinh_rieng);
                    }
                    if (!string.IsNullOrEmpty(obj["ghi_chu"].ToString()))
                    {
                        //ghi chú ứng viên
                        //var note_ung_vien = new NoteUngVien();
                        var note = new Note();

                        note.noi_dung = obj["ghi_chu"].ToString();
                        note.id_obj = id_ung_vien;
                        note.loai = LoaiNote.UNG_VIEN;
                        note.loai_du_lieu = LoaiDuLieu.NGUOI_DUNG;
                        note.thuoc_tinh = thuoc_tinh_ghi_chu;

                        //note_ung_vien.ghi_chu = obj["ghi_chu"].ToString();
                        //note_ung_vien.id_ung_vien = id_ung_vien;
                        //note_ung_vien.thuoc_tinh = thuoc_tinh_ghi_chu;
                        SetMetaData(note, false);

                        is_ok = is_ok | QLCUNL.BL.NoteBL.Index(note);
                    }
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
        public IActionResult ThongKeThuocTinh(string term, string id_ung_vien, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_mong_muon_from, double luong_mong_muon_to, string thuoc_tinh, string thuoc_tinh_rieng,
           long ngay_tao_from, long ngay_tao_to, bool tim_ung_vien_team_khac, int page, int page_size, string field_sort = "", string sort = "")
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
            Dictionary<string, bool> sort_order = new Dictionary<string, bool>();

            if (!string.IsNullOrEmpty(field_sort) && !string.IsNullOrEmpty(sort))
            {
                sort_order.Add(field_sort, sort == "0" ? true : false);
            }
            else
            {
                sort_order.Add("ngay_tao", true);
            }

            var tupe = QLCUNL.BL.UngVienBL.Search(app_id, user, group, term, id_ung_vien, ngay_di_lam_from, ngay_di_lam_to,
              luong_mong_muon_from, luong_mong_muon_to, lst_thuoc_tinh, lst_id, ngay_tao_from, ngay_tao_to, 1, out long _total_recs, out string _msg, 0,
              (is_sys_admin || is_app_admin || tim_ung_vien_team_khac), null, sort_order, true);

            return Ok(new DataResponse() { data = tupe.Item2.Select(x => new { k = x.Key, v = x.Value }), success = true, msg = "" });
        }

        [HttpGet]
        [Route("ungvienwasseen")]
        public IActionResult UngVienDuocXemCv(string id_ung_vien)
        {
            long total_recs = 0;
            string msg = "";
            // Có id ứng viên => tìm được tất cả các note ứng viên job
            // Từ Id note ứng viên job lấy được các note, nhà tuyển dụng đã xem cv c
            var get_note_uv_job_by_id_uv = NoteUngVienJobBL.GetNoteUngVienByIdUngVien(app_id, id_ung_vien.Split(","), 1, out total_recs, out msg, 999, (is_sys_admin || is_app_admin));
            // lấy id note ứng viên job
            var id_note_ung_vien_job = get_note_uv_job_by_id_uv.Select(x => x.id_note_ung_vien_job);
            var all_log_xem_cv_ung_vien = NoteBL.GetLogXemCvTuyenDung(id_note_ung_vien_job, out total_recs, out msg, 9999);
            var cv = all_log_xem_cv_ung_vien.GroupBy(x => x.id_obj).ToDictionary(o => o.Key, o => o.Select(x => x.noi_dung));

            //
            //lấy id job
            var lst_id_job = get_note_uv_job_by_id_uv.Select(x => x.id_job);
            // tìm nhà tuyển dụng bằng id_job sau đó lấy được những nhà tuyển dụng đã được tạo ra gắn với job đó
            var nha_tuyen_dung_job = NhaTuyenDungBL.GetNhaTuyenDungByIdJob(app_id, lst_id_job, 1, out total_recs, out msg, 9999);
            // lst id đã shared trong mỗi nhà tuyển dụng
            var lst_id_shared = nha_tuyen_dung_job.Select(x => x.lst_id_share);
            List<string> lst = new List<string>();
            foreach (var id in lst_id_shared)
            {
                lst.AddRange(id);
            }
            bool is_exist = lst.Intersect(id_note_ung_vien_job).Any();
            //
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in cv.Where(x => id_note_ung_vien_job.Contains(x.Key)))
            {
                dic.Add(item.Key, item.Value.FirstOrDefault());
            }
            foreach (var get in get_note_uv_job_by_id_uv)
            {
                if (dic.ContainsKey(get.id_note_ung_vien_job))
                {
                    get.ghi_chu = dic.Values.FirstOrDefault();
                }
                else if (is_exist)
                {
                    get.ghi_chu = "Đã gửi";
                }
                else
                {
                    get.ghi_chu = "Chưa gửi";
                }
            }
            return Ok(new DataResponse { data = get_note_uv_job_by_id_uv, success = true, msg = msg });
        }
    }
}