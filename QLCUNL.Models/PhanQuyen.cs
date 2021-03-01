using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class PhanQuyen : BaseET
    {
        [Keyword]
        public string id { get; set; }
        public PhanQuyenRule rule { get; set; }
        public PhanQuyenType type { get; set; }
        [Keyword]
        public string user { get; set; }
        public PhanQuyenObjType obj_type { get; set; }
        [Keyword]
        public string obj_id { get; set; }
        public List<Quyen> quyen { get; set; }
        public long ngay_het { get; set; }

        public PhanQuyen()
        {
            this.rule = PhanQuyenRule.OBJECT;
            this.type = PhanQuyenType.USERS;
            this.obj_type = PhanQuyenObjType.CONG_TY;
            this.quyen = new List<Quyen>() { Quyen.VIEW };
            this.ngay_het = XMedia.XUtil.TimeInEpoch(DateTime.Now.AddDays(7));
            
            this.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            this.nguoi_sua = this.nguoi_tao;
            this.ngay_sua = this.ngay_tao;
            
        }
        public string AutoId()
        {
            return $"{this.rule}_{this.type}_{this.user}_{this.obj_type}_{this.obj_id}_{string.Join("-", this.quyen)}";
        }
    }
}
