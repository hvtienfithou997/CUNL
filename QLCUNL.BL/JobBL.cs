using ESUtil;
using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Job = QLCUNL.Models.Job;

namespace QLCUNL.BL
{
    public class JobBL
    {
        public static void GetAll()
        {
            JobRepository.Instance.GetAll();
        }
        public static string Index(Job data, out string auto_id)
        {
            return JobRepository.Instance.Index(data, out auto_id);
        }
        public static Job GetById(string id)
        {
            return JobRepository.Instance.GetById(id);
        }
        public static bool Update(Job data)
        {
            return JobRepository.Instance.Update(data);
        }
        public static List<Job> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from, long ngay_di_lam_to,
           double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
           long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null)
        {
            return JobRepository.Instance.Search(app_id, nguoi_tao, group, term, id_ung_vien, id_cong_ty, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, thuoc_tinh, lst_id, lst_should_id,
            ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size, is_admin, fields);
        }

        public static List<Job> SearchDefault(string app_id, string nguoi_tao, int group, string value_filter, long ngay_nhan_hd, long ngay_tao, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from, long ngay_di_lam_to,
           double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<int> thuoc_tinh2, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
           long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null, string op = "0", string op2 = "0", Dictionary<string, bool> sort_order = null)
        {
            return JobRepository.Instance.SearchDefault(app_id, nguoi_tao, group, value_filter, ngay_nhan_hd, ngay_tao, term, id_ung_vien, id_cong_ty, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, thuoc_tinh, thuoc_tinh2, lst_id, lst_should_id,
            ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size, is_admin, fields, op, op2, sort_order);
        }

        public static List<Job> Search(string app_id, int page, out long total_recs, out string msg, int page_size = 50)
        {
            return JobRepository.Instance.Search(app_id, page, out total_recs, out msg, page_size);
        }
        public static List<Job> GetMany(string app_id, IEnumerable<string> lst_id, string[] fields)
        {
            return JobRepository.Instance.GetMany(app_id, lst_id, fields);
        }
        public static List<Job> GetMany(IEnumerable<string> lst_id)
        {
            return JobRepository.Instance.GetMany(lst_id);
        }
        public static bool Delete(string id)
        {
            return JobRepository.Instance.Delete(id);
        }
        public static bool DelUp(string id)
        {
            return JobRepository.Instance.DelUp(id);
        }
        public static bool IsOwner(string id, string owner)
        {
            return JobRepository.Instance.IsOwner(id, owner);
        }
        public static Job GetByAutoID(string app_id, string id_auto)
        {
            return JobRepository.Instance.GetByAutoID(app_id, id_auto);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return JobRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return JobRepository.Instance.RemoveDataByAppId<Job>(app_id);
        }
        public static async Task<List<string>> GetListIdCongTyByIdJob(string app_id, IEnumerable<string> ids_job)
        {
            return await JobRepository.Instance.GetListIdCongTyByIdJob(app_id, ids_job);
        }
        public static Tuple<Dictionary<int, long>, List<string>> ThongKeTheoThuocTinh(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, string id_cong_ty, long ngay_di_lam_from, long ngay_di_lam_to,
           double don_gia_from, double don_gia_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, IEnumerable<string> lst_should_id,
           long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null, string op = "0", IEnumerable<int> thuoc_tinh2 = null, string op2 = "0")
        {
            return JobRepository.Instance.ThongKeTheoThuocTinh(app_id, nguoi_tao, group, term, id_ung_vien, id_cong_ty, ngay_di_lam_from, ngay_di_lam_to, don_gia_from, don_gia_to, thuoc_tinh, lst_id, lst_should_id,
            ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size, is_admin, fields, op, thuoc_tinh2, op2);
        }
        public static bool IsIdAutoExist(string id_auto, string app_id)
        {
            return JobRepository.Instance.IsIdAutoExist(id_auto, app_id);
        }
        public static void MoveData()
        {
            JobRepository.Instance.MoveData();
        }
        public static void UpdateAutoId(string app_id, string auto_id_from, string auto_id_to)
        {
            JobRepository.Instance.UpdateAutoId(app_id, auto_id_from, auto_id_to);
        }
        public static List<string> GetIdDeleted(string app_id)
        {
            return JobRepository.Instance.GetIdDeleted(app_id);
        }
        public static List<Job> GetAllJobsIsOwner(string app_id, string nguoi_tao, string[] fields = null, bool is_admin = false)
        {
            return JobRepository.Instance.GetAllJobsIsOwner(app_id, nguoi_tao, fields, is_admin);
        }
    }
}
