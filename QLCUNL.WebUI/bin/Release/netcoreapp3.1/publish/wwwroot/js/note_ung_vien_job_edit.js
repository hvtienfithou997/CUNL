$(function () {
    var form = $('#validate-edit-note');
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

    callAPI(`${API_URL}/NoteUngVienJob/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                
                $("#id_ung_vien").val(res.data.id_ung_vien);
                $("#user_name").val(res.data.user_name);
                $("#id_job").val(res.data.id_job);
                $("#parent_id_ung_vien").val(res.data.parent_id_ung_vien);
                for (var i = 0; i < res.data.thuoc_tinh.length; i++) {
                    let tt = res.data.thuoc_tinh[i];
                    $(`input[name='thuoc_tinh_nhom_${tt.nhom}'][value='${tt.gia_tri}']`).prop('checked', true);
                    $(`input[name^='thuoc_tinh'][value='${tt.gia_tri}']`).prop('checked', true);
                }
                $("#ghi_chu").val(res.data.ghi_chu);
                $("#ngay_gio_phong_van").val(epochToTime(res.data.ngay_gio_phong_van));
                $("#ngay_di_lam").val(epochToTime(res.data.ngay_di_lam));
                $("#luong_thu_viec").val(res.data.luong_thu_viec);
                $("#luong_chinh_thuc").val(res.data.luong_chinh_thuc);
                $("#thuoc_tinh").val(res.data.thuoc_tinh);
                $("#so_tien_da_nhan").val(res.data.so_tien_da_nhan);
                $("#so_tien_tra_lai").val(res.data.so_tien_tra_lai);
                $("#ghi_chu_nha_tuyen_dung").val(res.data.ghi_chu_nha_tuyen_dung);
                $("#ghi_chu_ung_vien").val(res.data.ghi_chu_ung_vien);
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})

function onUpdate(event) {
    event.preventDefault();
    if ($('#validate-edit-note').valid()) {
        
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
        $("input[name^='thuoc_tinh']:checked").each(function (el) {
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
            "id_note_ung_vien_job": id, "id_ung_vien": id_ung_vien, "user_name": user_name, "id_job": id_job, "parent_id_ung_vien": parent_id_ung_vien,
            "ghi_chu": ghi_chu, "ngay_gio_phong_van": ngay_gio_phong_van, "ngay_di_lam": ngay_di_lam, "luong_thu_viec": luong_thu_viec, "luong_chinh_thuc": luong_chinh_thuc,
            "thuoc_tinh": thuoc_tinh, "so_tien_da_nhan": so_tien_da_nhan, "so_tien_tra_lai": so_tien_tra_lai, "ghi_chu_nha_tuyen_dung": ghi_chu_nha_tuyen_dung,
            "ghi_chu_ung_vien": ghi_chu_ung_vien, "nguoi_sua": user
        };
        callAPI(`${API_URL}/NoteUngVienJob/${id}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Sửa thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};