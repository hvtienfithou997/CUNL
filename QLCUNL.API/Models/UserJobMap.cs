using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class UserJobMap
    {
        
        public string id_user_job { get; set; }
        
        public string id_job { get; set; }
        public long ngay_nhan_job { get; set; }
        public dynamic thuoc_tinh { get; set; }
        public string chuc_danh { get; set; }
        public string cong_ty { get; set; }
        public long so_luong { get; set; }
        public int auto_id { get; set; }
    }
}
