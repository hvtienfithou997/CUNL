using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLCUNL.Models;

namespace QLCUNL.API.Models
{
    public class UngVienMap: BaseET
    {
        public UngVienMap(UngVien item, IEnumerable<ThuocTinh> _thuoc_tinh)
        {
            this.ho_ten_ung_vien = item.ho_ten_ung_vien;
            this.so_dien_thoai = item.so_dien_thoai;
            this.dia_chi = item.dia_chi;
            this.email = item.email;
            this.vi_tri_ung_tuyen = item.vi_tri_ung_tuyen;
            this.noi_dung = item.noi_dung;
            this.ghi_chu_cuoi = item.ghi_chu_cuoi;
            this.luong_mong_muon = item.luong_mong_muon;
            this.ngay_di_lam = item.ngay_di_lam;
            this.da_di_lam = item.da_di_lam;
            this.zalo = item.zalo;
            this.link_cv = item.link_cv;
            this.ngay_tuong_tac_cuoi = item.ngay_tuong_tac_cuoi;
            thuoc_tinh = _thuoc_tinh.Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type });
            this.id_ung_vien = item.id_ung_vien;
            ngay_sua = item.ngay_sua;
            ngay_tao = item.ngay_tao;
            nguoi_tao = item.nguoi_tao;
            nguoi_sua = item.nguoi_sua;

        }

        public string id_ung_vien { get; set; }
        public string ho_ten_ung_vien { get; set; }
        public string so_dien_thoai { get; set; }
        public string email { get; set; }
        public string dia_chi { get; set; }
        public string vi_tri_ung_tuyen { get; set; }
        
        public string noi_dung { get; set; }
        
        public string ghi_chu_cuoi { get; set; }
       
        public double luong_mong_muon { get; set; }
       
        public bool da_di_lam { get; set; }
        
        public long ngay_di_lam { get; set; }

        public string zalo { get; set; }
        
        public string link_cv { get; set; }
        
        public long ngay_tuong_tac_cuoi { get; set; }
        public dynamic thuoc_tinh { get; set; }
        public string user_job_name { get; set; }
        public long so_luong_job_da_gan { get; set; }
    }
}
