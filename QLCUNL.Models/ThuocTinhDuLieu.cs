using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class ThuocTinhDuLieu : BaseET
    {
        [Keyword]
        public string id { get; set; }
        public LoaiThuocTinh loai_obj { get; set; }
        [Keyword]
        public string id_obj { get; set; }
        [Keyword]
        public string user_name { get; set; }
        public List<int> thuoc_tinh { get; set; }
        public ThuocTinhDuLieu()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
    }
}
