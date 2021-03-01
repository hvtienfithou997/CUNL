using ESUtil;
using QLCUNL.Models;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class NhaTuyenDungBL
    {
        public static void GetAll()
        {
            NhaTuyenDungRepository.Instance.GetAll();
        }

        public static List<NhaTuyenDung> GetMany(IEnumerable<string> lst_id)
        {
            return NhaTuyenDungRepository.Instance.GetMany(lst_id);
        }

        public static string Index(NhaTuyenDung employer, out string token)
        {
            return NhaTuyenDungRepository.Instance.Index(employer, out token);
        }

        public static bool Update(NhaTuyenDung employer)
        {
            return NhaTuyenDungRepository.Instance.Update(employer);
        }

        public static NhaTuyenDung GetById(string id)
        {
            return NhaTuyenDungRepository.Instance.GetById(id);
        }

        public static NhaTuyenDung GetByToken(string token)
        {
            return NhaTuyenDungRepository.Instance.GetByToken(token);
        }

        public static List<NhaTuyenDung> Search(string term, IEnumerable<int> thuoc_tinh, string app_id, string nguoi_tao, int page, string token, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return NhaTuyenDungRepository.Instance.Search(term, thuoc_tinh, app_id, nguoi_tao, page, token, out total_recs, out msg, page_size, is_admin);
        }

        public static List<NhaTuyenDung> GetNhaTuyenDungByLstIdShare(string app_id, IEnumerable<string> lst_id_share, int page, out long total_recs, out string msg, int page_size = 50)
        {
            return NhaTuyenDungRepository.Instance.GetNhaTuyenDungByLstIdShare(app_id, lst_id_share, page, out total_recs, out msg, page_size);
        }

        public static List<NhaTuyenDung> GetNhaTuyenDungByIdJob(string app_id, IEnumerable<string> lst_id_job, int page, out long total_recs, out string msg, int page_size = 50)
        {
            return NhaTuyenDungRepository.Instance.GetNhaTuyenDungByIdJob(app_id, lst_id_job, page, out total_recs, out msg, page_size);
        }

        public static List<NhaTuyenDung> GetNhaTuyenDungByIdObj(string app_id, IEnumerable<string> lst_id_obj, int page, out long total_recs, out string msg, int page_size = 50)
        {
            return NhaTuyenDungRepository.Instance.GetNhaTuyenDungByIdObj(app_id, lst_id_obj, page, out total_recs, out msg, page_size);
        }

        public static List<NhaTuyenDung> GetAllNhaTuyenDung(string term, IEnumerable<int> thuoc_tinh, string app_id, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin = false)
        {
            return NhaTuyenDungRepository.Instance.GetAllNhaTuyenDung(term, thuoc_tinh, app_id, page, out total_recs, out msg, page_size);
        }

        public static bool Delete(string id)
        {
            return NhaTuyenDungRepository.Instance.Delete(id);
        }

        public static NhaTuyenDung GetByIdJob(string id_job)
        {
            return NhaTuyenDungRepository.Instance.GetByIdJob(id_job);
        }

        public static List<NhaTuyenDung> GetListNtdByIdJob(string id_job)
        {
            return NhaTuyenDungRepository.Instance.GetListNtdByIdJob(id_job);
        }
    }
}