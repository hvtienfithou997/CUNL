using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class TinhThanhBL
    {
        public static void GetAll()
        {
            TinhThanhRepository.Instance.GetAll();
        }
        public static bool Index(TinhThanh data)
        {
            return TinhThanhRepository.Instance.Index(data);
        }
        public static TinhThanh GetById(string id)
        {
            return TinhThanhRepository.Instance.GetById(id);
        }
        public static bool Update(TinhThanh data)
        {
            return TinhThanhRepository.Instance.Update(data);
        }

        public static List<TinhThanh> Search(int page, out long total_recs, out string msg, int page_size = 9999)
        {
            return TinhThanhRepository.Instance.Search(page, out total_recs, out msg, page_size);
        }
        public static bool UpdateSTT(string id, int stt)
        {
            return TinhThanhRepository.Instance.UpdateSTT(id, stt);
        }
    }
}
