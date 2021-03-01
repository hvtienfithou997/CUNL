using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class NoteUngVienUngVienMap: NoteUngVien
    {
        public NoteUngVienUngVienMap(NoteUngVien note)
        {
            id_note_ung_vien = note.id_note_ung_vien;
            id_ung_vien = note.id_ung_vien;
            ghi_chu = note.ghi_chu;
            thuoc_tinh = note.thuoc_tinh;
            ngay_sua = note.ngay_sua;
            nguoi_tao = note.nguoi_tao;
            nguoi_sua = note.nguoi_sua;
            ngay_tao = note.ngay_tao;
        }
        public string ho_ten_ung_vien { get; set; }
        public string so_dien_thoai { get; set; }
        public string email { get; set; }
        public string zalo { get; set; }
    }
}
