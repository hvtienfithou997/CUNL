using ESUtil;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.BL
{
    public class LogSendMailBL
    {
        public static int IndexMany(List<LogSendMail> data)
        {
            return LogSendMailRepository.Instance.IndexMany(data);
        }
        public static List<LogSendMail> TimMailMoi(string app_id)
        {
            return LogSendMailRepository.Instance.TimMailMoi(app_id);            
        }

        public static List<LogSendMail> TimMailMoiByStt(string stt)
        {
            return LogSendMailRepository.Instance.TimMailMoiByStt(stt);
        }
        public static List<LogSendMail> Search(string app_id, string status, int page, out long total_recs, out string msg, int page_size = 50)
        {
            page_size = page_size > 500 ? 500 : page_size;
            page = page < 1 ? 1 : page;
            return LogSendMailRepository.Instance.Search(app_id, status, page, out total_recs, out msg, page_size);
        }
        public static bool Update(LogSendMail data)
        {
            return LogSendMailRepository.Instance.Update(data);
        }
        public static bool UpdateStatus(string id, string stt)
        {
            return LogSendMailRepository.Instance.UpdateStatus(id, stt);
        }
    }
}
