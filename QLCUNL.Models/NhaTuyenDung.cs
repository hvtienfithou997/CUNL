using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class NhaTuyenDung : BaseET
    {
        [Keyword]
        public string id { get; set; }
        [Keyword]
        public string id_nha_tuyen_dung { get; set; }
        [Keyword]
        public string id_job { get; set; }
        [Keyword]
        public string id_user_job { get; set; }
        public long ngay_xem { get; set; }
        [Ip]
        public string ip { get; set; }
        [Text]
        public string browser { get; set; }
        public string noi_dung { get; set; }
        [Text(Index = false)]
        public string token { get; set; }
        public List<string> lst_id_share { get; set; }

    }
}
