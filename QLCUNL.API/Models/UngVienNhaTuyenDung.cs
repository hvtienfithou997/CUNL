using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLCUNL.Models;

namespace QLCUNL.API.Models
{
    public class UngVienNhaTuyenDung
    {
        public UngVienNhaTuyenDung(UngVien item)
        {
            this.ho_ten_ung_vien = item.ho_ten_ung_vien;
            this.dia_chi = item.dia_chi;
            this.vi_tri_ung_tuyen = item.vi_tri_ung_tuyen;
            this.noi_dung = item.noi_dung;
            
            this.luong_mong_muon = item.luong_mong_muon;
            
            this.link_cv = item.link_cv;
            this.id_ung_vien = item.id_ung_vien;
            

        }
        public int stt { get; set; }
        public string id_ung_vien { get; set; }
        public string ho_ten_ung_vien { get; set; }
        
        public string dia_chi { get; set; }
        public string vi_tri_ung_tuyen { get; set; }
        
        public string noi_dung { get; set; }
        
       
        public double luong_mong_muon { get; set; }
       

        
        public string link_cv { get; set; }
        
    }
}
