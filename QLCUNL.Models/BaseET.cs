using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QLCUNL.Models
{
    public class BaseET
    {
        [DisplayName("Ngày tạo")]
        public long ngay_tao { get; set; }
        [DisplayName("Người tạo")]
        [Keyword]
        public string nguoi_tao { get; set; }
        [DisplayName("Ngày sửa")]
        public long ngay_sua { get; set; }
        [DisplayName("Người sửa")]
        [Keyword]
        public string nguoi_sua { get; set; }
        [Keyword]
        public string app_id { get; set; }
        public TrangThai trang_thai { get; set; }
    }

    public class BaseGetAll
    {
        public string term { get; set; } = "";
        public int page_index { get; set; } = 1;
        public int page_size { get; set; } = 10;

    }
}
