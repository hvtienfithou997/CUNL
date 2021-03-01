using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QLCUNL.Models
{
    public class NoteUngVien : BaseET
    {
        [Keyword]
        [DisplayName("Note ứng viên ID")]
        public string id_note_ung_vien { get; set; }
        [Keyword]
        [DisplayName("Ứng viên ID")]
        public string id_ung_vien { get; set; }
        [DisplayName("Tên ứng viên")]
        public string ten_ung_vien { get; set; }        
        [DisplayName("Ghi chú")]
        public string ghi_chu { get; set; }
        [Keyword]
        [DisplayName("Tên người dùng")]
        public string user_name { get; set; }
        public List<int> thuoc_tinh { get; set; }
        public NoteUngVien()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
    }
}
