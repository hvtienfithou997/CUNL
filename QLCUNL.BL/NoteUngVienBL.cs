using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class NoteUngVienBL
    {
        public static IEnumerable<NoteUngVien> GetAll(int page_index, int page_size)
        {
            return NoteUngVienRepository.Instance.GetAll();
        }
        public static bool Index(NoteUngVien data)
        {
            return NoteUngVienRepository.Instance.Index(data);
        }
        public static NoteUngVien GetById(string id)
        {
            return NoteUngVienRepository.Instance.GetById(id);
        }
        public static bool Update(NoteUngVien data)
        {
            return NoteUngVienRepository.Instance.Update(data);
        }
        public static List<NoteUngVien> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return NoteUngVienRepository.Instance.Search(app_id, nguoi_tao, group, term, id_ung_vien, thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, is_admin);
        }
        public static bool IsOwner(string id,string nguoi_tao)
        {
            return NoteUngVienRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool Delete(string id)
        {
            return NoteUngVienRepository.Instance.Delete(id);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return NoteUngVienRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return NoteUngVienRepository.Instance.RemoveDataByAppId<NoteUngVien>(app_id);
        }
    }
}
