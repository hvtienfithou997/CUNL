using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class PhanQuyenMapSimple
    {
        public PhanQuyenRule rule { get; set; }
        public PhanQuyenType type { get; set; }

        public string user { get; set; }
        public PhanQuyenMapSimple(PhanQuyen pq)
        {
            rule = pq.rule;
            type = pq.type;
            user = pq.user;
        }

    }
}
