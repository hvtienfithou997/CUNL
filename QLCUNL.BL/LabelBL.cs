using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;

namespace QLCUNL.BL
{
    public class LabelBL
    {
        public static IEnumerable<Label> GetAll(int page_index, int page_size)
        {
            return LabelRepository.Instance.GetAll();
        }
        public static bool Index(Label data)
        {
            return LabelRepository.Instance.Index(data);
        }
        public static Label GetById(string id)
        {
            return LabelRepository.Instance.GetById(id);
        }
        public static bool Update(Label data)
        {
            return LabelRepository.Instance.Update(data);
        }
        public static List<Label> Search(string app_id, string term, int page, out long total_recs, out string msg, int page_size = 50)
        {
            return LabelRepository.Instance.Search(app_id, term, page, out total_recs, out msg, page_size);
        }
    }
}
