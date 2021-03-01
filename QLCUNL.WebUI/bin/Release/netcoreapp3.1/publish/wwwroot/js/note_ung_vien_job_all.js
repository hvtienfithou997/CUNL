function search(page) {
    let term = $(`[name='term']`).val();
    if (typeof page === 'undefined') {
        page = 1;
    }
    let thuoc_tinh = []; let thuoc_tinh_rieng = [];
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



    let ngay_gio_phong_van_from = 0;
    let ngay_gio_phong_van_to = 0;
    let ngay_di_lam_from = 0;
    let ngay_di_lam_to = 0;
    let luong_thu_viec_from = $('#luong_thu_viec_from').val() != "" ? $('#luong_thu_viec_from').val() : 0;
    let luong_thu_viec_to = $('#luong_thu_viec_to').val() != "" ? $('#luong_thu_viec_to').val() : 0;
    let luong_chinh_thuc_from = $('#luong_chinh_thuc_from').val() != "" ? $('#luong_chinh_thuc_from').val() : 0;
    let luong_chinh_thuc_to = $('#luong_chinh_thuc_to').val() != "" ? $('#luong_chinh_thuc_to').val() : 0;
    if ($('#ngay_gio_phong_van_from').val() != "") {
        ngay_gio_phong_van_from = Math.floor(new Date($('#ngay_gio_phong_van_from').val()) / 1000.0);
    }
    if ($('#ngay_gio_phong_van_to').val() != "") {
        ngay_gio_phong_van_to = Math.floor(new Date($('#ngay_gio_phong_van_to').val()) / 1000.0);
    }
    if ($('#ngay_di_lam_from').val() != "") {
        ngay_di_lam_from = Math.floor(new Date($('#ngay_di_lam_from').val()) / 1000.0);
    }
    if ($('#ngay_di_lam_to').val() != "") {
        ngay_di_lam_to = Math.floor(new Date($('#ngay_di_lam_to').val()) / 1000.0);
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
    if (typeof term === 'undefined') {
        term = '';
    }
    if (typeof id_ung_vien === 'undefined' || typeof id_ung_vien === 'object') {
        id_ung_vien = '';
    }
    if (typeof id_user === 'undefined' || typeof id_user === 'object') {
        id_user = '';
    }
    if (id_job === '' || typeof id_job === 'object') {
        id_job = $('#id_job').val();
    }

    if (isNaN(ngay_gio_phong_van_from)) {
        ngay_gio_phong_van_from = 0;
    }
    if (isNaN(ngay_gio_phong_van_to)) {
        ngay_gio_phong_van_to = 0;
    }
    if (isNaN(ngay_di_lam_from)) {
        ngay_di_lam_from = 0;
    }
    if (isNaN(ngay_di_lam_to)) {
        ngay_di_lam_to = 0;
    }
    if (typeof luong_thu_viec_from === 'undefined') {
        luong_thu_viec_from = 0;
    }
    if (typeof luong_thu_viec_to === 'undefined') {
        luong_thu_viec_to = 0;
    }
    if (typeof luong_chinh_thuc_from === 'undefined') {
        luong_chinh_thuc_from = 0;
    }
    if (typeof luong_chinh_thuc_to === 'undefined') {
        luong_chinh_thuc_to = 0;
    }
    console.log(id_job + id_ung_vien);
    callAPI(
        `${API_URL}/NoteUngVienJob/search?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string
        }&term=${term}&id_user=${id_user}&id_job=${id_job}&id_ung_vien=${id_ung_vien}&ngay_gio_phong_van_from=${
        ngay_gio_phong_van_from}&ngay_gio_phong_van_to=${ngay_gio_phong_van_to}&ngay_di_lam_from=${ngay_di_lam_from
        }&ngay_di_lam_to=${ngay_di_lam_to}&luong_thu_viec_from=${luong_thu_viec_from}&luong_thu_viec_to=${
        luong_thu_viec_to}&luong_chinh_thuc_from=${luong_chinh_thuc_from}&luong_chinh_thuc_to=${luong_chinh_thuc_to
        }&page=${page}&page_size=${PAGE_SIZE}`,
        null,
        "GET",
        function (res) {
            if (res.success) {
                $("#div_data").empty();
                $(".totalRecs").html("Tổng số bản ghi: " + res.total);
                if (res.data != null && res.data.length > 0) {
                    let html_data = "";
                    res.data.forEach(item => {
                        html_data += `<tr>`;
                        html_data += `<td class="d-none"><input type="type" id="id_note_uv_job" value="${item.id_note_ung_vien_job}"/></td>`;
                        html_data += `<td><a href="/ungvien/detail/${item.id_ung_vien}" target="_blank">${item.ho_ten_ung_vien}<br>- ${item.so_dien_thoai}<br>- ${item.email}<br>- ${item.zalo}</a></td>`;
                        html_data += `<td><a href="/job/detail/${item.id_job}" target="_blank">${item.ten_job}<br>${item.cong_ty}</a></td>`;

                        let html_thuoc_tinh = "";
                        if (item.thuoc_tinh != null) {
                            item.thuoc_tinh.forEach(tt => {
                                html_thuoc_tinh += `<p>- ${tt.ten}</p>`;
                            });
                        }
                        html_data += `<td style='vertical-align:bottom'>${html_thuoc_tinh}`;
                        html_data += `<button class="btn btn-secondary" data-toggle="modal" data-tooltip="tooltip" title="Thay đổi trạng thái ứng viên!" data-target="#myStatus" id="${item.id_note_ung_vien_job}" onclick="onShowTrangThai(this.id)">Thay đổi</button></td >`;
                        html_data += `<td style='vertical-align:bottom'>${item.ngay_gio_phong_van > 0 ? "<p>" + epochToTimeWithHour(item.ngay_gio_phong_van) + "</p>" : ""}`;
                        html_data += `<button class="btn btn-primary" data-toggle="modal" data-tooltip="tooltip" title="Thêm giờ phỏng vấn!" data-target="#addDate" id="${item.id_note_ung_vien_job}" onclick="onShowGioPhongVan(this.id)">Tạo giờ</button></td>`;
                        html_data += `<td width:150px>${item.ngay_di_lam > 0 ? epochToTime(item.ngay_di_lam) : ""}</td>`;
                        html_data += `<td>${item.luong_thu_viec ? formatCurency(item.luong_thu_viec) : ""}/${item.luong_thu_viec > 0 ? formatCurency(item.luong_chinh_thuc) : ""}</td>`;
                        html_data += `<td>${item.ghi_chu !== null ? item.ghi_chu : ""}</td>`;
                        html_data += `<td><a class="btn btn-warning btn-big btn-w1" href="edit?id=${item.id_note_ung_vien_job}">Sửa</a>`;
                        html_data += `&nbsp;<a class="btn btn-success btn-big btn-w1" href="detail/${item.id_note_ung_vien_job}">Xem</a>`;
                        html_data += `&nbsp;<a class="btn btn-info btn-big" href="share?id=${item.id_note_ung_vien_job}">Chia sẻ</a>`;
                        html_data += `<button class="btn btn-dark modall btn-big btn-w2" id="${item.id_note_ung_vien_job}" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>`;
                        html_data += `<button class="btn btn-danger btn-s-small" id="${item.id_note_ung_vien_job}" onclick="deleteRecs(this.id)">Xóa</button></td>`;

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
    //
    // Lấy chi tiết job được chọn
    

}
function detailJob(id_job) {
    if (id_job.length > 0) {
        callAPI(`${API_URL}/Job/view?id=${id_job}`, null, "GET", function (res) {
            if (res.success) {
                console.log(res.data);
                if (res.data != null && res.data != undefined && res.data != "") {
                    let html_job = "";
                    let html_cong_ty = "";
                    html_job += `<h6>Vị trí: <strong>${res.data.chuc_danh}</strong></h6>`;
                    html_job += `<h6>Người liên hệ: <strong>${res.data.nguoi_lien_he}</strong></h6>`;
                    html_job += `<h6>Lương chính thức: <strong>${formatCurency(res.data.luong_chinh_thuc)} VND</strong></h6>`;
                    res.data.tinh_thanh.forEach((item) => {
                        html_job += `<h6>Địa chỉ Job: <strong>${item.ten_tinh}</strong></h6>`;
                    })
                    html_job += `<h6>Số lượng: <strong>${res.data.so_luong} (người)</strong></h6>`;

                    html_cong_ty += `<h6>Tên công ty: <strong>${res.data.cong_ty.ten_cong_ty}</strong></h6>`;
                    html_cong_ty += `<h6>Địa chỉ: <strong>${res.data.cong_ty.dia_chi}</strong></h6>`;
                    html_cong_ty += `<h6>Điện thoại: <strong>${res.data.cong_ty.dien_thoai}</strong></h6>`;
                    html_cong_ty += `<h6>Người gửi HĐ: <strong>${res.data.cong_ty.info_gui_hop_dong}</strong></h6>`;
                    $('.info-job-left').html(html_job);
                    $('.info-job-right').html(html_cong_ty);
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
}
function onShowThuocTinh(id) {
    callAPI(`${API_URL}/thuoctinh/canhan?loai=5&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let thuoc_tinh = '';
            thuoc_tinh = `<input class="d-none" id="id_obj" value="${id}">`;

            var result = [];
            res.data.forEach(function (hash) {
                return function (a) {
                    if (!hash[a.nhom]) {
                        hash[a.nhom] = { nhom: a.nhom, activities: [] };
                        result.push(hash[a.nhom]);
                    }
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom == 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_danh_dau' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                }
            });
            $('#thuoc_tinh_du_lieu').html(thuoc_tinh);

            for (var i = 0; i < res.value.length; i++) {
                $(`input[name='thuoc_tinh_danh_dau'][value='${res.value[i]}']`).prop('checked', true);
            }
            $('#thuoc_tinh_du_lieu').append(`<a href="/thuoctinh/add"><i class="icon-add"></i>Thêm thuộc tính</a>`);
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
            $.notify("Thành công", "success");
            $('#myModal').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function onShowTrangThai(id) {
    callAPI(`${API_URL}/thuoctinh/shared?loai=5&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let trang_thai = `<input class="d-none" id="id_note_uv_job_modal" value="${id}">`;
            var result = [];
            res.data.forEach(function (hash) {
                return function (a) {
                    if (!hash[a.nhom]) {
                        hash[a.nhom] = { nhom: a.nhom, activities: [] };
                        result.push(hash[a.nhom]);
                    }
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom == 0) {
                    trang_thai += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        trang_thai += `<div class="col-md-12"><input type='checkbox' name='update_trang_thai' value='${child.gia_tri}'> ${child.ten} </div>`;
                    });
                } else {
                    trang_thai += `Nhóm ${item.nhom}`;
                    trang_thai += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        trang_thai += `<li class='col-md-12'><input type='radio' name='update_trang_thai' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    trang_thai += "</ul>";
                }
            });
            $('#status').html(trang_thai);
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name='update_trang_thai'][value='${res.value[i]}']`).prop('checked', true);
            }

            $('#thuoc_tinh_du_lieu').append(`<a href="/thuoctinh/add"><i class="icon-add"></i>Thêm thuộc tính</a>`);
        }
    });
}

function onSaveTrangThai() {
    let id_note_ung_vien_job = $('#id_note_uv_job_modal').val();
    let thuoc_tinh = [];
    $("input[name='update_trang_thai']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch{
        }
    });
    var obj = {
        "thuoc_tinh": thuoc_tinh
    }
    callAPI(`${API_URL}/noteungvienjob/trangthai?id=${id_note_ung_vien_job}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Thành công", "success");
            $('#myStatus').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function onShowGioPhongVan(id) {
    $("#id_note_uv_job_pv").val(id);
    let dt = new Date();
    dt.setDate(dt.getDate() + 1);
    let date_str = `${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}`;
    date_str += ` ${dt.getDate().toString().padStart(2, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getFullYear().toString().padStart(4, '0')}`;

    $("#gio_phong_van").val(`${date_str}`);
}
function onSaveGioPhongVan() {
    let id = $('#id_note_uv_job_pv').val();
    let time = 0;
    if (time !== "") {
        time = Math.floor(new Date(toDateWithHour($('#gio_phong_van').val())) / 1000.0);
    }
    var obj = {
        "ngay_gio_phong_van": time
    }
    callAPI(`${API_URL}/noteungvienjob/giophongvan?id=${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Đặt giờ phỏng vấn thành công", "success");
            $('#addDate').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function deleteRecs(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/noteungvienjob/delete?id=${id}`,
            null,
            "DELETE",
            function (res) {
                if (res) {
                    $.notify("Xóa thành công", "success");
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
            });
    } else {
        return false;
    }
};

function searchBy() {
    let value = $('#type-search').val();
    if (value == "byid") {
        $('.search-by-id').removeClass('d-none');
        $('.search-by-datetime').addClass('d-none');
        $('.search-by-daywork').addClass('d-none');
        $('.search-by-salary').addClass('d-none');
        $('.search-by-salarymain').addClass('d-none');
    } else if (value == "bydayinterview") {
        $('.search-by-id').addClass('d-none');
        $('.search-by-datetime').removeClass('d-none');
        $('.search-by-daywork').addClass('d-none');
        $('.search-by-salary').addClass('d-none');
        $('.search-by-salarymain').addClass('d-none');
    } else if (value == "bydaywork") {
        $('.search-by-daywork').removeClass('d-none');
        $('.search-by-id').addClass('d-none');
        $('.search-by-datetime').addClass('d-none');
        $('.search-by-salary').addClass('d-none');
        $('.search-by-salarymain').addClass('d-none');
    } else if (value == "bysalarytest") {
        $('.search-by-salary').removeClass('d-none');
        $('.search-by-daywork').addClass('d-none');
        $('.search-by-id').addClass('d-none');
        $('.search-by-datetime').addClass('d-none');
        $('.search-by-salarymain').addClass('d-none');
    } else if (value == "bysalarymain") {
        $('.search-by-salarymain').removeClass('d-none');
        $('.search-by-salary').addClass('d-none');
        $('.search-by-daywork').addClass('d-none');
        $('.search-by-id').addClass('d-none');
        $('.search-by-datetime').addClass('d-none');
    } else {
        $('.box').children().addClass('d-none');
    }
}