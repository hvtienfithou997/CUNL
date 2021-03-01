using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Models
{
    public class NoteUngVienJobMap : BaseET
    {
        public NoteUngVienJobMap(NoteUngVienJob item, Dictionary<string, UngVien> data_ung_vien)
        {
                UngVien ung_vien;
                
                if (data_ung_vien.TryGetValue(item.id_ung_vien, out ung_vien))
                {
                    this.id_note_ung_vien_job = item.id_note_ung_vien_job;
                    this.ho_ten_ung_vien = ung_vien.ho_ten_ung_vien;
                this.id_ung_vien = ung_vien.id_ung_vien;
                    
                }
        }

        public NoteUngVienJobMap(Note note, List<NoteUngVienJobMap> lst)
        {
           foreach(var item in lst.Where(x => x.id_note_ung_vien_job.Contains(note.id_obj)))
            {
                ho_ten_ung_vien = item.ho_ten_ung_vien;
                noi_dung = note.noi_dung;
                nguoi_tao = note.nguoi_tao;
                thuoc_tinh = note.thuoc_tinh;
                ngay_tao = note.ngay_tao;
            }
            
        }
        public NoteUngVienJobMap(NoteUngVienJob item, Dictionary<string, Job> data_job,
            Dictionary<string, UngVien> data_ung_vien, List<ThuocTinh> data_thuoc_tinh, bool filter_thuoc_tinh = true)
        {
            this.ghi_chu = item.ghi_chu;
            this.id_job = item.id_job;
            this.id_note_ung_vien_job = item.id_note_ung_vien_job;
            this.id_ung_vien = item.id_ung_vien;
            this.id_user_job = item.id_user_job;
            this.luong_chinh_thuc = item.luong_chinh_thuc;
            this.luong_thu_viec = item.luong_thu_viec;
            this.ngay_di_lam = item.ngay_di_lam;
            this.ngay_gio_phong_van = item.ngay_gio_phong_van;
            this.parent_id_ung_vien = item.parent_id_ung_vien;
            this.ghi_chu_ung_vien = item.ghi_chu_ung_vien;
            this.ghi_chu_nha_tuyen_dung = item.ghi_chu_nha_tuyen_dung;
            this.so_tien_da_nhan = item.so_tien_da_nhan;
            this.so_tien_tra_lai = item.so_tien_tra_lai;
            if (item.thuoc_tinh != null && filter_thuoc_tinh)
                thuoc_tinh = data_thuoc_tinh.Where(x => item.thuoc_tinh.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type }).ToList();
            else
            {
                if (!filter_thuoc_tinh)
                    thuoc_tinh = data_thuoc_tinh;
            }
            if (data_job.TryGetValue(this.id_job, out Job job))
            {
                this.ten_job = job.chuc_danh;
                this.cong_ty = job.cong_ty.ten_cong_ty;
                this.auto_id_job = job.auto_id.ToString();
            }
            if (data_ung_vien.TryGetValue(this.id_ung_vien, out UngVien ung_vien))
            {
                this.ho_ten_ung_vien = ung_vien.ho_ten_ung_vien;
                this.so_dien_thoai = ung_vien.so_dien_thoai;
                this.email = ung_vien.email;
                this.zalo = ung_vien.zalo;
            }
            if (thuoc_tinh == null)
                thuoc_tinh = new List<object>();
            if (string.IsNullOrEmpty(this.cong_ty))
                this.cong_ty = "";
            ngay_sua = item.ngay_sua;
            ngay_tao = item.ngay_tao;
            nguoi_tao = item.nguoi_tao;
            nguoi_sua = item.nguoi_sua;
        }
        public NoteUngVienJobMap(NoteUngVienJob item, Dictionary<string, Job> data_job, Dictionary<string, string> dic_cong_ty,
           Dictionary<string, UngVien> data_ung_vien, List<ThuocTinh> data_thuoc_tinh, bool filter_thuoc_tinh = true)
        {
            this.ghi_chu = item.ghi_chu;
            this.id_job = item.id_job;
            this.id_note_ung_vien_job = item.id_note_ung_vien_job;
            this.id_ung_vien = item.id_ung_vien;
            this.id_user_job = item.id_user_job;
            this.luong_chinh_thuc = item.luong_chinh_thuc;
            this.luong_thu_viec = item.luong_thu_viec;
            this.ngay_di_lam = item.ngay_di_lam;
            this.ngay_gio_phong_van = item.ngay_gio_phong_van;
            this.parent_id_ung_vien = item.parent_id_ung_vien;
            this.ghi_chu_ung_vien = item.ghi_chu_ung_vien;
            this.ghi_chu_nha_tuyen_dung = item.ghi_chu_nha_tuyen_dung;
            this.so_tien_da_nhan = item.so_tien_da_nhan;
            this.so_tien_tra_lai = item.so_tien_tra_lai;
            if (item.thuoc_tinh != null && filter_thuoc_tinh)
                thuoc_tinh = data_thuoc_tinh.Where(x => item.thuoc_tinh.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type }).ToList();
            else
            {
                if (!filter_thuoc_tinh)
                    thuoc_tinh = data_thuoc_tinh;
            }
            if (data_job.TryGetValue(this.id_job, out Job job))
            {
                this.ten_job = job.chuc_danh;
                if (job.cong_ty != null && !string.IsNullOrEmpty(job.cong_ty.id_cong_ty))
                {
                    dic_cong_ty.TryGetValue(job.cong_ty.id_cong_ty, out string ten_cong_ty);
                    job.cong_ty.ten_cong_ty = ten_cong_ty;
                    this.cong_ty = job.cong_ty.ten_cong_ty;
                    this.id_cong_ty = job.cong_ty.id_cong_ty;
                }                
                this.auto_id_job = job.id_auto;
            }
            if (data_ung_vien.TryGetValue(this.id_ung_vien, out UngVien ung_vien))
            {
                this.ho_ten_ung_vien = ung_vien.ho_ten_ung_vien;
                this.so_dien_thoai = ung_vien.so_dien_thoai;
                this.email = ung_vien.email;
                this.zalo = ung_vien.zalo;
            }
            if (thuoc_tinh == null)
                thuoc_tinh = new List<object>();
            if (string.IsNullOrEmpty(this.cong_ty))
                this.cong_ty = "";
            ngay_sua = item.ngay_sua;
            ngay_tao = item.ngay_tao;
            nguoi_tao = item.nguoi_tao;
            nguoi_sua = item.nguoi_sua;
        }
        public NoteUngVienJobMap(NoteUngVienJob item, Dictionary<string, Job> data_job,
           Dictionary<string, UngVien> data_ung_vien, List<ThuocTinh> data_thuoc_tinh, List<int> thuoc_tinh_rieng)
        {
            this.ghi_chu = item.ghi_chu;
            this.id_job = item.id_job;
            this.id_note_ung_vien_job = item.id_note_ung_vien_job;
            this.id_ung_vien = item.id_ung_vien;
            this.id_user_job = item.id_user_job;
            this.luong_chinh_thuc = item.luong_chinh_thuc;
            this.luong_thu_viec = item.luong_thu_viec;
            this.ngay_di_lam = item.ngay_di_lam;
            this.ngay_gio_phong_van = item.ngay_gio_phong_van;
            this.parent_id_ung_vien = item.parent_id_ung_vien;
            this.ghi_chu_ung_vien = item.ghi_chu_ung_vien;
            this.ghi_chu_nha_tuyen_dung = item.ghi_chu_nha_tuyen_dung;
            this.so_tien_da_nhan = item.so_tien_da_nhan;
            this.so_tien_tra_lai = item.so_tien_tra_lai;

            var item_thuoc_tinh = new List<int>();
            if (item.thuoc_tinh != null)
                item_thuoc_tinh = item.thuoc_tinh;

            thuoc_tinh = data_thuoc_tinh.Where(x => item_thuoc_tinh.Contains(x.gia_tri) || thuoc_tinh_rieng.Contains(x.gia_tri)).Select(x => new { gia_tri = x.gia_tri, ten = x.ten, nhom = x.nhom, type = x.type }).ToList();

            if (data_job.TryGetValue(this.id_job, out Job job))
            {
                this.ten_job = job.chuc_danh;
                this.cong_ty = job.cong_ty.ten_cong_ty;
                this.auto_id_job = job.auto_id.ToString();
            }
            if (data_ung_vien.TryGetValue(this.id_ung_vien, out UngVien ung_vien))
            {
                this.ho_ten_ung_vien = ung_vien.ho_ten_ung_vien;
                this.so_dien_thoai = ung_vien.so_dien_thoai;
                this.email = ung_vien.email;
                this.zalo = ung_vien.zalo;
            }
            if (string.IsNullOrEmpty(this.cong_ty))
                this.cong_ty = "";
            ngay_sua = item.ngay_sua;
            ngay_tao = item.ngay_tao;
            nguoi_tao = item.nguoi_tao;
            nguoi_sua = item.nguoi_sua;
         
        }

        public string noi_dung { get; set; }
        public string ho_ten_ung_vien { get; set; }
        public string so_dien_thoai { get; set; }
        public string email { get; set; }
        public string zalo { get; set; }
        public string ten_job { get; set; }
        public string cong_ty { get; set; }
        public string id_cong_ty { get; set; }
        public string nguoi_lien_he { get; set; }
        public string id_note_ung_vien_job { get; set; }
        public string id_ung_vien { get; set; }
        public string id_job { get; set; }
        public string auto_id_job { get; set; }
        public string parent_id_ung_vien { get; set; }
        public string id_user_job { get; set; }
        public string ghi_chu { get; set; }
        public long ngay_gio_phong_van { get; set; }
        public long ngay_di_lam { get; set; }
        public double luong_thu_viec { get; set; }
        public double luong_chinh_thuc { get; set; }
        public dynamic thuoc_tinh { get; set; }
        public string ghi_chu_nha_tuyen_dung { get; set; }
        public string ghi_chu_ung_vien { get; set; }
        public double so_tien_da_nhan { get; set; }
        public double so_tien_tra_lai { get; set; }
        public bool xem_cv { get; set; }
    }
}
