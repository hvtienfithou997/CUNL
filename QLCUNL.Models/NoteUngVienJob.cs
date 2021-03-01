using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class NoteUngVienJob : BaseET
    {
        [Keyword]
        public string id_note_ung_vien_job { get; set; }
        [Keyword]
        public string id_ung_vien { get; set; }
        [Keyword]
        public string user_name { get; set; }
        [Keyword]
        public string id_job { get; set; }
        [Keyword]
        public string parent_id_ung_vien { get; set; }
        [Keyword]
        public string id_user_job { get; set; }
        public string ghi_chu { get; set; }
        public long ngay_gio_phong_van { get; set; }
        public long ngay_di_lam { get; set; }
        public double luong_thu_viec { get; set; }
        public double luong_chinh_thuc { get; set; }
        public List<int> thuoc_tinh { get; set; }
        public double so_tien_da_nhan { get; set; }
        public double so_tien_tra_lai { get; set; }
        public string ghi_chu_nha_tuyen_dung { get; set; }
        public string ghi_chu_ung_vien { get; set; }
    }
}
