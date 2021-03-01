using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class GroupUserBL
    {
        public static IEnumerable<GroupUser> GetAll(string app_id, string[] fields)
        {
            return GroupUserRepository.Instance.GetAll(app_id, fields);
        }
        public static bool Index(GroupUser data, out string msg)
        {
            return GroupUserRepository.Instance.Index(data, out msg);
        }
        public static GroupUser GetById(string id)
        {
            return GroupUserRepository.Instance.GetById(id);
        }
        public static bool Update(GroupUser data)
        {
            return GroupUserRepository.Instance.Update(data);
        }
        public static List<GroupUser> Search(string app_id, string term, int page, out long total_recs, out string msg, int page_size = 50)
        {
            page_size = page_size > 500 ? 500 : page_size;
            page = page < 1 ? 1 : page;
            return GroupUserRepository.Instance.Search(app_id, term, page, out total_recs, out msg, page_size);
        }

        public static List<GroupUser> Search(string app_id, int page, out long total_recs, out string msg, int page_size = 9999)
        {
            return GroupUserRepository.Instance.Search(app_id, page, out total_recs, out msg, page_size);
        }
        public static bool Delete(string id)
        {
            return GroupUserRepository.Instance.Delete(id);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return GroupUserRepository.Instance.RemoveDataByAppId<GroupUser>(app_id);
        }
    }
}
