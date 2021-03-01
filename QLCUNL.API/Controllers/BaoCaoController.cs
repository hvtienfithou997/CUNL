using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoController : APIBase
    {
        private IConfiguration configuration;

        List<int> gia_tri_thuoc_tinh_bao_cao = new List<int>() { -1 };
        public BaoCaoController(IConfiguration conf)
        {
            configuration = conf;

        }
        string IdThuocTinhToString(Dictionary<int, string> dic_thuoc_tinh, List<int> thuoc_tinh)
        {
            StringBuilder thuoc_tinh_text = new StringBuilder();
            foreach (var tt in thuoc_tinh)
            {
                if (dic_thuoc_tinh.TryGetValue(tt, out string ten))
                    thuoc_tinh_text.AppendFormat("-{0} ", ten);
            }
            return thuoc_tinh_text.ToString();
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            QLCUNL.BL.UserJobBL.Search(app_id, user, group, null, string.Empty, 0, 0, new List<int>(), new List<string>(), 1, out long total_user_job, out _, 0, (is_sys_admin || is_app_admin));
            QLCUNL.BL.NoteUngVienJobBL.Search(app_id, user, group, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, 0, 0, 0, 0, new List<int>(), new List<string>(), 1, out long total_note_ung_vien_job, out _, 0, (is_sys_admin || is_app_admin), new string[] { });
            QLCUNL.BL.UngVienBL.Search(app_id, user, group, string.Empty, string.Empty, 0, 0, 0, 0, new List<int>(), new List<string>(), 0, 0, 1, out long total_ung_vien, out _, 0, (is_sys_admin || is_app_admin), null, null);
            QLCUNL.BL.JobBL.Search(app_id, user, group, string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, new List<int>(), new List<string>(), new List<string>(), 0, 0, 1, out long total_job, out _, 0, (is_sys_admin || is_app_admin));

            return Ok(new { success = true, job_cua_toi = total_user_job, ung_vien_theo_job = total_note_ung_vien_job, ung_vien = total_ung_vien, job_chung = total_job });
        }
        //GET: api/label
        [HttpGet]
        [Route("view")]
        public IActionResult Get(long ngay_nhan_job_from, long ngay_nhan_job_to, string thuoc_tinh, bool gop_job = true)
        {
            var default_settings = QLCUNL.BL.UserBL.GetDefaultSettingByAppId(app_id);
            gia_tri_thuoc_tinh_bao_cao = new List<int>() { default_settings.trang_thai_user_job_bao_cao };
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
            }
            else
            {
                lst_thuoc_tinh = new List<int>() { default_settings.trang_thai_user_job_bao_cao };
            }
            ///Lấy tất cả JOB của tôi ở trạng thái đã hoàn thành JOB để báo cáo

            var data_user_job = QLCUNL.BL.UserJobBL.Search(app_id, user, group, null, string.Empty, ngay_nhan_job_from, ngay_nhan_job_to, new List<int>(), new List<string>(), 1, out _, out string msg, 9999, (is_sys_admin || is_app_admin));
            var lst_id_job = data_user_job.Select(x => x.id_job);
            var data_job = QLCUNL.BL.JobBL.GetMany(app_id, lst_id_job, new string[] { }).ToDictionary(x => x.id_job, y => y);
            var lst_id_user_job = data_user_job.Select(x => x.id_user_job);
            var lst_ung_vien_theo_job = QLCUNL.BL.NoteUngVienJobBL.GetNoteUngVienJobByIdUserJob(app_id, lst_id_user_job, lst_thuoc_tinh, 1, out _, out _, 9999, (is_sys_admin || is_app_admin));
            var lst_thuoc_tinh_job = QLCUNL.BL.ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, (int)LoaiThuocTinh.JOB, (int)ThuocTinhType.SHARED);
            Dictionary<int, string> dic_thuoc_tinh_job = new Dictionary<int, string>();
            foreach (var tt in lst_thuoc_tinh_job)
            {
                if (!dic_thuoc_tinh_job.ContainsKey(tt.gia_tri))
                    dic_thuoc_tinh_job.Add(tt.gia_tri, tt.ten);
            }
            var lst_id_user_job_need_display = lst_ung_vien_theo_job.Select(x => x.id_user_job);
            List<dynamic> lst = new List<dynamic>();
            int count = 0;

            var data_user_job_bao_cao = data_user_job.Where(x => lst_id_user_job_need_display.Contains(x.id_user_job));

            foreach (var user_job in data_user_job_bao_cao)
            {
                try
                {
                    long so_ngay_bao_hanh_theo_job = 0;


                    string nguoi_tuyen_dung = ""; string dia_diem = ""; string vi_tri = "";
                    long so_luong_yeu_cau = 0; double don_gia = 0; double tien_coc = 0;
                    int so_ung_vien_da_tuyen_duoc = 0; int so_ung_vien_can_tuyen_tiep = 0;
                    double so_tien_can_thanh_toan_theo_ung_vien_du_kien = 0;
                    double so_tien_can_thanh_toan_theo_ung_vien = 0;
                    double so_tien_con_phai_thanh_toan_du_kien = 0;
                    double so_tien_con_phai_thanh_toan = 0;
                    string ngay_cac_ung_vien_di_lam = "";
                    string tinh_trang_thanh_toan = "";
                    string han_thanh_toan = "";
                    string tinh_trang_hop_dong = "";
                    double so_tien_da_nhan = 0;
                    if (data_job.TryGetValue(user_job.id_job, out Job job))
                    {
                        nguoi_tuyen_dung = $"<a href='/congty/detail/{job.cong_ty.id_cong_ty}' target='_blank'>{job.nguoi_lien_he}</a>";
                        dia_diem = job.dia_chi;
                        vi_tri = $"<a href='/job/detail/{job.id_job}' target='_blank'>{job.chuc_danh}</a>";
                        so_luong_yeu_cau = job.so_luong;
                        tien_coc = job.tien_coc;
                        don_gia = job.don_gia;
                        so_ngay_bao_hanh_theo_job = job.thoi_han_bao_hanh;
                        tinh_trang_hop_dong = IdThuocTinhToString(dic_thuoc_tinh_job, job.thuoc_tinh);
                    }
                    ///Tính theo ứng viên đầu tiên (F0) có trạng thái ĐÃ ĐI LÀM và đã hết hạn bảo hành (theo ngày)
                    ///
                    //Ứng viên đã đi làm
                    var grp_ung_vien_by_user_job_da_di_lam = lst_ung_vien_theo_job.Where(x => string.IsNullOrEmpty(x.parent_id_ung_vien) && x.id_user_job == user_job.id_user_job && x.thuoc_tinh != null
                        && gia_tri_thuoc_tinh_bao_cao.All(x.thuoc_tinh.Contains) && x.ngay_di_lam > 0);

                    so_ung_vien_da_tuyen_duoc = grp_ung_vien_by_user_job_da_di_lam.Count();

                    if (grp_ung_vien_by_user_job_da_di_lam.Count() > 0)
                    {
                        ngay_cac_ung_vien_di_lam = string.Join("<br>", grp_ung_vien_by_user_job_da_di_lam.Select(x => XMedia.XUtil.EpochToTime(x.ngay_di_lam).ToString("dd-MM")));

                        if (job.don_gia_theo_luong)
                        {
                            so_tien_can_thanh_toan_theo_ung_vien_du_kien = grp_ung_vien_by_user_job_da_di_lam.Sum(x => x.luong_chinh_thuc * job.don_gia);
                        }
                        else
                        {
                            so_tien_can_thanh_toan_theo_ung_vien_du_kien = so_ung_vien_da_tuyen_duoc * don_gia;
                        }
                        so_tien_da_nhan = grp_ung_vien_by_user_job_da_di_lam.Sum(x => x.so_tien_da_nhan);
                        so_tien_con_phai_thanh_toan_du_kien = so_tien_can_thanh_toan_theo_ung_vien_du_kien - job.tien_coc - so_tien_da_nhan;
                        so_tien_con_phai_thanh_toan_du_kien = so_tien_con_phai_thanh_toan_du_kien <= 0 ? 0 : so_tien_con_phai_thanh_toan_du_kien;

                    }
                    //Ứng viên đã đi làm và số ngày bảo hành đã hết
                    var grp_ung_vien_by_user_job = grp_ung_vien_by_user_job_da_di_lam.Where(x => XMedia.XUtil.TimeInEpoch(DateTime.Now.AddDays(-so_ngay_bao_hanh_theo_job)) > x.ngay_di_lam);
                    if (grp_ung_vien_by_user_job.Count() > 0)
                    {
                        ngay_cac_ung_vien_di_lam = string.Join("<br>", grp_ung_vien_by_user_job.Select(x => XMedia.XUtil.EpochToTime(x.ngay_di_lam).ToString("dd-MM")));

                        if (job.don_gia_theo_luong)
                        {
                            so_tien_can_thanh_toan_theo_ung_vien = grp_ung_vien_by_user_job.Sum(x => x.luong_chinh_thuc * job.don_gia);
                        }
                        else
                        {
                            so_tien_can_thanh_toan_theo_ung_vien = grp_ung_vien_by_user_job.Count() * don_gia;
                        }
                        so_tien_con_phai_thanh_toan = so_tien_can_thanh_toan_theo_ung_vien - job.tien_coc - grp_ung_vien_by_user_job.Sum(x => x.so_tien_da_nhan);
                        so_tien_con_phai_thanh_toan = so_tien_con_phai_thanh_toan <= 0 ? 0 : so_tien_con_phai_thanh_toan;
                    }
                    dynamic data = new
                    {
                        id_job = user_job.id_job,
                        stt = ++count,
                        ngay_nhan_job = $"<a href='/userjob/detail/{user_job.id_user_job}' target='_blank'>{XMedia.XUtil.EpochToTime(user_job.ngay_nhan_job).ToString("dd-MM")}</a>",
                        nguoi_tuyen_dung = nguoi_tuyen_dung,
                        dia_diem = dia_diem,
                        vi_tri = vi_tri,
                        so_luong_yeu_cau = so_luong_yeu_cau,
                        don_gia = job.don_gia_theo_luong ? $"{job.don_gia} tháng lương" : don_gia.ToString("0,00").Replace(",", "."),
                        tien_coc = tien_coc,
                        so_ung_vien_da_tuyen_duoc = so_ung_vien_da_tuyen_duoc,
                        ngay_cac_ung_vien_di_lam = ngay_cac_ung_vien_di_lam,
                        so_ung_vien_can_tuyen_tiep = so_ung_vien_can_tuyen_tiep,
                        so_tien_can_thanh_toan_theo_ung_vien = so_tien_can_thanh_toan_theo_ung_vien,
                        so_tien_con_phai_thanh_toan = so_tien_con_phai_thanh_toan,
                        tinh_trang_thanh_toan = tinh_trang_thanh_toan,
                        han_thanh_toan = han_thanh_toan,
                        tinh_trang_hop_dong = tinh_trang_hop_dong,
                        so_tien_can_thanh_toan_theo_ung_vien_du_kien,
                        so_tien_con_phai_thanh_toan_du_kien,
                        so_tien_da_nhan
                    };
                    lst.Add(data);
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            if (gop_job)
            {
                var tmp = lst.GroupBy(x => x.id_job);

                lst = new List<dynamic>();
                count = 0;
                foreach (var item in tmp)
                {
                    dynamic data = new
                    {
                        stt = ++count,
                        ngay_nhan_job = item.First().ngay_nhan_job,
                        nguoi_tuyen_dung = item.First().nguoi_tuyen_dung,
                        dia_diem = item.First().dia_diem,
                        vi_tri = item.First().vi_tri,
                        so_luong_yeu_cau = item.First().so_luong_yeu_cau,
                        don_gia = item.First().don_gia,
                        tien_coc = item.First().tien_coc,
                        so_ung_vien_da_tuyen_duoc = item.Sum(x => x.so_ung_vien_da_tuyen_duoc),
                        ngay_cac_ung_vien_di_lam = string.Join("<br>", item.Select(x => x.ngay_cac_ung_vien_di_lam)),
                        so_ung_vien_can_tuyen_tiep = item.Sum(x => x.so_ung_vien_can_tuyen_tiep),
                        so_tien_can_thanh_toan_theo_ung_vien = item.Sum(x => (double)x.so_tien_can_thanh_toan_theo_ung_vien),
                        so_tien_con_phai_thanh_toan = item.Sum(x => (double)x.so_tien_con_phai_thanh_toan),
                        tinh_trang_thanh_toan = item.First().tinh_trang_thanh_toan,
                        han_thanh_toan = item.First().han_thanh_toan,
                        tinh_trang_hop_dong = item.First().tinh_trang_hop_dong
                    };
                    lst.Add(data);
                }

            }


            return Ok(new DataResponse() { data = lst, success = true, msg = "" });
        }

    }
}