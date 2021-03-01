var listungvienchecked = [];

function onAssignJobs_UngVien() {
    if (listjobchecked.length > 0) {
        if (listungvienchecked.length > 0) {
            $.each(listjobchecked, function (index, value) {
                var obj = { "id_user_job": '', "id_job": value, "id_ung_viens": listungvienchecked }

                callAPI(`${API_URL}/NoteUngVienJob/grant`, obj, "POST", function (res) {
                    if (res.success) {
                        $.notify("Gán thành công", "success");
                    } else {
                        $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                    }
                });
            });
        }
        else {
            $.notify(`Vui lòng chọn ứng viên`, "error");
        }
    }
    else {
        $.notify(`Vui lòng chọn job`, "error");
    }
}

function onOneAssignJobs_UngVien(id_ung_vien) {
    if (listjobchecked.length > 0) {
        let list_uv = [];
        list_uv.push(id_ung_vien);
        if (list_uv.length > 0) {
            $.each(listjobchecked, function (index, value) {
                var obj = { "id_user_job": '', "id_job": value, "id_ung_viens": list_uv }
                callAPI(`${API_URL}/NoteUngVienJob/grant`, obj, "POST", function (res) {
                    if (res.success) {
                        $.notify("Gán thành công", "success");
                    } else {
                        $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                    }
                });
            });
        }
        else {
            $.notify(`Vui lòng chọn ứng viên`, "error");
        }
    }
    else {
        $.notify(`Vui lòng chọn job`, "error");
    }
}

