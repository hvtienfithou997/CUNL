using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
namespace QLCUNL.BL
{
    public class CongTyBL
    {
        public static void GetAll()
        {
            CongTyRepository.Instance.GetAll();
        }

        public static IEnumerable<CongTy> GetAll(string app_id, string term, int page_index, int page_size)
        {
            return CongTyRepository.Instance.GetAll(app_id, term, page_index, page_size);
        }

        public static string Index(CongTy data)
        {
            return CongTyRepository.Instance.Index(data);
        }
        public static bool IndexCongTy(CongTy cong_ty, out string id_cong_ty)
        {
            return CongTyRepository.Instance.IndexCongTy(cong_ty, out id_cong_ty);
        }

        public static CongTy GetById(string id)
        {
            return CongTyRepository.Instance.GetById(id);
        }
        public static bool Update(CongTy data)
        {
            return CongTyRepository.Instance.Update(data);
        }

        public static List<CongTy> SearchAll(string app_id, string nguoi_tao, int group, int page,
            out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return CongTyRepository.Instance.SearchAll(app_id, nguoi_tao, group, page, out total_recs, out msg, page_size = 50, is_admin = false);
        }
        public static List<CongTy> Search(string app_id, string nguoi_tao, int group, string term, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, IEnumerable<string> lst_id_should, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return CongTyRepository.Instance.Search(app_id, nguoi_tao, group, term, thuoc_tinh, lst_id, lst_id_should, page, out total_recs, out msg, page_size, is_admin).Item1;
        }
        public static Tuple<List<CongTy>, Dictionary<int, long>> Search(string app_id, string nguoi_tao, int group, string term, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
            IEnumerable<string> lst_id_should, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, bool thong_ke = true)
        {
            return CongTyRepository.Instance.Search(app_id, nguoi_tao, group, term, thuoc_tinh, lst_id, lst_id_should, page, out total_recs, out msg, page_size, is_admin, thong_ke);
        }
        public static bool Delete(string id)
        {
            return CongTyRepository.Instance.Delete(id);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return CongTyRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static List<CongTy> GetMany(IEnumerable<string> lst_id)
        {
            return CongTyRepository.Instance.GetMany(lst_id);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return CongTyRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return CongTyRepository.Instance.RemoveDataByAppId<CongTy>(app_id);
        }
        public static bool IsExistTenCongTy(string app_id, string ten_cong_ty)
        {
            return CongTyRepository.Instance.IsExistTenCongTy(app_id, ten_cong_ty);
        }
        public static CongTy GetCongTyByName(string app_id, string ten_cong_ty)
        {
            return CongTyRepository.Instance.GetCongTyByName(app_id, ten_cong_ty);
        }
        public static CongTy GetCongTyByNameV2(string app_id, string ten_cong_ty)
        {
            return CongTyRepository.Instance.GetCongTyByNameV2(app_id, ten_cong_ty);
        }
        public static int IndexMany(List<CongTy> data)
        {
            return CongTyRepository.Instance.IndexMany(data);
        }
    }
}
