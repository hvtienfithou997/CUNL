using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class UserBL
    {
        public static IEnumerable<User> GetByTeam()
        {
            return UserRepository.Instance.GetByTeam();
        }
        public static IEnumerable<User> GetAll(string term, string[] fields)
        {
            return UserRepository.Instance.GetAll(term, fields);
        }
        public static bool Index(User data, out string msg)
        {
            return UserRepository.Instance.Index(data, out msg);
        }
        public static User GetById(string id)
        {
            return UserRepository.Instance.GetById(id);
        }
        public static User GetByUserName(string username)
        {
            return UserRepository.Instance.GetByUserName(username);
        }
        public static bool IsSysAdmin(string id)
        {
            return UserRepository.Instance.IsSysAdmin(id);
        }
        public static bool Update(User data)
        {
            return UserRepository.Instance.Update(data);
        }
        public static bool Delete(string id)
        {
            return UserRepository.Instance.Delete(id);
        }
        public static bool UserEditInfo(string id, string full_name, out string msg)
        {
            return UserRepository.Instance.UserEditInfo(id, full_name, out msg);
        }
        public static bool UpdatePassWord(string id, string password, string old_password, out string msg)
        {
            return UserRepository.Instance.UpdatePassWord(id, password, old_password, out msg);
        }
        public static bool ResetPassWord(string id, string password, out string msg, bool is_admin = false)
        {
            return UserRepository.Instance.ResetPassWord(id, password, out msg, is_admin);
        }
        public static List<User> Search(string app_id, string term,string id_team, int page, out long total_recs, out string msg, int page_size = 50)
        {
            page_size = page_size > 500 ? 500 : page_size;
            page = page < 1 ? 1 : page;
            return UserRepository.Instance.Search(app_id, term, id_team, page, out total_recs, out msg, page_size);
        }
        public static User Login(string user_name, string password, string ip, string browser)
        {
            return UserRepository.Instance.Login(user_name, password, ip, browser);
        }
        public static List<string> GetRoles(string user_name)
        {
            return UserRepository.Instance.GetRoles(user_name);
        }
        public static List<User> GetAllUserNameByTeam(string app_id, int id_team, string[] fields = null, bool is_admin=false)
        {
            return UserRepository.Instance.GetAllUserNameByTeam(app_id, id_team, fields, is_admin);
        }
        public static List<User> GetAllUserNameByAppId(string app_id, string[] fields = null, bool is_admin = false)
        {
            return UserRepository.Instance.GetAllUserNameByAppId(app_id,  fields, is_admin);
        }
        
        public static UserSetting GetDefaultSettingByAppId(string app_id)
        {
            return UserRepository.Instance.GetDefaultSettingByAppId(app_id);
        }
        public static void UpdateDefaultSettingByAppId(string app_id, string setting)
        {
            UserRepository.Instance.UpdateDefaultSettingByAppId(app_id, setting);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return UserRepository.Instance.RemoveDataByAppId<User>(app_id);
        }
    }
}
