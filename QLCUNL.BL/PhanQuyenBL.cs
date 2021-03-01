using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class PhanQuyenBL
    {
        public static void GetAll()
        {
            PhanQuyenRepository.Instance.GetAll();
        }
        public static bool Index(PhanQuyen data)
        {
            return PhanQuyenRepository.Instance.Index(data);
        }
        public static PhanQuyen GetById(string id)
        {
            return PhanQuyenRepository.Instance.GetById(id);
        }
        public static bool Update(PhanQuyen data)
        {
            return PhanQuyenRepository.Instance.Update(data);
        }
        public static bool UpdateNgayHetHan(List<PhanQuyen> lst, string nguoi_sua)
        {
            return PhanQuyenRepository.Instance.UpdateNgayHetHan(lst, nguoi_sua);
        }
        public static PhanQuyen GetById(string id, string[] fields)
        {
            return PhanQuyenRepository.Instance.GetById(id, fields);
        }

        public static List<PhanQuyen> GetByUser(string user, string[] fields = null)
        {
            return PhanQuyenRepository.Instance.GetByUser(user, fields);
        }
        public static List<PhanQuyen> GetQuyenActive(string user, int group, PhanQuyenObjType obj_type, IEnumerable<int> quyen, string[] fields = null)
        {
            return PhanQuyenRepository.Instance.GetQuyenActive(user, group, obj_type, quyen, fields);
        }
        public static List<PhanQuyen> Get(string tu_khoa, PhanQuyenRule rule, PhanQuyenType type, string user,
           PhanQuyenObjType obj_type, string obj_id, List<Quyen> quyen,
           long ngay_het_tu, long ngay_het_den, string nguoi_tao, long ngay_tao_tu, long ngay_tao_den,
           string nguoi_sua, long ngay_sua_tu, long ngay_sua_den, int page, int recs, out long total)
        {
            return PhanQuyenRepository.Instance.Get(tu_khoa, rule, type, user, obj_type, obj_id, quyen,
            ngay_het_tu, ngay_het_den, nguoi_tao, ngay_tao_tu, ngay_tao_den, nguoi_sua, ngay_sua_tu, ngay_sua_den, page, recs, out total);
        }
        public static bool IsExistQuyen(string user, int group, PhanQuyenObjType obj_type, string obj_id, string owner, List<int> quyen)
        {
            return PhanQuyenRepository.Instance.IsExistQuyen(user, group, obj_type, obj_id, owner, quyen);
        }
        public static void RemoveByListId(List<string> lst_id)
        {
             PhanQuyenRepository.Instance.RemoveByListId(lst_id);
        }
        public static bool RemovePhanQuyenByUser(string user, List<string> obj_id)
        {
            return PhanQuyenRepository.Instance.RemovePhanQuyenByUser(user, obj_id);
        }
        public static void DeleteAll()
        {
            PhanQuyenRepository.Instance.DeleteAll();
        }
    }
}
