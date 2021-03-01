$(function () {
    getNoteUngVien();
    $(".field-thuoc-tinh").append("<span type='button' class='float-right btn-danger remove-elem'> Bỏ chọn</span>");
    $(".remove-elem").click(function () {
        $(this).siblings('ul').find('input').prop('checked', false);
    });
    $('#validate-ung-vien').validate({
        errorClass: 'errors',
        rules: {
            ho_ten_ung_vien: {
                required: true
            },
            so_dien_thoai: {
                required: true
            }
        },
        messages: {
            ho_ten_ung_vien: {
                required: "Bạn phải nhập tên ứng viên"
            },
            so_dien_thoai: {
                required: "Bạn phải nhập số điện thoại liên hệ"
            }
        }
    });
    callAPI(`${API_URL}/UngVien/view?id=${id}`,
        null,
        "GET",
        function(res) {
            if (res.success) {
                if (res.data != null && res.data != undefined && res.data != "") {

                    $("#ho_ten_ung_vien").val(res.data.ho_ten_ung_vien);
                    $("#so_dien_thoai").val(res.data.so_dien_thoai);
                    $("#email").val(res.data.email);
                    $("#dia_chi").val(res.data.dia_chi);
                    $("#vi_tri_ung_tuyen").val(res.data.vi_tri_ung_tuyen);
                    $("#noi_dung").val(res.data.noi_dung);
                    $("#ghi_chu_cuoi").val(res.data.ghi_chu_cuoi);
                    $("#luong_mong_muon").val(formatCurency(res.data.luong_mong_muon));
                    if (res.data.da_di_lam) {
                        $("#da_di_lam option[value='true']").prop('selected', true);
                    } else {
                        $("#da_di_lam option[value='false']").prop('selected', true);
                    }
                    $("#ngay_di_lam").val(epochToTime(res.data.ngay_di_lam));

                    for (var i = 0; i < res.data.thuoc_tinh.length; i++) {
                        let tt = res.data.thuoc_tinh[i];
                        $(`input[name='thuoc_tinh_nhom_${tt.nhom}'][value='${tt.gia_tri}']`).prop('checked', true);
                        $(`input[name^='thuoc_tinh'][value='${tt.gia_tri}']`).prop('checked', true);
                    }
                    $('#zalo').val(res.data.zalo);
                    $('#link_cv').val(res.data.link_cv);
                    $('#ngay_tuong_tac_cuoi').val(epochToTime(res.data.ngay_tuong_tac_cuoi));
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    $('#btn_upload').click(function () {
        var fd = new FormData();
        var files = $('#file_upload')[0].files[0];
        fd.append('file', files);
        fd.append('name', $("#ho_ten_ung_vien").val());
        $.ajax({
            url: '/ungvien/uploadcv',
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.success) {
                    $("#link_cv_upload").val(`${document.location.origin}/${res.file_path}`);
                    $("#link_cv").val(`${document.location.origin}/${res.file_path}`);
                } else {
                    $.notify(`${res.msg}`, "error");
                }
            }
        });
    });
});

function onUpdate(event) {
    event.preventDefault();
    if ($('#validate-ung-vien').valid()) {
        let ho_ten_ung_vien = $('#ho_ten_ung_vien').val();
        let so_dien_thoai = $('#so_dien_thoai').val();
        let email = $('#email').val();
        let dia_chi = $('#dia_chi').val();
        let vi_tri_ung_tuyen = $('#vi_tri_ung_tuyen').val();
        let noi_dung = $('#noi_dung').val();
        let ghi_chu_cuoi = $('#ghi_chu_cuoi').val();
        let luong_mong_muon = $('#luong_mong_muon').val();
        luong_mong_muon = replaceDot(luong_mong_muon);
        let da_di_lam = $('#da_di_lam').val();
        let ngay_di_lam = 0;
        try {
            if ($('#ngay_di_lam').val() !== "")
                ngay_di_lam = Math.floor(toDate($('#ngay_di_lam').val()) / 1000.0);
        } catch (e) {
        }

        let thuoc_tinh = [], thuoc_tinh_rieng = [];
        $("input[name^='thuoc_tinh']:checked").each(function (el) {
            try {
                if ($(this).attr('data-type') === '0')
                    thuoc_tinh.push(parseInt($(this).val()));
                else {
                    thuoc_tinh_rieng.push(parseInt($(this).val()));
                }
            } catch{
            }
        });
        let zalo = $('#zalo').val();
        let link_cv = $('#link_cv').val();
        let ngay_tuong_tac_cuoi = 0;
        try {
            if ($('#ngay_tuong_tac_cuoi').val() !== "")
                ngay_tuong_tac_cuoi = Math.floor(toDate($('#ngay_tuong_tac_cuoi').val()) / 1000.0);
        } catch (e) {
        }
        let note_ung_vien = $("#note-ung-vien").val();
        var obj = {
            "id_ung_vien": id,
            "ho_ten_ung_vien": ho_ten_ung_vien,
            "so_dien_thoai": so_dien_thoai,
            "email": email,
            "dia_chi": dia_chi,
            "vi_tri_ung_tuyen": vi_tri_ung_tuyen,
            "noi_dung": noi_dung,
            "ghi_chu_cuoi": ghi_chu_cuoi,
            "luong_mong_muon": luong_mong_muon,
            "da_di_lam": da_di_lam,
            "ngay_di_lam": ngay_di_lam,
            "thuoc_tinh": thuoc_tinh,
            "thuoc_tinh_rieng": thuoc_tinh_rieng,
            "zalo": zalo,
            "link_cv": link_cv,
            "ngay_tuong_tac_cuoi": ngay_tuong_tac_cuoi,
            "nguoi_sua": user,
            "note_ung_vien": note_ung_vien
        };

        callAPI(`${API_URL}/UngVien/${id}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Sửa thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    } else {
        $.notify(`Chưa đầy đủ thông tin`, "error");
    }
};

$("#link_cv_upload").click(function (event) {
    event.preventDefault();
    let link_cv_upload = document.getElementById("link_cv_upload");
    copyValue(link_cv_upload);
})


$("#ngay_di_lam").blur(function () {
    checkDayInput($("#ngay_di_lam"));
});


$("#ngay_tuong_tac_cuoi").blur(function () {
    checkDayInput($("#ngay_tuong_tac_cuoi"));
});


function getNoteUngVien() {
    callAPI(`${API_URL}/note/getnotebyobject?id_obj=${id}`, null, "GET", function (res) {
        if (res.success && res.data.length > 0) {
            let data_html = "<div id='div_ghi_chu_ung_vien' class='row'>";
            res.data.forEach(item => {
                if (item.ghi_chu != "") {
                    data_html += `<div class='col-sm-8'>${item.noi_dung}</div>`;
                    data_html += `<div class='col-sm-4'>${epochToTimeWithHour(item.ngay_tao)}</div>`;
                    
                }
            });
            data_html += `</div><hr>`;
            $("#ghi_chu_da_note").html(data_html);
        }
    });
}