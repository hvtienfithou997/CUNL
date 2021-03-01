using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class UserJob : BaseET
    {
        [Keyword]
        public string id_user_job { get; set; }
        [Keyword]
        public string id_job { get; set; }
        [Keyword]
        public string id_user { get; set; }
        public long ngay_nhan_job { get; set; }
        public List<int> thuoc_tinh { get; set; }
    }
}
