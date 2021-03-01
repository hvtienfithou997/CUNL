var listungvienchecked = [];
function search(page) {
    let term = $(`[name='term']`).val();
    let thuoc_tinh = []; let thuoc_tinh_rieng = [];
    if (typeof page === 'undefined') {
        page = 1;
    }
    $("input[name^='thuoc_tinh']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng.push(parseInt($(this).val()));
        } catch{
        }
    });
    let thuoc_tinh_string = thuoc_tinh.join(',');
    let thuoc_tinh_rieng_string = thuoc_tinh_rieng.join(',');

    let id_ung_vien = $('#id_ung_vien').val();
    let ngay_di_lam_from = 0;
    let ngay_di_lam_to = 0; let ngay_tao_from = 0; let ngay_tao_to = 0;

    if ($('#ngay_di_lam_from').val() != "") {
        ngay_di_lam_from = Math.floor(toDate($('#ngay_di_lam_from').val()) / 1000.0);
    }
    if ($('#ngay_di_lam_to').val() != "") {
        ngay_di_lam_to = Math.floor(toDate($('#ngay_di_lam_to').val()) / 1000.0);
    }
    if ($('#ngay_tao_from').val() != "") {
        ngay_tao_from = Math.floor(toDate($('#ngay_tao_from').val()) / 1000.0);
    }

    if ($('#ngay_tao_to').val() != "") {
        ngay_tao_to = Math.floor(toDate($('#ngay_tao_to').val()) / 1000.0);
    }
    let luong_mong_muon_from = $('#luong_mong_muon_from').val() != "" ? parseInt($('#luong_mong_muon_from').val()) : 0;
    let luong_mong_muon_to = $('#luong_mong_muon_to').val() != "" ? parseInt($('#luong_mong_muon_to').val()) : 0;

    callAPI(`${API_URL}/UngVien/free?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string}&term=${term}&id_ung_vien=${id_ung_vien}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${ngay_di_lam_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to}&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        console.log(res);

        $("#div_data").html(DIV_LOADING);
        if (res.success) {
            $("#div_data").empty();

            if (res.data != null && res.data.length > 0) {

                let html_data = "";
                res.data.forEach(item => {

                    html_data += `<tr>`;
                    html_data += `<td><input type="checkbox" value="${item.id_ung_vien}" name="ungvienUnchecked" onclick="ungvienchecked()" id="ungvienUnchecked"></td>`;
                    html_data += `<td><a href="#" data-toggle="modal" data-target="#modal_detail" id="${item.id_ung_vien}" onclick="showDetailUv(this.id)">${item.ho_ten_ung_vien}<br><ul><li><span class="font-weight-bold">${item.so_dien_thoai}</span></li><li><span class="font-weight-bold">${item.email}</span></li></ul></td></a>`;
                    html_data += `<td>${item.thuoc_tinh}</td>`;
                    html_data += `<td>${item.dia_chi}</td>`;
                    html_data += `<td>${item.vi_tri_ung_tuyen}</td>`;
                    html_data += `<td>${item.ghi_chu_cuoi}</td>`;
                    html_data += `<td><button class="btn btn-primary" onclick="onOneAssign('${item.id_ung_vien}')">Gán</button></td>`;
                    html_data += `</tr>`;
                    
                });

                $("#div_data").html(html_data);
            }
            paging(res.total, 'search', page);
        } else {
            $("#div_data").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};
function showDetailUv(id) {
    callAPI(`${API_URL}/UngVien/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#ungvien_detail").empty();
                //$("#ungvien_detail").append(`<li>Id: <span class="font-weight-bold">${res.data.id_ung_vien}</span></li>`);
                $("#ungvien_detail").append(`<li>Họ và tên: <span class="font-weight-bold">${res.data.ho_ten_ung_vien}</span> || Số điện thoại: <span class="font-weight-bold">${res.data.so_dien_thoai}</span></li>`);
                //$("#ungvien_detail").append(`<li>Số điện thoại: <span class="font-weight-bold">${res.data.so_dien_thoai}</span></li>`);
                $("#ungvien_detail").append(`<li>Email: <span class="font-weight-bold">${res.data.email}</span> || Vị trí ứng tuyển: <span class="font-weight-bold">${res.data.vi_tri_ung_tuyen}</span></li>`);
                $("#ungvien_detail").append(`<li>Địa chỉ: <span class="font-weight-bold">${res.data.dia_chi}</span> || Zalo: <span class="font-weight-bold">${res.data.zalo}</span></li>`);
                //$("#ungvien_detail").append(`<li>Vị trí ứng tuyển: <span class="font-weight-bold">${res.data.vi_tri_ung_tuyen}</span></li>`);
                $("#ungvien_detail").append(`<li>Nội dung: <span class="font-weight-bold">${res.data.noi_dung}</span></li>`);
                $("#ungvien_detail").append(`<li>Ghi chú cuối: <span class="font-weight-bold">${res.data.ghi_chu_cuoi}</span></li>`);
                $("#ungvien_detail").append(`<li>Lương mong muốn: <span class="font-weight-bold">${res.data.luong_mong_muon}</span></li>`);
                res.data.da_di_lam = res.data.da_di_lam ? "Đã đi làm":  "Chưa đi làm";
                $("#ungvien_detail").append(`<li>Đã đi làm: <span class="font-weight-bold">${res.data.da_di_lam}</span> || Ngày đi làm: <span class="font-weight-bold">${epochToTime(res.data.ngay_di_lam)}</span></li>`);
                //$("#ungvien_detail").append(`<li>Ngày đi làm: <span class="font-weight-bold">${epochToTime(res.data.ngay_di_lam)}</span></li>`);
                
                let html_thuoc_tinh = "<ul>";
                if (res.data.thuoc_tinh != null) {
                    res.data.thuoc_tinh.forEach(tt => {
                        html_thuoc_tinh += `<li><span class="font-weight-bold">${tt.ten}</span></li>`;
                    });
                }
                html_thuoc_tinh += `</ul>`;
                $("#ungvien_detail").append(`<li>Thuộc tính: <span class="font-weight-bold">${html_thuoc_tinh}</span></li>`);
                
                //$("#ungvien_detail").append(`<li></li>`);
                $("#ungvien_detail").append(`<li>Link CV: <span class="font-weight-bold">${res.data.link_cv}</span></li>`);
                
               
                $("#ungvien_detail").append(`<li>Ngày tương tác cuối: <span class="font-weight-bold">${epochToTime(res.data.ngay_tuong_tac_cuoi)}</span></li>`);
                $("#ungvien_detail").children().addClass('list-group-item');
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
    callAPI(`${API_URL}/NoteUngVien/search?term=&id_ung_vien=${id}&thuoc_tinh=&thuoc_tinh_rieng=&page=1`, null, "GET", function (res) {
        if (res.success) {
            let data_html = "<h3>Ghi chú ứng viên</h3>";
            data_html += `<ul class="row ">`;
                data_html += `<li class="col-sm-7 note_uv"><b>Nội dung</b></li>`;
                data_html += `<li class="col-sm-2"><b>Ngày tạo</b></li>`;
                data_html += `<li class="col-sm-2"><b>Người tạo</b></li>`;
                data_html += `</ul>`;
            res.data.forEach(item => {
                data_html += `<ul class="row ">`;
                data_html += `<li class="col-sm-7 note_uv">${item.ghi_chu}</li>`;
                data_html += `<li class="col-sm-2">${epochToTime(item.ngay_tao)}</li>`;
                data_html += `<li class="col-sm-2">${item.nguoi_tao}</li>`;
                data_html += `</ul>`;
                

            });
            $('#note_ung_vien').html(data_html);
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
    enableDisableButton();
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
    ungvienchecked();
    if (listungvienchecked.length > 0) {
        var obj = { "id_user_job": id_user_job, "id_job": id_job, "id_ung_viens": listungvienchecked }

        callAPI(`${API_URL}/NoteUngVienJob/grant`, obj, "POST", function (res) {
            if (res.success) {
                $.notify("Gán thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
    else {
        $.notify(`Vui lòng chọn ứng viên`, "error");
    }

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
function onCheckAll() {
    $("input[type='checkbox']", "#div_data").prop("checked", $("#check_all").is(":checked"));
    enableDisableButton();
}
function enableDisableButton() {
    if ($("input[type='checkbox']:checked", "#div_data").length > 0) {
        $("#btn_grant_all").removeClass("disabled");
    } else {
        $("#btn_grant_all").addClass("disabled");
    }
}