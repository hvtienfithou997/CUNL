
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class JobThuocTinhMap : BaseET
    {
        public JobThuocTinhMap() { }

        public JobThuocTinhMap(NhaTuyenDung tuyen_dung, Dictionary<string, Job> data_job)
        {
            Job job;
            if (data_job.TryGetValue(tuyen_dung.id_job, out job))
            {
                this.id_job = tuyen_dung.id_job;
                this.chuc_danh = job.chuc_danh;
                this.id = tuyen_dung.id;
                this.id_nha_tuyen_dung = tuyen_dung.id_nha_tuyen_dung;
                this.noi_dung = tuyen_dung.noi_dung;
                token = tuyen_dung.token;
                this.luong_chinh_thuc = job.luong_chinh_thuc;
                dia_chi = job.dia_chi;
                id_auto = job.id_auto;
            }
        }
        public JobThuocTinhMap(Job _job, IEnumerable<ThuocTinh> _thuoc_tinh)
        {
            id_job = _job.id_job;
            chuc_danh = _job.chuc_danh;
            tinh_thanh = _job.tinh_thanh;
            so_luong = _job.so_luong;
            luong_chinh_thuc = _job.luong_chinh_thuc;
            luong_chinh_thuc_text = _job.luong_chinh_thuc_text;
            nguoi_lien_he = _job.nguoi_lien_he;
            cong_ty = _job.cong_ty;
            don_gia = _job.don_gia;
            co_hop_dong = _job.co_hop_dong;
            thoi_han_bao_hanh = _job.thoi_han_bao_hanh;
            so_lan_doi_toi_da = _job.so_lan_doi_toi_da;
            dat_coc = _job.dat_coc;
            tien_coc = _job.tien_coc;
            ghi_chu = _job.ghi_chu;
            noi_dung = _job.noi_dung;
            don_gia_theo_luong = _job.don_gia_theo_luong;
            link_job = _job.link_job;
            link_job_upload = _job.link_job_upload;
            dia_chi = _job.dia_chi;

            var item_thuoc_tinh = new List<int>();
            if (_job.thuoc_tinh != null)
                item_thuoc_tinh = _job.thuoc_tinh;
            thuoc_tinh = _thuoc_tinh.Where(x => _job.thuoc_tinh.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type });
            //thuoc_tinh = _thuoc_tinh.Where(x => item_thuoc_tinh.Contains(x.gia_tri) || thuoc_tinh_rieng.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type }).ToList();
            ngay_sua = _job.ngay_sua;
            ngay_tao = _job.ngay_tao;
            nguoi_tao = _job.nguoi_tao;
            nguoi_sua = _job.nguoi_sua;
            tien_coc_tra_lai = _job.tien_coc_tra_lai;
            auto_id = _job.auto_id;
            id_auto = _job.id_auto;
            owner = _job.owner;
            ngay_nhan_hd = _job.ngay_nhan_hd;
        }
        public JobThuocTinhMap(Job _job, IEnumerable<ThuocTinh> _thuoc_tinh, List<int> thuoc_tinh_rieng)
        {
            id_job = _job.id_job;
            chuc_danh = _job.chuc_danh;
            tinh_thanh = _job.tinh_thanh;
            so_luong = _job.so_luong;
            luong_chinh_thuc = _job.luong_chinh_thuc;
            luong_chinh_thuc_text = _job.luong_chinh_thuc_text;
            nguoi_lien_he = _job.nguoi_lien_he;
            cong_ty = _job.cong_ty;
            don_gia = _job.don_gia;
            co_hop_dong = _job.co_hop_dong;
            thoi_han_bao_hanh = _job.thoi_han_bao_hanh;
            so_lan_doi_toi_da = _job.so_lan_doi_toi_da;
            dat_coc = _job.dat_coc;
            tien_coc = _job.tien_coc;
            ghi_chu = _job.ghi_chu;
            noi_dung = _job.noi_dung;
            don_gia_theo_luong = _job.don_gia_theo_luong;
            link_job = _job.link_job;
            link_job_upload = _job.link_job_upload;
            dia_chi = _job.dia_chi;

            var item_thuoc_tinh = new List<int>();
            if (_job.thuoc_tinh != null)
                item_thuoc_tinh = _job.thuoc_tinh;
            //thuoc_tinh = _thuoc_tinh.Where(x => _job.thuoc_tinh.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type });
            thuoc_tinh = _thuoc_tinh.Where(x => item_thuoc_tinh.Contains(x.gia_tri) || thuoc_tinh_rieng.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type }).ToList();
            ngay_sua = _job.ngay_sua;
            ngay_tao = _job.ngay_tao;
            nguoi_tao = _job.nguoi_tao;
            nguoi_sua = _job.nguoi_sua;
            tien_coc_tra_lai = _job.tien_coc_tra_lai;
            auto_id = _job.auto_id;
            id_auto = _job.id_auto;
            owner = _job.owner;
            ngay_nhan_hd = _job.ngay_nhan_hd;
            do_uu_tien = _job.do_uu_tien;
        }

        public JobThuocTinhMap(Job _job, IEnumerable<ThuocTinh> _thuoc_tinh, string id)
        {
            id_job = _job.id_job;
            chuc_danh = _job.chuc_danh;
            tinh_thanh = _job.tinh_thanh;
            so_luong = _job.so_luong;
            luong_chinh_thuc = _job.luong_chinh_thuc;
            luong_chinh_thuc_text = _job.luong_chinh_thuc_text;
            nguoi_lien_he = _job.nguoi_lien_he;
            cong_ty = _job.cong_ty;
            don_gia = _job.don_gia;
            co_hop_dong = _job.co_hop_dong;
            thoi_han_bao_hanh = _job.thoi_han_bao_hanh;
            so_lan_doi_toi_da = _job.so_lan_doi_toi_da;
            dat_coc = _job.dat_coc;
            tien_coc = _job.tien_coc;
            ghi_chu = _job.ghi_chu;
            noi_dung = _job.noi_dung;
            don_gia_theo_luong = _job.don_gia_theo_luong;
            link_job = _job.link_job;
            link_job_upload = _job.link_job_upload;
            dia_chi = _job.dia_chi;
            thuoc_tinh = _thuoc_tinh.Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type });
            ngay_sua = _job.ngay_sua;
            ngay_tao = _job.ngay_tao;
            nguoi_tao = _job.nguoi_tao;
            nguoi_sua = _job.nguoi_sua;
            tien_coc_tra_lai = _job.tien_coc_tra_lai;
            auto_id = _job.auto_id;
            id_auto = _job.id_auto;
            owner = _job.owner;
            ngay_nhan_hd = _job.ngay_nhan_hd;
            do_uu_tien = _job.do_uu_tien;
        }
        public string token { get; set; }
        public string id_nha_tuyen_dung { get; set; }
        public string id { get; set; }
        public string id_auto { get; set; }
        public string owner { get; set; }
        public int auto_id { get; set; }
        public string link_job_upload { get; set; }
        public string id_job { get; set; }
        public string chuc_danh { get; set; }
        public List<TinhThanh> tinh_thanh { get; set; }
        public long so_luong { get; set; }
        public double luong_chinh_thuc { get; set; }
        public string luong_chinh_thuc_text { get; set; }
        public string nguoi_lien_he { get; set; }
        public CongTy cong_ty { get; set; }
        public double don_gia { get; set; }
        public bool co_hop_dong { get; set; }
        public long thoi_han_bao_hanh { get; set; }
        public long so_lan_doi_toi_da { get; set; }
        public bool dat_coc { get; set; }
        public double tien_coc { get; set; }
        public string ghi_chu { get; set; }
        public string noi_dung { get; set; }
        public string link_job { get; set; }
        public string dia_chi { get; set; }
        public dynamic thuoc_tinh { get; set; }
        public bool don_gia_theo_luong { get; set; }
        public double tien_coc_tra_lai { get; set; }
        /// <summary>
        /// Nếu là Job được gán cho để tìm ứng viên thì SET vào trường này
        /// </summary>
        public bool is_user_job { get; set; }
        public string id_user_job { get; set; }
        public bool is_owner { get; set; }
        public long ngay_nhan_hd { get; set; }
        public dynamic ung_vien_thong_ke { get; set; }
        public long so_luong_user_job { get; set; }
        public int do_uu_tien { get; set; }
        public int ung_vien_da_gui { get; set; }
        public bool ntd_da_xem { get; set; }
        public bool ntd_da_phan_hoi { get; set; }

    }
}
