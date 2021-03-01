using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QLCUNL.Models
{
    public class UngVien : BaseET
    {
        [Keyword]
        [DisplayName("Ứng viên ID")]
        public string id_ung_vien { get; set; }
        [DisplayName("Họ và tên")]
        public string ho_ten_ung_vien { get; set; }
        [DisplayName("Số điện thoại")]
        public string so_dien_thoai { get; set; }        
        [DisplayName("Email")]
        public string email { get; set; }
        [DisplayName("Địa chỉ")]
        public string dia_chi { get; set; }
        [DisplayName("Vị trí ứng tuyển")]
        public string vi_tri_ung_tuyen { get; set; }
        [DisplayName("Nội dung")]
        public string noi_dung { get; set; }
        [DisplayName("Ghi chú cuối")]
        public string ghi_chu_cuoi { get; set; }
        [DisplayName("Lương mong muốn")]
        public double luong_mong_muon { get; set; }
        [DisplayName("Đã đi làm")]
        public bool da_di_lam { get; set; }
        [DisplayName("Ngày đi làm")]
        public long ngay_di_lam { get; set; }        
        public List<int> thuoc_tinh { get; set; }
        [DisplayName("Zalo cá nhân")]
        public string zalo { get; set; }
        [DisplayName("Đường dẫn CV")]
        public string link_cv { get; set; }
        [DisplayName("Ngày tương tác cuối")]
        public long ngay_tuong_tac_cuoi { get; set; }
      
    }
}
