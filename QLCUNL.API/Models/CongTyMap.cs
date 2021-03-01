using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class CongTyMap : BaseET
    {
        public CongTyMap(CongTy item, IEnumerable<ThuocTinh> _thuoc_tinh)
        {
            id_cong_ty = item.id_cong_ty;
            ten_cong_ty = item.ten_cong_ty;
            app_id = item.app_id;
            dia_chi = item.dia_chi;
            dien_thoai = item.dien_thoai;
            ghi_chu = item.ghi_chu;
            info_gui_hop_dong = item.info_gui_hop_dong;
            info_tax = item.info_tax;
            lien_he = item.lien_he;
            ma_so_thue = item.ma_so_thue;
            ngay_sua = item.ngay_sua;
            ngay_tao = item.ngay_tao;
            nguoi_sua = item.nguoi_sua;
            nguoi_tao = item.nguoi_tao;
            so_dkkd = item.so_dkkd;
            tai_khoan_ngan_hang = item.tai_khoan_ngan_hang;
            trang_thai = item.trang_thai;
            website = item.website;
            ngay_sua = item.ngay_sua;
            ngay_tao = item.ngay_tao;
            nguoi_tao = item.nguoi_tao;
            nguoi_sua = item.nguoi_sua;
            auto_id = item.auto_id;
            thuoc_tinh = _thuoc_tinh.Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type });
        }
        public dynamic thuoc_tinh { get; set; }
        public string id_cong_ty { get; set; }
        public string ten_cong_ty { get; set; }

        public string so_dkkd { get; set; }

        public string dien_thoai { get; set; }
        public string dia_chi { get; set; }
        public string website { get; set; }

        public string ghi_chu { get; set; }

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
        public int auto_id { get; set; }
    }
}
