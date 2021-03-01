$(document).ready(function () {
    var form = $('#validate-note-uv-add');

    form.validate({
        errorClass: 'errors',
        rules: {
            id_ung_vien: {
                required: true
            },
            id_job: {
                required: true
            },
            luong_thu_viec: {
                required: true
            },
            luong_chinh_thuc: {
                required: true
            }
        },
        messages: {
            id_ung_vien: {
                required: "Bạn phải chọn ứng viên"
            },
            id_job: {
                required: "Bạn phải chọn một job"
            },
            luong_thu_viec: {
                required: "Bạn phải nhập lương thử việc"
            },
            luong_chinh_thuc: {
                required: "Bạn phải nhập lương chính thức"
            }
        }
    });
});

function onSubmit(event) {
    event.preventDefault();
    if ($('#validate-note-uv-add').valid()) {
        
        let id_ung_vien = $('#id_ung_vien').val();
        let user_name = $('#user_name').val();
        let id_job = $('#id_job').val();
        let parent_id_ung_vien = $('#parent_id_ung_vien').val();
        let ghi_chu = $('#ghi_chu').val();
        let ngay_gio_phong_van = 0;
        if ($('#ngay_gio_phong_van').val() !== "") {
            ngay_gio_phong_van = Math.floor(toDate($('#ngay_gio_phong_van').val()) / 1000.0);
        }

        let ngay_di_lam = 0;
        if ($('#ngay_di_lam').val() !== "") {
            ngay_di_lam = Math.floor(toDate($('#ngay_di_lam').val()) / 1000.0);
        }
        let luong_thu_viec = $('#luong_thu_viec').val() != "" ? $('#luong_thu_viec').val() : 0;
        let luong_chinh_thuc = $('#luong_chinh_thuc').val() != "" ? $('#luong_chinh_thuc').val() : 0;
        luong_thu_viec = replaceDot(luong_thu_viec);
        luong_chinh_thuc = replaceDot(luong_chinh_thuc);
        let thuoc_tinh = [];
        $("input[name^='thuoc_tinh']: checked").each(function (el) {
            try {
                thuoc_tinh.push(parseInt($(this).val()));
            } catch{
            }
        });
        let so_tien_da_nhan = $('#so_tien_da_nhan').val() != "" ? $('#so_tien_da_nhan').val() : 0;
        let so_tien_tra_lai = $('#so_tien_tra_lai').val() != "" ? $('#so_tien_tra_lai').val() : 0;
        let ghi_chu_nha_tuyen_dung = $('#ghi_chu_nha_tuyen_dung').val();
        let ghi_chu_ung_vien = $('#ghi_chu_ung_vien').val();

        var obj = {
            "id_ung_vien": id_ung_vien, "user_name": user_name, "id_job": id_job, "parent_id_ung_vien": parent_id_ung_vien,
            "ghi_chu": ghi_chu, "ngay_gio_phong_van": ngay_gio_phong_van, "ngay_di_lam": ngay_di_lam, "luong_thu_viec": luong_thu_viec, "luong_chinh_thuc": luong_chinh_thuc,
            "thuoc_tinh": thuoc_tinh, "so_tien_da_nhan": so_tien_da_nhan, "so_tien_tra_lai": so_tien_tra_lai, "ghi_chu_nha_tuyen_dung": ghi_chu_nha_tuyen_dung,
            "ghi_chu_ung_vien": ghi_chu_ung_vien, "nguoi_tao": user, "nguoi_sua": user
        };

        callAPI(`${API_URL}/NoteUngVienJob/Add`, obj, "POST", function (res) {
            if (res.success) {
                $.notify("Thêm thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};

function bindDetail(id) {
    callAPI(`${API_URL}/NoteUngVienJob/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            console.log(res)
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#note_ung_vien_job_detail").empty();
                $("#note_ung_vien_job_detail").append(`<li>Ứng viên:  <span class="font-weight-bold"><a target="_blank" href="/ungvien/detail/${res.data.id_ung_vien}">${res.data.ho_ten_ung_vien}</a></span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Liên hệ:  <ul><li>SĐT:  <span class="font-weight-bold">${res.data.so_dien_thoai}</span></li><li>Email:  <span class="font-weight-bold">${res.data.email}</span></li><li>Zalo:  <span class="font-weight-bold">${res.data.zalo}</span></li></ul></li>`);
                $("#note_ung_vien_job_detail").append(`<li>JOB:  <span class="font-weight-bold">(<span class="auto_id" onclick="copyText(this)"><code>${res.data.auto_id_job}</code></span>) <a target="_blank" href="/job/detail/${res.data.id_job}">${res.data.ten_job}</a></span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Đối cho ứng viên: <span class="font-weight-bold">${res.data.parent_id_ung_vien}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Ghi chú: <span class="font-weight-bold">${res.data.ghi_chu}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Ngày giờ phỏng vấn: <span class="font-weight-bold">${epochToTime(res.data.ngay_gio_phong_van)}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Ngày đi làm: <span class="font-weight-bold">${epochToTime(res.data.ngay_di_lam)}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Lương thử việc: <span class="font-weight-bold">${formatCurency(res.data.luong_thu_viec)}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Lương chính thức: <span class="font-weight-bold">${formatCurency(res.data.luong_chinh_thuc)}</span></li>`);

                let html_thuoc_tinh = "<ul>";
                if (res.data.thuoc_tinh != null) {
                    res.data.thuoc_tinh.forEach(tt => {
                        html_thuoc_tinh += `<li><span class="font-weight-bold">${tt.ten}</span></li>`;
                    });
                }
                html_thuoc_tinh += `</ul>`;
                $("#note_ung_vien_job_detail").append(`<li>Thuộc tính: <span class="font-weight-bold">${html_thuoc_tinh}</span></li>`);
                //$("#note_ung_vien_job_detail").append(`<li>Ghi chú nhà tuyển dụng: <span class="font-weight-bold">${res.data.ghi_chu_nha_tuyen_dung}</span></li>`);
                //$("#note_ung_vien_job_detail").append(`<li>Ghi chú ứng viên: <span class="font-weight-bold">${res.data.ghi_chu_ung_vien}</span></li>`);
                if (typeof res.data.so_tien_da_nhan !== 'undefined')
                    $("#note_ung_vien_job_detail").append(`<li>Số tiền đã nhận: <span class="font-weight-bold">${formatCurency(res.data.so_tien_da_nhan)}</span></li>`);
                if (typeof res.data.so_tien_tra_lai !== 'undefined')
                    $("#note_ung_vien_job_detail").append(`<li>Số tiền trả lại: <span class="font-weight-bold">${formatCurency(res.data.so_tien_tra_lai)}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Ngày tạo: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_tao)}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Người tạo: <span class="font-weight-bold">${res.data.nguoi_tao}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Ngày sửa: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_sua)}</span></li>`);
                $("#note_ung_vien_job_detail").append(`<li>Người sửa: <span class="font-weight-bold">${res.data.nguoi_sua}</span></li>`);
                $("#note_ung_vien_job_detail").children().addClass('list-group-item');
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}