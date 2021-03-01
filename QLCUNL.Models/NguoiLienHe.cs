using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class NguoiLienHe
    {
        [Text]
        public string chuc_vu { get; set; }
        [Keyword]
        public string email { get; set; }
        [Keyword]
        public string sdt { get; set; }
        [Keyword]
        public string zalo { get; set; }
        [Keyword]
        public string skype { get; set; }
        [Keyword]
        public string facebook { get; set; }
    }
}
