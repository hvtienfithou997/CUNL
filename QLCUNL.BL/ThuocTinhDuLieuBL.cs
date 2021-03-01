using ESUtil;
using Nest;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class ThuocTinhDuLieuBL
    {
        public static IEnumerable<ThuocTinhDuLieu> GetAll(int page_index, int page_size)
        {
            return ThuocTinhDuLieuRepository.Instance.GetAll();
        }
        public static bool Index(ThuocTinhDuLieu data)
        {
            return ThuocTinhDuLieuRepository.Instance.Index(data);
        }
        public static ThuocTinhDuLieu GetById(string id)
        {
            return ThuocTinhDuLieuRepository.Instance.GetById(id);
        }
        public static bool Update(ThuocTinhDuLieu data)
        {
            return ThuocTinhDuLieuRepository.Instance.Update(data);
        }
        public static List<ThuocTinhDuLieu> Search(string app_id, string nguoi_tao, LoaiThuocTinh loai, List<int> thuoc_tinh, int page, out long total_recs, out string msg, int page_size = 50, string op = "0", List<int> thuoc_tinh2 = null, string op2 = "0")
        {
            return ThuocTinhDuLieuRepository.Instance.Search(app_id, nguoi_tao, loai, thuoc_tinh, page, out total_recs, out msg, page_size, op, thuoc_tinh2, op2);
        }
        public static List<ThuocTinhDuLieu> GetThuocTinhByIdObj(string app_id, string id_obj, string nguoi_tao)
        {
            return ThuocTinhDuLieuRepository.Instance.GetThuocTinhByIdObj(app_id, id_obj, nguoi_tao);
        }
        public static IEnumerable<int> GetIdThuocTinhByIdObj(string app_id, string id_obj, string nguoi_tao)
        {
            return ThuocTinhDuLieuRepository.Instance.GetIdThuocTinhByIdObj(app_id, id_obj, nguoi_tao);
        }
        public static Dictionary<string, List<int>> GetIdThuocTinhByIdObj(string app_id, IEnumerable<string> ids_obj, string nguoi_tao)
        {
            return ThuocTinhDuLieuRepository.Instance.GetIdThuocTinhByIdObj(app_id, ids_obj, nguoi_tao);
        }
        public static IEnumerable<int> GetIdThuocTinhByIdObj(string app_id, string id_obj, LoaiThuocTinh loai_obj, string nguoi_tao)
        {
            return ThuocTinhDuLieuRepository.Instance.GetIdThuocTinhByIdObj(app_id, id_obj, loai_obj, nguoi_tao);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return ThuocTinhDuLieuRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return ThuocTinhDuLieuRepository.Instance.RemoveDataByAppId<ThuocTinhDuLieu>(app_id);
        }
        public static Dictionary<int, long> ThongKeTheoThuocTinh(string app_id, string nguoi_tao, LoaiThuocTinh loai, IEnumerable<int> thuoc_tinh, IEnumerable<string> lst_id,
            IEnumerable<string> lst_should_id, IEnumerable<string> lst_must_not_id, bool is_admin = false, string op = "0")
        {
            return ThuocTinhDuLieuRepository.Instance.ThongKeTheoThuocTinh(app_id, nguoi_tao, loai, thuoc_tinh, lst_id, lst_should_id, lst_must_not_id, is_admin, op);
        }
    }
}
