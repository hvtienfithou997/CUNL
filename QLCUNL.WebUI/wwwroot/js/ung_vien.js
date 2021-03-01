$(document).ready(function () {
    let url = document.location.href;
    let auto_id = url.substring(url.lastIndexOf('=') + 1);
    $(".field-thuoc-tinh").append("<span type='button' class='float-right btn-danger remove-elem'> Bỏ chọn</span>");
    $(".remove-elem").click(function () {
        $(this).siblings('ul').find('input').prop('checked', false);
    });
    if (url.match(/\?./)) {
        let html = "";
        html += `<d><div class="form-group">`;
        html += `<b><code>JOB AUTO ID</code></b>`;
        html += `<input id="auto_id" class="form-control text-center" disabled value="${auto_id}" readonly/></div>`;

        $(".show-auto-id").html(html);
    }

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
        try {
            luong_mong_muon = parseFloat(luong_mong_muon)
            if (isNaN(luong_mong_muon))
                luong_mong_muon = 0;
        } catch (e) {
            luong_mong_muon = 0;
        }
        let da_di_lam = $('#da_di_lam').val();
        let ngay_di_lam = 0;
        if (da_di_lam !== "false")
            ngay_di_lam = Math.floor(toDate($('#ngay_di_lam').val()).getTime() / 1000.0);
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
        let auto_id = $("#auto_id").val();
        if (auto_id === null || auto_id === "" || auto_id === undefined) {
            callAPI(`${API_URL}/UngVien/add`, obj, "POST", function (res) {
                if (res.success) {
                    $.notify("Thêm thành công", "success");
                    $("form input").val("");
                    $("form textarea").val("");
                    try {
                        document.querySelector('input:checked').checked = false;
                    } catch (e) { }
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
            });
        } else {
            addUngvienByAutoID(obj, auto_id);
        }
    }
};

function addUngvienByAutoID(obj, autoid) {
    callAPI(`${API_URL}/UngVien/addungvienbyautoid?id_auto=${autoid}`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm ứng viên theo Auto_ID thành công", "success");
            $("form input").val("");
            $("form textarea").val("");
            document.querySelector('input:checked').checked = false;
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

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
                $("#ungvien_detail").append(`<li>Nội dung:<br> <span class="font-weight-bold">${urlify(res.data.noi_dung)}</span></li>`);
                $("#ungvien_detail").append(`<li>Ghi chú cuối: <span class="font-weight-bold">${res.data.ghi_chu_cuoi}</span></li>`);
                let luong = res.data.luong_mong_muon;
                luong = luong.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
                $("#ungvien_detail").append(`<li>Lương mong muốn: <span class="font-weight-bold">${luong}</span></li>`);
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
                $("#ungvien_detail").append(`<li>Link CV: <span class="font-weight-bold"><a target="_blank" href="${res.data.link_cv}">${res.data.link_cv}</a></span><button class="btn btn-info" id="preview" style="float:right">Xem trước</button></li>`);

                $("#ungvien_detail").append(`<li>Ngày tạo: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_tao)}</span></li>`);
                $("#ungvien_detail").append(`<li>Người tạo: <span class="font-weight-bold">${res.data.nguoi_tao}</span></li>`);
                $("#ungvien_detail").append(`<li>Ngày sửa: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_sua)}</span></li>`);
                $("#ungvien_detail").append(`<li>Người sửa: <span class="font-weight-bold">${res.data.nguoi_sua}</span></li>`);
                $('#preview').click(function () {
                    var url_temp = "";
                    let url = res.data.link_cv;
                    var ext = url.split('.').pop();

                    let type_file = ["pdf", "docx", "pptx"];
                    if (type_file.includes(ext)) {
                    }
                    url_temp = ext !== "pdf" ? "https://view.officeapps.live.com/op/view.aspx?src=" + url : url;
                    
                    if (url.length > 0) {
                        $("#ungvien_detail").append(`<li><iframe id="iframe-cv" src="${url_temp}"></iframe></li>`);
                    }
                    else {
                        $("#ungvien_detail").append("<li><p>Chưa có link!</p></li>");
                    }
                });

                $("#ungvien_detail").children().addClass('list-group-item');
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

$("#link_cv_upload").click(function (event) {
    event.preventDefault();
    let link_cv_upload = document.getElementById("link_cv_upload");
    copyValue(link_cv_upload);
});

$("#ngay_di_lam").blur(function () {
    checkDayInput($("#ngay_di_lam"));
});