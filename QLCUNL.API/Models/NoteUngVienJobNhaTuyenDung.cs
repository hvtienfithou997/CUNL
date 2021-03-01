using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class NoteUngVienJobNhaTuyenDung
    {
        public NoteUngVienJobNhaTuyenDung(NoteUngVienJob item, UngVien ung_vien)
        {
            this.luong_mong_muon = ung_vien.luong_mong_muon;
            this.dia_chi = ung_vien.dia_chi;
            this.noi_dung = ung_vien.noi_dung;
            this.vi_tri_ung_tuyen = ung_vien.vi_tri_ung_tuyen;
            this.ho_ten_ung_vien = ung_vien.ho_ten_ung_vien;
            this.link_cv = ung_vien.link_cv;
            
            this.id_ung_vien = item.id_ung_vien;            
            this.ngay_gio_phong_van = item.ngay_gio_phong_van;
            this.ghi_chu_ung_vien = item.ghi_chu_ung_vien;
            this.ghi_chu_nha_tuyen_dung = item.ghi_chu_nha_tuyen_dung;
            this.ghi_chu = item.ghi_chu;
            id_note_ung_vien_job = item.id_note_ung_vien_job;
        }
        public string tom_tat_ung_vien { get; set; }
        public string id_note_ung_vien_job { get; set; }
        public int stt { get; set; }
        public string id_ung_vien { get; set; }
        public string ho_ten_ung_vien { get; set; }
        public string dia_chi { get; set; }
        public string vi_tri_ung_tuyen { get; set; }

        public string noi_dung { get; set; }

        public string ghi_chu { get; set; }

        public double luong_mong_muon { get; set; }

        public string link_cv { get; set; }
        public string ghi_chu_ung_vien { get; set; }
        public string ghi_chu_nha_tuyen_dung { get; set; }
        public long ngay_gio_phong_van { get; set; }
    }
}
