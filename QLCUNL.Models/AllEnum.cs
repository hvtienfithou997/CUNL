
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace QLCUNL.Models
{
    public enum UserType
    {
        USER, SYS_ADMIN
    }

    public enum TrangThaiJob
    {
        NA,
        [Display(Name = "Mới tạo")]
        MOI_TAO,
        [Display(Name = "Đang kích hoạt")]
        DANG_KICH_HOAT,
        [Display(Name = "Dừng kích hoạt")]
        DUNG_KICH_HOAT,
        [Display(Name = "Đã đóng")]
        DA_DONG
    }
    public enum TrangThaiUngVien
    {
        [Display(Name = "Mới tạo")]
        MOI_TAO,
        [Display(Name = "Đã đi làm")]
        DA_DI_LAM,
        [Display(Name = "Chưa đi làm")]
        CHUA_DI_LAM
    }
    public enum TrangThaiNoteUVJob
    {
        [Display(Name = "Không tham gia")]
        KHONG_THAM_GIA,
        [Display(Name = "Tham gia phỏng vấn")]
        THAM_GIA_PHONG_VAN,
        [Display(Name = "Đỗ phỏng vấn")]
        DO_PHONG_VAN,
        [Display(Name = "Trượt phỏng vấn")]
        TRUOT_PHONG_VAN,
        [Display(Name = "Mới nhận")]
        DUOC_TUYEN
    }
    public enum TrangThaiUserJob
    {
        [Display(Name = "Mới nhận")]
        MOI_NHAN,
        [Display(Name = "Hoàn thành")]
        HOAN_THANH
    }
    public enum PhanQuyenRule : byte
    {
        ALL = 0,
        [Display(Name = "Theo tài khoản")]
        USER = 1,
        [Display(Name = "Theo đối tượng")]
        OBJECT = 2
    }
    public enum PhanQuyenType : byte
    {
        ALL = 0,
        [Display(Name = "Theo tài khoản")]
        USERS = 1,
        [Display(Name = "Theo nhóm")]
        GROUP_USERS = 2
    }
    public enum PhanQuyenObjType
    {
        ALL,
        [Display(Name = "Công ty")]
        CONG_TY,
        [Display(Name = "JOB đá gán")]
        USER_JOB,
        [Display(Name = "Note ứng viên")]
        NOTE_UNG_VIEN,
        [Display(Name = "Ứng viên")]
        UNG_VIEN,
        [Display(Name = "JOB")]
        JOB,
        [Display(Name = "Note ứng viên theo JOB")]
        NOTE_UNG_VIEN_JOB,
        [Display(Name = "Menu")]
        MENU
    }
    public enum ThuocTinhType
    {
        [Display(Name = "Shared")]
        SHARED,
        [Display(Name = "Private")]
        PRIVATE
    }
    public enum LoaiThuocTinh
    {
        [Display(Name = "Công ty")]
        CONG_TY,
        [Display(Name = "JOB đã gán")]
        USER_JOB,
        [Display(Name = "Note ứng viên")]
        NOTE_UNG_VIEN,
        [Display(Name = "Ứng viên")]
        UNG_VIEN,
        [Display(Name = "JOB")]
        JOB,
        [Display(Name = "Note ứng viên theo JOB")]
        NOTE_UNG_VIEN_JOB,
        [Display(Name = "Note ghi chú ứng viên ")]
        NOTE_GHI_CHU_UNG_VIEN,
        [Display(Name = "Note ghi chú công ty")]
        NOTE_GHI_CHU_CONG_TY,
        [Display(Name = "Note ghi chú ứng viên theo JOB")]
        NOTE_GHI_CHU_UNG_VIEN_JOB,
        [Display(Name = "Note ghi chú JOB")]
        NOTE_GHI_CHU_JOB
    }
    public enum LoaiNote
    {
        [Display(Name = "Công ty")]
        CONG_TY,
        [Display(Name = "JOB đã Gán")]
        USER_JOB,
        [Display(Name = "Note ứng viên")]
        NOTE_UNG_VIEN,
        [Display(Name = "Ứng viên")]
        UNG_VIEN,
        [Display(Name = "JOB")]
        JOB,
        [Display(Name = "Note ứng viên theo JOB")]
        NOTE_UNG_VIEN_JOB,
        [Display(Name = "Nhà tuyển dụng")]
        NHA_TUYEN_DUNG,
        [Display(Name = "Note ứng viên gửi nhà tuyển dụng")]
        NOTE_UNG_VIEN_GUI_NHA_TUYEN_DUNG          
    }

    public enum LoaiDuLieu
    {
        [Display(Name="Hệ thống sinh")]
        HE_THONG,
        [Display(Name = "Người dùng sinh")]
        NGUOI_DUNG
    }
    public enum Quyen
    {
        VIEW,
        EDIT,
        ADD,
        DELETE
    }
    public enum TrangThai
    {
        ACTIVE, DISABLED, DELETED
    }
    public enum Role
    {
        SYS_ADMIN, APP_ADMIN, SUPER_USER, USER
    }
}