function search1() {
    let thuoc_tinh = []; let thuoc_tinh_rieng = [];
    //$("input[name^='thuoc_tinh']:checked").each(function (el) {
    //    try {
    //        if ($(this).attr('data-type') === '0')
    //            thuoc_tinh.push(parseInt($(this).val()));
    //        else
    //            thuoc_tinh_rieng.push(parseInt($(this).val()));
    //    } catch{
    //    }
    //});
    let thuoc_tinh_string = '';
    let thuoc_tinh_rieng_string = '';

    let term = $('#term1').val();
    let id_ung_vien = $('#id_ung_vien1').val();
    let ngay_di_lam_from = 0;
    let ngay_di_lam_to = 0; let ngay_tao_from = 0; let ngay_tao_to = 0;

    if ($('#ngay_di_lam_from1').val() != "") {
        ngay_di_lam_from = Math.floor(toDate($('#ngay_di_lam_from1').val()).getTime() / 1000.0);
    }
    if ($('#ngay_di_lam_to1').val() != "") {
        ngay_di_lam_to = Math.floor(toDate($('#ngay_di_lam_to1').val()).getTime() / 1000.0);
    }
    if ($('#ngay_tao_from1').val() != "") {
        ngay_tao_from = Math.floor(toDate($('#ngay_tao_from1').val()).getTime() / 1000.0);
    }

    if ($('#ngay_tao_to1').val() != "") {
        ngay_tao_to = Math.floor(toDate($('#ngay_tao_to1').val()).getTime() / 1000.0);
    }
    let luong_mong_muon_from = $('#luong_mong_muon_from1').val() != "" ? parseInt($('#luong_mong_muon_from1').val()) : 0;
    let luong_mong_muon_to = $('#luong_mong_muon_to1').val() != "" ? parseInt(('#luong_mong_muon_to1').val()) : 0;

    callAPI(`${API_URL}/UngVien/search?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string}&term=${term}&id_ung_vien=${id_ung_vien}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${ngay_di_lam_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to}&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&page=1`, null, "GET", function (res) {
        $("#div_data1").html(DIV_LOADING);
        if (res.success) {
            $("#div_data1").empty();
            if (res.data != null && res.data.length > 0) {

                let html_data = "";
                res.data.forEach(item => {
                    html_data += `<tr>`;
                    html_data += `<td><input type="checkbox" value="${item.id_ung_vien}" name="ungvienUnchecked" onclick="ungvienchecked()" id="ungvienUnchecked"></td>`;
                    html_data += `<td>${item.ho_ten_ung_vien}</td>`;
                    html_data += `<td>${item.so_dien_thoai}</td>`;
                    html_data += `<td>${item.email}</td>`;
                    html_data += `<td>${item.dia_chi}</td>`;
                    html_data += `<td>${item.vi_tri_ung_tuyen}</td>`;
                    html_data += `<td>${item.ghi_chu_cuoi}</td>`;

                    html_data += `<td><button class="btn btn-primary" onclick="onOneAssignJobs_UngVien('${item.id_ung_vien}')">Gán</button></td>`;
                    html_data += `</tr>`;
                });
                $("#div_data1").html(html_data);
            }
        } else {
            $("#div_data1").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function onShowThuocTinh(id) {

    callAPI(`${API_URL}/thuoctinh/canhan?loai=3`, null, "GET", function (res) {
        if (res.success) {

            let thuoc_tinh = '';
            thuoc_tinh = `<input class="hide" id="id_obj" value="${id}">`;
            res.data.forEach((item) => {
                thuoc_tinh += "<ul class='check-box row'>";
                thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau' value='${item.gia_tri}'> <span class="font-weight-bold">${item.ten}</span></li>`;
                thuoc_tinh += "</ul>";


            });
            $('#thuoc_tinh_du_lieu').html(thuoc_tinh);
            $('.hide').css('display', 'none');
        }
    });
}
function onCreateThuocTinh(loai_obj) {
    let id = $('#id_obj').val();
    let thuoc_tinh = [];
    $("input[name='thuoc_tinh_danh_dau']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch{
        }
    });

    let obj = { "loai_obj": loai_obj, "id_obj": id, "thuoc_tinh": thuoc_tinh, "nguoi_tao": user, "nguoi_sua": user }

    callAPI(`${API_URL}/thuoctinhdulieu/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function ungvienchecked() {
    listungvienchecked = [];
    $("input[name='ungvienUnchecked']:checked").each(function (el) {
        try {
            listungvienchecked.push($(this).val());
        } catch{
        }
    });
}

function onOneAssign(id_ung_vien) {
    let arr_id_ung_vien = [];
    arr_id_ung_vien.push(id_ung_vien);
    var obj = {
        "id_user_job": id_user_job, "id_job": id_job, "id_ung_viens": arr_id_ung_vien
    };

    callAPI(`${API_URL}/NoteUngVienJob/grant`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Gán thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });


}

function onAssign(id_user_job, id_job) {
    var obj = { "id_user_job": id_user_job, "id_job": id_job, "id_ung_viens": listungvienchecked }

    callAPI(`${API_URL}/NoteUngVienJob/grant`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Gán thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });

}
function searchBy() {
    let value = $('#type-search').val();
    if (value == "byid") {
        $('#id_ung_vien').removeClass('d-none');
        $('#ngay_di_lam_from').addClass('d-none');
        $('#ngay_di_lam_to').addClass('d-none');
        $('#luong_mong_muon_from').addClass('d-none');
        $('#luong_mong_muon_to').addClass('d-none');
        $('#ngay_tao_from').addClass('d-none');
        $('#ngay_tao_to').addClass('d-none');
    } else if (value == "bydaywork") {
        $('#ngay_di_lam_from').removeClass('d-none');
        $('#ngay_di_lam_to').removeClass('d-none');
        $('#luong_mong_muon_from').addClass('d-none');
        $('#luong_mong_muon_to').addClass('d-none');
        $('#ngay_tao_from').addClass('d-none');
        $('#ngay_tao_to').addClass('d-none');
        $('#id_ung_vien').addClass('d-none');
    } else if (value == "bysalary") {
        $('#luong_mong_muon_from').removeClass('d-none');
        $('#luong_mong_muon_to').removeClass('d-none');
        $('#ngay_tao_from').addClass('d-none');
        $('#ngay_tao_to').addClass('d-none');
        $('#id_ung_vien').addClass('d-none');
        $('#ngay_di_lam_from').addClass('d-none');
        $('#ngay_di_lam_to').addClass('d-none');
    } else if (value == "bydaycreate") {
        $('#ngay_tao_from').removeClass('d-none');
        $('#ngay_tao_to').removeClass('d-none');
        $('#id_ung_vien').addClass('d-none');
        $('#ngay_di_lam_from').addClass('d-none');
        $('#ngay_di_lam_to').addClass('d-none');
        $('#luong_mong_muon_from').addClass('d-none');
        $('#luong_mong_muon_to').addClass('d-none');
    } else {
        $('.search-form').children().addClass('d-none');
    }
}

