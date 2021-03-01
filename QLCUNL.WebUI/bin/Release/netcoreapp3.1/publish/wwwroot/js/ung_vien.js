$(document).ready(function () {
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
});
function onSubmit(event) {
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
        if (da_di_lam !== "false")
            ngay_di_lam = Math.floor(new Date($('#ngay_di_lam').val()) / 1000.0);
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
        let ngay_tuong_tac_cuoi = Math.floor(new Date() / 1000.0);

        var obj = {
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
            "nguoi_tao": user,
            "nguoi_sua": user
        };

        callAPI(`${API_URL}/UngVien/add`, obj, "POST", function (res) {
            if (res.success) {
                $.notify("Thêm thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};
function bindDetail(id) {
    callAPI(`${API_URL}/UngVien/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#ungvien_detail").empty();
                //$("#ungvien_detail").append(`<li>Id: <span class="font-weight-bold">${res.data.id_ung_vien}</span></li>`);
                $("#ungvien_detail").append(`<li>Họ và tên: <span class="font-weight-bold">${res.data.ho_ten_ung_vien}</span></li>`);
                $("#ungvien_detail").append(`<li>Số điện thoại: <span class="font-weight-bold">${res.data.so_dien_thoai}</span></li>`);
                $("#ungvien_detail").append(`<li>Email: <span class="font-weight-bold">${res.data.email}</span></li>`);
                $("#ungvien_detail").append(`<li>Địa chỉ: <span class="font-weight-bold">${res.data.dia_chi}</span></li>`);
                $("#ungvien_detail").append(`<li>Vị trí ứng tuyển: <span class="font-weight-bold">${res.data.vi_tri_ung_tuyen}</span></li>`);
                $("#ungvien_detail").append(`<li>Nội dung: <span class="font-weight-bold">${res.data.noi_dung}</span></li>`);
                $("#ungvien_detail").append(`<li>Ghi chú cuối: <span class="font-weight-bold">${res.data.ghi_chu_cuoi}</span></li>`);
                $("#ungvien_detail").append(`<li>Lương mong muốn: <span class="font-weight-bold">${res.data.luong_mong_muon}</span></li>`);
                res.data.da_di_lam = res.data.da_di_lam ? "Đã đi làm" : "Chưa đi làm";
                $("#ungvien_detail").append(`<li>Đã đi làm: <span class="font-weight-bold">${res.data.da_di_lam}</span></li>`);
                $("#ungvien_detail").append(`<li>Ngày đi làm: <span class="font-weight-bold">${epochToTime(res.data.ngay_di_lam)}</span></li>`);

                let html_thuoc_tinh = "<ul>";
                if (res.data.thuoc_tinh != null) {
                    res.data.thuoc_tinh.forEach(tt => {
                        html_thuoc_tinh += `<li><span class="font-weight-bold">${tt.ten}</span></li>`;
                    });
                }
                html_thuoc_tinh += `</ul>`;
                $("#ungvien_detail").append(`<li>Thuộc tính: <span class="font-weight-bold">${html_thuoc_tinh}</span></li>`);

                $("#ungvien_detail").append(`<li>Zalo: <span class="font-weight-bold">${res.data.zalo}</span></li>`);
                $("#ungvien_detail").append(`<li>Ngày tương tác cuối: <span class="font-weight-bold">${epochToTime(res.data.ngay_tuong_tac_cuoi)}</span></li>`);
                $("#ungvien_detail").append(`<li>Link CV: <span class="font-weight-bold">${res.data.link_cv}</span><button class="btn btn-info" id="preview" style="float:right">Xem trước</button></li>`);
                $('#preview').click(function () {
                    var url_temp = "";
                    let url = res.data.link_cv;
                    var ext = url.split('.').pop();
                    url_temp = ext == "pdf" ? url : "https://view.officeapps.live.com/op/view.aspx?src=" + url;
                    $("#ungvien_detail").append(`<li><iframe id="iframe-cv" src="${url_temp}"></iframe></li>`);
                });

                $("#ungvien_detail").children().addClass('list-group-item');
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}