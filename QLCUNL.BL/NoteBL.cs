using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class NoteBL
    {
        public static IEnumerable<NoteUngVien> GetAll(int page_index, int page_size)
        {
            return NoteRepository.Instance.GetAll();
        }
        public static bool Index(Note data)
        {
            return NoteRepository.Instance.Index(data);
        }
     
        public static Note GetById(string id)
        {
            return NoteRepository.Instance.GetById(id);
        }
        public static bool Update(Note data)
        {
            return NoteRepository.Instance.Update(data);
        }
        public static List<Note> Search(string app_id, string nguoi_tao, int group, string term, string id_ung_vien, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return NoteRepository.Instance.Search(app_id, nguoi_tao, group, term, id_ung_vien, thuoc_tinh, lst_id, page, out total_recs, out msg, page_size, is_admin);
        }

        public static List<Note> Search(IEnumerable<int> thuoc_tinh, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.Search(thuoc_tinh, out total_recs, out msg, page_size);
        }
        public static List<Note> GetAllLogNhaTuyenDung(string id_obj, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.GetAllLogNhaTuyenDung(id_obj, out total_recs, out msg, page_size);
        }
        public static List<Note> GetAllNhaTuyenDungXemCvUngVien(IEnumerable<string> id_obj, string nguoi_tao, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.GetAllNhaTuyenDungXemCvUngVien(id_obj, nguoi_tao, out total_recs, out msg, page_size);
        } 
        public static List<Note> GetLogXemCvTuyenDung(IEnumerable<string> id_obj, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.GetLogXemCvTuyenDung(id_obj, out total_recs, out msg, page_size);
        }
        public static List<Note> NhaTuyenDungNoteUngVien(IEnumerable<string> id_obj, string nguoi_tao, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.NhaTuyenDungNoteUngVien(id_obj, nguoi_tao, out total_recs, out msg, page_size);
        }
        public static List<Note> GetListNoteByIdObj(IEnumerable<string> id_obj, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.GetListNoteByIdObj(id_obj, out total_recs, out msg, page_size);
        }
        public static int IndexMany(List<Note> data)
        {
            return NoteRepository.Instance.IndexMany(data);
        }
        public static List<Note> GetNoteByObject(string app_id,IEnumerable<string> id_obj,LoaiNote loai, LoaiDuLieu loai_du_lieu, string nguoi_tao, out long total_recs, out string msg, int page_size = 50)
        {
            return NoteRepository.Instance.GetNoteByObject(app_id, id_obj, loai, loai_du_lieu, nguoi_tao, out total_recs, out msg, page_size);
        }
        public static Note GetNoteByIdObj(string id_obj)
        {
            return NoteRepository.Instance.GetNoteByIdObj(id_obj);
        }
        public static bool IsOwner(string id,string nguoi_tao)
        {
            return NoteRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool Delete(string id)
        {
            return NoteRepository.Instance.Delete(id);
        }
        public static bool UpdateThuocTinh(string id, List<int> thuoc_tinh)
        {
            return NoteRepository.Instance.UpdateThuocTinh(id, thuoc_tinh);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return NoteRepository.Instance.RemoveDataByAppId<NoteUngVien>(app_id);
        }
    }
}
