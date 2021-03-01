using Nest;
using System;
using System.Collections.Generic;

namespace QLCUNL.Models
{
    public class Job : BaseET
    {
        [Keyword]
        public string id_job { get; set; }
        public string chuc_danh { get; set; }
        public List<TinhThanh> tinh_thanh { get; set; }
        public long so_luong { get; set; }
        public double luong_chinh_thuc { get; set; }
        public string luong_chinh_thuc_text { get; set; }
        public string nguoi_lien_he { get; set; }
        public CongTy cong_ty { get; set; }
        public double don_gia { get; set; }
        public bool don_gia_theo_luong { get; set; }
        public bool co_hop_dong { get; set; }
        public long thoi_han_bao_hanh { get; set; }
        public long so_lan_doi_toi_da { get; set; }
        public bool dat_coc { get; set; }
        public double tien_coc { get; set; }
        public string ghi_chu { get; set; }
        public string noi_dung { get; set; }
        public string link_job { get; set; }
        public string link_job_upload { get; set; }
        public string dia_chi { get; set; }
        public List<int> thuoc_tinh { get; set; }
        public int auto_id { get; set; }
        public string id_auto { get; set; }
        [Keyword]
        public string owner { get; set; }
        public double tien_coc_tra_lai { get; set; }

        public long ngay_nhan_hd { get; set; }
        public int do_uu_tien { get; set; }
    }

    
}
