using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class TinhThanh
    {
        [Keyword]
        public string id_tinh { get; set; }
        public string ten_tinh { get; set; }
        public string ten_viet_tat { get; set; }
        public long stt { get; set; }
    }
}
