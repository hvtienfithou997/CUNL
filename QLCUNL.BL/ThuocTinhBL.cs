using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class ThuocTinhBL
    {
        public static IEnumerable<ThuocTinh> GetAll(int page_index, int page_size)
        {
            return ThuocTinhRepository.Instance.GetAll();
        }
        public static bool Index(ThuocTinh data)
        {
            return ThuocTinhRepository.Instance.Index(data);
        }
        public static ThuocTinh GetById(string id)
        {
            return ThuocTinhRepository.Instance.GetById(id);
        }
        public static bool Update(ThuocTinh data, out string msg)
        {
            return ThuocTinhRepository.Instance.Update(data, out msg);
        }
        public static bool UpdateTenNhom(ThuocTinh data)
        {
            return ThuocTinhRepository.Instance.UpdateTenNhom(data);
        }
        
        public static List<ThuocTinh> Search(string app_id, string term, int loai, int type, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin=false)
        {
            return ThuocTinhRepository.Instance.Search(app_id, term, loai, type, page, out total_recs, out msg, page_size, is_admin);
        }
        public static List<ThuocTinh> Search(string app_id, string nguoi_tao, string term, int loai, int type, int page, out long total_recs, out string msg, int page_size = 50, bool is_admin=false)
        {
            return ThuocTinhRepository.Instance.Search(app_id, nguoi_tao, term, loai, type, page, out total_recs, out msg, page_size, is_admin);
        }
        public static List<ThuocTinh> GetAllByLoaiThuocTinh(string app_id, int loai, int type)
        {
            return ThuocTinhRepository.Instance.GetAllByLoaiThuocTinh(app_id, loai, type);
        }
        public static List<ThuocTinh> GetAllByLoaiThuocTinh(string app_id, string nguoi_tao, int loai)
        {
            return ThuocTinhRepository.Instance.GetAllByLoaiThuocTinh(app_id, nguoi_tao, loai);
        }
        public static bool IsGiaTriThuocTinhExist(string app_id, LoaiThuocTinh loai, ThuocTinhType type, int gia_tri)
        {
            return ThuocTinhRepository.Instance.IsGiaTriThuocTinhExist(app_id, loai, type, gia_tri);
        }
        public static bool IsTenThuocTinhExist(string app_id, LoaiThuocTinh loai, ThuocTinhType type, string ten)
        {
            return ThuocTinhRepository.Instance.IsTenThuocTinhExist(app_id, loai, type, ten);
        }
        public static List<ThuocTinh> GeSharedtByLoaiGiaTri(string app_id, IEnumerable<int> thuoc_tinh, LoaiThuocTinh loai)
        {
            return ThuocTinhRepository.Instance.GeSharedtByLoaiGiaTri(app_id, thuoc_tinh, loai);

        }
        public static List<ThuocTinh> GetPrivateByLoaiGiaTri(string app_id, string nguoi_tao, IEnumerable<int> thuoc_tinh, LoaiThuocTinh loai, bool is_admin)
        {
            return ThuocTinhRepository.Instance.GetPrivateByLoaiGiaTri(app_id, nguoi_tao, thuoc_tinh, loai, is_admin);

        }
        public static List<ThuocTinh> GetMany(IEnumerable<string> lst_id)
        {
            return ThuocTinhRepository.Instance.GetMany(lst_id);
        }
        public static List<ThuocTinh> GetManyByGiaTri(string app_id, IEnumerable<int> lst_gia_tri, LoaiThuocTinh loai, ThuocTinhType type)
        {
            return ThuocTinhRepository.Instance.GetManyByGiaTri(app_id, lst_gia_tri, loai, type);
        }
        public static List<ThuocTinh> GetManyByGiaTri(string app_id, IEnumerable<int> lst_gia_tri, LoaiThuocTinh loai, int type)
        {
            return ThuocTinhRepository.Instance.GetManyByGiaTri(app_id, lst_gia_tri, loai, type);
        }
        public static bool IsOwner(string id, string nguoi_tao)
        {
            return ThuocTinhRepository.Instance.IsOwner(id, nguoi_tao);
        }
        public static bool Delete(string id)
        {
            return ThuocTinhRepository.Instance.Delete(id);
        }
        public static bool RemoveDataByAppId(string app_id)
        {
            return ThuocTinhRepository.Instance.RemoveDataByAppId<ThuocTinh>(app_id);
        }
        public static void UpdateTest(string id, int gia_tri)
        {
            ThuocTinhRepository.Instance.UpdateTest(id,gia_tri);
        }
    }

}
