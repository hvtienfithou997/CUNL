using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class NoteUngVienJobBL
    {
        public static void GetAll()
        {
            NoteUngVienJobRepository.Instance.GetAll();
        }
        public static string Index(NoteUngVienJob data)
        {
            return NoteUngVienJobRepository.Instance.Index(data);
        }
        public static int IndexMany(List<NoteUngVienJob> data)
        {
            return NoteUngVienJobRepository.Instance.IndexMany(data);
        }
        public static NoteUngVienJob GetById(string id)
        {
            return NoteUngVienJobRepository.Instance.GetById(id);
        }
        public static bool Update(NoteUngVienJob data)
        {
            return NoteUngVienJobRepository.Instance.Update(data);
        }
        public static bool UpdateThuocTinh(NoteUngVienJob data)
        {
            return NoteUngVienJobRepository.Instance.UpdateThuocTinh(data);
        }
        public static bool UpdateGioPhongVan(NoteUngVienJob data)
        {
            return NoteUngVienJobRepository.Instance.UpdateGioPhongVan(data);
        }
        public static List<NoteUngVienJob> Search(string app_id, string nguoi_tao, int group, string term, string id_user, string id_job, string id_ung_vien,
           long ngay_gio_phong_van_from, long ngay_gio_phong_van_to, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_thu_viec_from, long luong_thu_viec_to, double luong_chinh_thuc_from, long luong_chinh_thuc_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
           int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null)
        {
            return NoteUngVienJobRepository.Instance.Search(app_id, nguoi_tao, group, term, id_user, id_job, id_ung_vien, ngay_gio_phong_van_from, ngay_gio_phong_van_to, ngay_di_lam_from, ngay_di_lam_to,
            luong_thu_viec_from, luong_thu_viec_to, luong_chinh_thuc_from, luong_chinh_thuc_to, thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, is_admin, fields).Item1;
        }
        public static Tuple<List<NoteUngVienJob>, Dictionary<int, long>> Search(string app_id, string nguoi_tao, int group, string term, string id_user, string id_job, string id_ung_vien,
           long ngay_gio_phong_van_from, long ngay_gio_phong_van_to, long ngay_di_lam_from, long ngay_di_lam_to,
           double luong_thu_viec_from, long luong_thu_viec_to, double luong_chinh_thuc_from, long luong_chinh_thuc_to, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
           int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false, string[] fields = null, bool thong_ke = true)
        {
            return NoteUngVienJobRepository.Instance.Search(app_id, nguoi_tao, group, term, id_user, id_job, id_ung_vien, ngay_gio_phong_van_from, ngay_gio_phong_van_to, ngay_di_lam_from, ngay_di_lam_to,
            luong_thu_viec_from, luong_thu_viec_to, luong_chinh_thuc_from, luong_chinh_thuc_to, thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, is_admin, fields, thong_ke);
        }
        public static List<string> GetListIdUngVienByIdUserJob(string app_id, string id_user_job, int page, out long total_recs, out string msg, int page_size, bool is_admin = false)
        {
            return NoteUngVienJobRepository.Instance.GetListIdUngVienByIdUserJob(app_id, id_user_job, page, out total_recs, out msg, page_size, is_admin);
        }
        public static Dictionary<string, int> GetListIdUngVienByIdUserJobNew(string app_id, IEnumerable<string> id_user_job)
        {
            return NoteUngVienJobRepository.Instance.GetListIdUngVienByIdUserJobNew(app_id, id_user_job);
        }
        public static List<NoteUngVienJob> GetNoteUngVienJobByIdUserJob(string app_id, IEnumerable<string> id_user_job, IEnumerable<int> thuoc_tinh, int page, out long total_recs, out string msg, int page_size, bool is_admin)
        {
            return NoteUngVienJobRepository.Instance.GetNoteUngVienJobByIdUserJob(app_id, id_user_job, thuoc_tinh, page, out total_recs, out msg, page_size, is_admin);
        }
        public static List<NoteUngVienJob> GetNoteUngVienJobByIdUngVien(string app_id, IEnumerable<string> id_ung_vien, string id_job, int page, out long total_recs, out string msg, int page_size, bool is_admin)
        {
            return NoteUngVienJobRepository.Instance.GetNoteUngVienJobByIdUngVien(app_id, id_ung_vien, id_job, page, out total_recs, out msg, page_size, is_admin);
        }
        public static List<NoteUngVienJob> GetNoteUngVienByIdUngVien(string app_id, IEnumerable<string> id_ung_vien, int page, out long total_recs, out string msg, int page_size, bool is_admin)
        {
            return NoteUngVienJobRepository.Instance.GetNoteUngVienByIdUngVien(app_id, id_ung_vien, page, out total_recs, out msg, page_size, is_admin);
        }
        public static List<NoteUngVienJob> Searchbyidungvien(string app_id, string id_ung_vien, string user, out long total_recs, out string msg)
        {
            return NoteUngVienJobRepository.Instance.Searchbyidungvien(app_id, id_ung_vien, user, out total_recs, out msg);
        }
        public static bool Delete(string id)
        {
            return NoteUngVienJobRepository.Instance.Delete(id);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return NoteUngVienJobRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return NoteUngVienJobRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static List<NoteUngVienJob> GetNoteUngVienJobByIdJobOwner(string app_id, string id_job)
        {
            return NoteUngVienJobRepository.Instance.GetNoteUngVienJobByIdJobOwner(app_id, id_job);
        }
        public static bool IsNoteUngVienJobExistInJob(string app_id, string id_job, string id_ung_vien)
        {
            return NoteUngVienJobRepository.Instance.IsNoteUngVienJobExistInJob(app_id, id_job, id_ung_vien);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return NoteUngVienJobRepository.Instance.RemoveDataByAppId<NoteUngVienJob>(app_id);
        }
        public static Dictionary<string, Dictionary<int, long>> ThongKeTrangThaiUngVien(string app_id, string nguoi_tao, List<int> trang_thai_can_thong_ke, IEnumerable<string> ids_job,
            IEnumerable<string> ids_job_owner, bool is_admin)
        {
            if (trang_thai_can_thong_ke == null)
            {
                trang_thai_can_thong_ke = new List<int>();
            }
            if (ids_job == null)
            {
                ids_job = new List<string>();
            }
            if (ids_job_owner == null)
            {
                ids_job_owner = new List<string>();
            }
            return NoteUngVienJobRepository.Instance.ThongKeTrangThaiUngVien(app_id, nguoi_tao, trang_thai_can_thong_ke, ids_job, ids_job_owner, is_admin);
        }
        public static Dictionary<string, long> ThongKeUngVienTheoJob(string app_id, IEnumerable<string> ids_ung_vien)
        {
            if (ids_ung_vien == null)
                ids_ung_vien = new List<string>();
            return NoteUngVienJobRepository.Instance.ThongKeUngVienTheoJob(app_id, ids_ung_vien);
        }
        public static List<NoteUngVienJob> GetMany(IEnumerable<string> lst_id)
        {
            return NoteUngVienJobRepository.Instance.GetMany(lst_id);
        }
        public static bool UpdateGhiChuNhaTuyenDung(string id, string ghi_chu_nha_tuyen_dung, out string msg)
        {
            return NoteUngVienJobRepository.Instance.UpdateGhiChuNhaTuyenDung(id, ghi_chu_nha_tuyen_dung, out msg);
        }
        public static bool UpdateGhiChuUngVien(string id, string ghi_chu_ung_vien, out string msg)
        {
            return NoteUngVienJobRepository.Instance.UpdateGhiChuUngVien(id, ghi_chu_ung_vien, out msg);
        }
    }
}
