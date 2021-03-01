using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QLCUNL.Models
{
    public class ThuocTinh : BaseET
    {
        [Keyword]
        public string id { get; set; }
        [Keyword]
        public string user_name { get; set; }
        [DisplayName("Loại")]
        public LoaiThuocTinh loai { get; set; }
        [DisplayName("Giá trị")]
        public int gia_tri { get; set; }
        [DisplayName("Tên")]
        public string ten { get; set; }
        [DisplayName("Nhóm")]
        public int nhom { get; set; }
        [DisplayName("Loại thuộc tính")]
        public ThuocTinhType type { get; set; }
        public ThuocTinh()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
    }
}
