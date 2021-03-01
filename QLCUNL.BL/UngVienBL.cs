using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class UngVienBL
    {
        public static IEnumerable<UngVien> GetAll(int page_index, int page_size)
        {
            return UngVienRepository.Instance.GetAll();
        }
        public static bool Index(UngVien data)
        {
            return UngVienRepository.Instance.Index(data);
        }
        public static string IndexRetId(UngVien data)
        {
            return UngVienRepository.Instance.IndexRetId(data);
        }
        public static UngVien GetById(string id)
        {
            return UngVienRepository.Instance.GetById(id);
        }
        public static bool Update(UngVien data)
        {
            return UngVienRepository.Instance.Update(data);
        }
        public static List<UngVien> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_mong_muon_from, double luong_mong_muon_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
           long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, IEnumerable<string> lst_id_neq = null,
           Dictionary<string, bool> sort_order = null)
        {
            return UngVienRepository.Instance.Search(app_id, nguoi_tao, group, term, id_ung_vien, ngay_di_lam_from, ngay_di_lam_to,
                 luong_mong_muon_from, luong_mong_muon_to, thuoc_tinh, lst_id, ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size, is_admin, lst_id_neq, sort_order, false).Item1;
        }
        public static Tuple<List<UngVien>, Dictionary<int, long>> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_mong_muon_from, double luong_mong_muon_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
           long ngay_tao_from, long ngay_tao_to, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, IEnumerable<string> lst_id_neq = null,
           Dictionary<string, bool> sort_order = null, bool thong_ke = true)
        {
            return UngVienRepository.Instance.Search(app_id, nguoi_tao, group, term, id_ung_vien, ngay_di_lam_from, ngay_di_lam_to,
                 luong_mong_muon_from, luong_mong_muon_to, thuoc_tinh, lst_id, ngay_tao_from, ngay_tao_to, page, out total_recs, out msg, page_size, is_admin, lst_id_neq, sort_order, thong_ke);
        }
        public static List<UngVien> Search(string app_id, int page, out long total_recs, out string msg, int page_size = 9999)
        {
            return UngVienRepository.Instance.Search(app_id, page, out total_recs, out msg, page_size);
        }
        public static List<UngVien> GetMany(IEnumerable<string> lst_id)
        {
            return UngVienRepository.Instance.GetMany(lst_id);
        }
        public static bool Delete(string id)
        {
            return UngVienRepository.Instance.Delete(id);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return UngVienRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool CanEdit(string id, string nguoi_tao, int group)
        {
            return UngVienRepository.Instance.CanEdit(id, nguoi_tao, group);
        }
        public static bool CanView(string id, string nguoi_tao, int group)
        {
            return UngVienRepository.Instance.CanView(id, nguoi_tao, group);
        }
        public static bool CanDelete(string id, string nguoi_tao, int group)
        {
            return UngVienRepository.Instance.CanDelete(id, nguoi_tao, group);
        }
        public static bool SetThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UngVienRepository.Instance.SetThuocTinh(id, thuoc_tinh);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return UngVienRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return UngVienRepository.Instance.RemoveDataByAppId<UngVien>(app_id);
        }
    }
}
