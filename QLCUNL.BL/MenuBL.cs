using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
namespace QLCUNL.BL
{
    public class MenuBL
    {
        public static void GetAll()
        {
            MenuRepository.Instance.GetAll();
        }

        public static IEnumerable<Menu> GetAll(string app_id, int page_index, int page_size)
        {
            return MenuRepository.Instance.GetAll(app_id, page_index, page_size);
        }

        public static bool Index(Menu data)
        {
            return MenuRepository.Instance.Index(data);
        }
        public static Menu GetById(string id)
        {
            return MenuRepository.Instance.GetById(id);
        }
        public static bool Update(Menu data)
        {
            return MenuRepository.Instance.Update(data);
        }

        public static List<Menu> SearchAll(string app_id, string nguoi_tao, int group, int page,
            out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return MenuRepository.Instance.SearchAll(app_id, nguoi_tao, group, page, out total_recs, out msg, page_size = 50, is_admin = false);
        }
        public static List<Menu> Search(string app_id, string nguoi_tao, int group, string term, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return MenuRepository.Instance.Search(app_id, nguoi_tao, group, term, page, out total_recs, out msg, page_size, is_admin);
        }
        public static bool Delete(string id)
        {
            return MenuRepository.Instance.Delete(id);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return MenuRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return MenuRepository.Instance.RemoveDataByAppId<Menu>(app_id);
        }
    }
}
