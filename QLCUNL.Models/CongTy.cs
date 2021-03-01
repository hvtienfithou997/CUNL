using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class CongTy : BaseET
    {
        public int auto_id { get; set; }
        [Keyword]
        public string id_cong_ty { get; set; }
        public string ten_cong_ty { get; set; }
        
        public string so_dkkd { get; set; }

        public string dien_thoai { get; set; }
        public string dia_chi { get; set; }
        public string website { get; set; }

        public string ghi_chu { get; set; }

        public List<int> thuoc_tinh { get; set; }
        public string info_tax { get; set; }
        /// <summary>
        /// Thông tin gửi hợp đồng
        /// </summary>
        public string info_gui_hop_dong { get; set; }
        public string ma_so_thue { get; set; }
        /// <summary>
        /// Người liên hệ:         Chức vụ Người liên hệ Email người liên hệ   SDT người liên hệ Zalo Skype Facebook"
        /// </summary>
        public List<NguoiLienHe> lien_he { get; set; }
        /// <summary>
        /// Tài khoản ngân hàng phía công ty: cái này dành cho trường hợp hợp đồng thực hiện không thành công phải trả lại tiền
        /// </summary>
        public string tai_khoan_ngan_hang { get; set; }
    }
}
