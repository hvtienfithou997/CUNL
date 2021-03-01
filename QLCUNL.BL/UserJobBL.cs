using ESUtil;
using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLCUNL.BL
{
    public class UserJobBL
    {
        public static void GetAll()
        {
            UserJobRepository.Instance.GetAll();
        }
        public static bool Index(UserJob data)
        {
            return UserJobRepository.Instance.Index(data);
        }
        public static int IndexMany(List<UserJob> data)
        {
            return UserJobRepository.Instance.IndexMany(data);
        }
        public static UserJob GetById(string id)
        {
            return UserJobRepository.Instance.GetById(id);
        }
        public static bool Update(UserJob data)
        {
            return UserJobRepository.Instance.Update(data);
        }
        public static bool UpdateThuocTinh(UserJob data)
        {
            return UserJobRepository.Instance.UpdateThuocTinh(data);
        }
        public static List<UserJob> Search(string app_id, string nguoi_tao, int group, IEnumerable<string> id_job, string id_ung_vien, long ngay_nhan_job_from, long ngay_nhan_job_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
            int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return UserJobRepository.Instance.Search(app_id, nguoi_tao, group, id_job, id_ung_vien, ngay_nhan_job_from, ngay_nhan_job_to, thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, is_admin);
        }

        public static bool Delete(string id)
        {
            return UserJobRepository.Instance.Delete(id);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return UserJobRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static List<UserJob> GetByIdJob(string app_id, string id_job)
        {
            return UserJobRepository.Instance.GetByIdJob(app_id, id_job);
        }
        public static List<UserJob> GetUserJobByIdJob(string app_id, string id_job, string user)
        {
            return UserJobRepository.Instance.GetUserJobByIdJob(app_id, id_job, user);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UserJobRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return UserJobRepository.Instance.RemoveDataByAppId<UserJob>(app_id);
        }
        public static async Task<List<string>> GetListIdJobByUser(string app_id, IEnumerable<string> ids_user)
        {
            return await UserJobRepository.Instance.GetListIdJobByUser(app_id, ids_user);
        }
        public static Dictionary<string, long> ThongKeUserJob(string app_id, IEnumerable<string> ids_job)
        {
            if (ids_job == null) ids_job = new List<string>();
            return UserJobRepository.Instance.ThongKeUserJob(app_id, ids_job);
        }
        public static bool DeleteByIdUserJob(IEnumerable<string> id_user_job)
        {
            return UserJobRepository.Instance.DeleteByIdUserJob(id_user_job);
        }
    }
}
