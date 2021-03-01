using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using QLCUNL.Models;

namespace QLCUNL.API.Models
{
    public class UserMap
    {
        public UserMap(User data, Dictionary<long, GroupUser> dic)
        {

            this.id_user = data.id_user;
            this.id_team = data.id_team;
            this.user_name = data.user_name;
            this.full_name = data.full_name;
            this.password = data.password;
            this.last_login = data.last_login;
            this.old_password = data.old_password;
            this.email = data.email;
            
            if (dic.ContainsKey(data.id_team))
            {
                team_name = dic[data.id_team].team_name;
            }

        }
        public string team_name { get; set; }
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
}
