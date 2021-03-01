$(function () {
    $('#validate-ung-vien').validate({
        errorClass: 'errors',
        rules: {
            ho_ten_ung_vien: {
                required: true
            },
            so_dien_thoai: {
                required: true
            },
            email: {
                required: true
            },
            dia_chi: {
                required: true
            },
            vi_tri_ung_tuyen: {
                required: true
            },
            luong_mong_muon: {
                required: true
            }
        },
        messages: {
            ho_ten_ung_vien: {
                required: "Bạn phải nhập tên ứng viên"
            },
            so_dien_thoai: {
                required: "Bạn phải nhập số điện thoại liên hệ"
            },
            email: {
                required: 'Bạn phải nhập email ứng viên'
            },
            dia_chi: {
                required: 'Bạn phải nhập địa chỉ'
            },
            vi_tri_ung_tuyen: {
                required: "Bạn phải nhập vị trí ứng tuyển"
            },
            luong_mong_muon: {
                required: "Bạn phải nhập lương mong muốn của ứng viên"
            }
        }
    });
    callAPI(`${API_URL}/UngVien/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {

                $("#ho_ten_ung_vien").val(res.data.ho_ten_ung_vien);
                $("#so_dien_thoai").val(res.data.so_dien_thoai);
                $("#email").val(res.data.email);
                $("#dia_chi").val(res.data.dia_chi);
                $("#vi_tri_ung_tuyen").val(res.data.vi_tri_ung_tuyen);
                $("#noi_dung").val(res.data.noi_dung);
                $("#ghi_chu_cuoi").val(res.data.ghi_chu_cuoi);
                $("#luong_mong_muon").val(res.data.luong_mong_muon);
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
})

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
            "nguoi_sua": user
        };

        callAPI(`${API_URL}/UngVien/${id}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Sửa thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};