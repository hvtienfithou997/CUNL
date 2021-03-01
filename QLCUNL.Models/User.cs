using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using XMedia;
namespace QLCUNL.Models
{
    public class User : BaseET
    {
        public User()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
        [Keyword]
        public string id_user { get; set; }
        [Keyword]
        public string user_name { get; set; }
        public string full_name { get; set; }
        public long id_team { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string browser { get; set; }
        public UserType type { get; set; }
        public long last_login { get; set; }
        public string old_password { get; set; }
        [Keyword]
        public List<string> roles { get; set; }
        public string default_settings { get; set; }
    }
    public class UserSetting
    {
        public List<int> trang_thai_thong_ke_ung_vien_job { get; set; }
        public int trang_thai_user_job_bao_cao { get; set; }
        public bool tim_ung_vien_team_khac { get; set; }
        public double tien_coc { get; set; }
        public int bao_hanh { get; set; }
        public int so_lan_doi { get; set; }
    }
}
