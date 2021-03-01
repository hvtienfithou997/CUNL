using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using XMedia;
namespace QLCUNL.Models
{
    public class GroupUser : BaseET
    {
        public GroupUser()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
        [Keyword]
        public long id_team { get; set; }        
        public string team_name { get; set; }
        public string id { get; set; }
    }
}
