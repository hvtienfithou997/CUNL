function search(page) {
    let term = $(`[name='term']`).val();
    if (typeof page === 'undefined') {
        page = 1;
    }
    let thuoc_tinh = []; let thuoc_tinh_rieng = [];
    $("input[name^='thuoc_tinh']:checked", "#div_thuoc_tinh").each(function (el) {
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
        ngay_gio_phong_van_from = Math.floor(toDate($('#ngay_gio_phong_van_from').val()).getTime() / 1000.0);
    }
    if ($('#ngay_gio_phong_van_to').val() != "") {
        ngay_gio_phong_van_to = Math.floor(toDate($('#ngay_gio_phong_van_to').val()).getTime() / 1000.0);
    }
    if ($('#ngay_di_lam_from').val() != "") {
        ngay_di_lam_from = Math.floor(toDate($('#ngay_di_lam_from').val()).getTime() / 1000.0);
    }
    if ($('#ngay_di_lam_to').val() != "") {
        ngay_di_lam_to = Math.floor(toDate($('#ngay_di_lam_to').val()).getTime() / 1000.0);
    }

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
                        let html_thuoc_tinh = "";
                        if (item.thuoc_tinh != null) {
                            item.thuoc_tinh.forEach(tt => {
                                html_thuoc_tinh += `<li>- ${tt.ten}</li>`;
                            });
                        }
                        html_data += `<tr>`;
                        html_data += `<td class="d-none"><input type="type" id="id_note_uv_job" value="${item.id_note_ung_vien_job}"/></td>`;
                        html_data += `<td><a href="/ungvien/detail/${item.id_ung_vien}" target="_blank">${item.ho_ten_ung_vien}<br>${item.so_dien_thoai = item.so_dien_thoai != "" ? item.so_dien_thoai : ""}<br>${item.email = item.email != "" ? item.email : ""}</a><hr>${html_thuoc_tinh}</td>`;
                        html_data += `<td style='vertical-align:bottom'>`;
                        html_data += `<button class="btn btn-secondary" style='white-space:nowrap' data-toggle="modal" data-tooltip="tooltip" title="Thay đổi trạng thái ứng viên!" data-target="#myStatus" id="${item.id_note_ung_vien_job}" onclick="onShowTrangThai(this.id)">Thay đổi</button></td >`;
                        html_data += `<td style='vertical-align:bottom'>${item.ngay_gio_phong_van > 0 ? "<p style='white-space:nowrap'>" + epochToTimeWithHour(item.ngay_gio_phong_van) + "</p>" : ""}`;
                        html_data += `<button class="btn btn-primary btn-w2" data-toggle="modal" data-tooltip="tooltip" title="Thêm giờ phỏng vấn!" data-target="#addDate" id="${item.id_note_ung_vien_job}" onclick="onShowGioPhongVan(this.id)">Tạo giờ</button></td>`;
                        html_data += `<td width:150px>${item.ngay_di_lam > 0 ? epochToTime(item.ngay_di_lam) : ""}</td>`;
                        html_data += `<td>${item.luong_thu_viec ? formatCurency(item.luong_thu_viec) : ""}/${item.luong_thu_viec > 0 ? formatCurency(item.luong_chinh_thuc) : ""}</td>`;
                        html_data += `<td>${item.ghi_chu !== null ? item.ghi_chu : ""} <hr> ${item.xem_cv == true ? `Nhà tuyển dụng đã xem CV <b>${item.ho_ten_ung_vien}</b>` : ""}</td>`;
                        html_data += `<td><a class="btn btn-warning btn-big btn-w1" href="edit?id=${item.id_note_ung_vien_job}">Sửa</a>`;
                        html_data += `&nbsp;<a class="btn btn-success btn-big btn-w1" href="detail/${item.id_note_ung_vien_job}">Xem</a>`;
                        html_data += `&nbsp;<a class="btn btn-info btn-big" href="share?id=${item.id_note_ung_vien_job}">Chia sẻ</a>`;
                        html_data += `<button class="btn btn-dark modall btn-big btn-w2" id="${item.id_note_ung_vien_job
                            }" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id,'${item.id_ung_vien}')">Đánh dấu</button>`;
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
    callAPI(`${API_URL}/noteungvienjob/thongkethuoctinh?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string
        }&term=${term}&id_user=${id_user}&id_job=${id_job}&id_ung_vien=${id_ung_vien}&ngay_gio_phong_van_from=${
        ngay_gio_phong_van_from}&ngay_gio_phong_van_to=${ngay_gio_phong_van_to}&ngay_di_lam_from=${ngay_di_lam_from
        }&ngay_di_lam_to=${ngay_di_lam_to}&luong_thu_viec_from=${luong_thu_viec_from}&luong_thu_viec_to=${
        luong_thu_viec_to}&luong_chinh_thuc_from=${luong_chinh_thuc_from}&luong_chinh_thuc_to=${luong_chinh_thuc_to
        }&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
            if (res.success) {
                $(".thong_ke").remove();
                res.data.forEach(item => {
                    let ele = $(`input[name^='thuoc_tinh'][value='${item.k}']`);
                    if (ele) {
                        ele.next().append(` <code class='thong_ke'>(${item.v})</code>`);
                    }
                });
            }
        });
    //
    // Lấy chi tiết job được chọn
}
function detailJob(id_job) {
    if (id_job.length > 0) {
        callAPI(`${API_URL}/Job/view?id=${id_job}`, null, "GET", function (res) {
            if (res.success) {
                if (res.data != null && res.data != undefined && res.data != "") {
                    let html_job = "";
                    let html_cong_ty = "";
                    html_job += `<h6>ID JOB: <strong>${res.data.auto_id}</strong></h6>`;
                    html_job += `<h6>Vị trí: <strong>${res.data.chuc_danh}</strong></h6>`;
                    html_job += `<h6>Người liên hệ: <strong>${res.data.nguoi_lien_he}</strong></h6>`;
                    html_job += `<h6>Lương chính thức: <strong>${formatCurency(res.data.luong_chinh_thuc)} VND</strong></h6>`;
                    res.data.tinh_thanh.forEach((item) => {
                        html_job += `<h6>Địa chỉ Job: <strong>${item.ten_tinh}</strong></h6>`;
                    })
                    html_job += `<h6>Số lượng: <strong>${res.data.so_luong} (người)</strong></h6>`;

                    html_cong_ty += `<h6>Tên công ty: <strong>${res.data.cong_ty.ten_cong_ty}</strong></h6>`;
                    html_cong_ty += `<h6>Địa chỉ: <strong>${res.data.cong_ty.dia_chi != null ? res.data.cong_ty.dia_chi : ""}</strong></h6>`;
                    html_cong_ty += `<h6>Điện thoại: <strong>${res.data.cong_ty.dien_thoai != null ? res.data.cong_ty.dien_thoai : ""}</strong></h6>`;
                    html_cong_ty += `<h6>Người gửi HĐ: <strong>${res.data.cong_ty.info_gui_hop_dong != null ? res.data.cong_ty.info_gui_hop_dong : ""}</strong></h6>`;
                    html_cong_ty += `<h6> &nbsp</h6>`;
                    html_cong_ty += `<a type="button" class="btn btn-size btn-primary" href="/job/detail/${res.data.id_job}" title="Xem chi tiết" target="_blank">Chi tiết JOB</a>`;
                    $('.info-job-left').html(html_job);
                    $('.info-job-right').html(html_cong_ty);
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
}
function onShowThuocTinh(id, id_uv) {
    
    callAPI(`${API_URL}/thuoctinh/canhan?loai=5&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let thuoc_tinh = '';
            thuoc_tinh = `<input class="d-none" id="id_obj" value="${id}">`;
            thuoc_tinh += `<input class="d-none" id="id_uv" value="${id_uv}">`;
            var result = [];
            res.data.forEach(function (hash) {
                return function (a) {
                    if (!hash[a.nhom]) {
                        hash[a.nhom] = { nhom: a.nhom, activities: [] };
                        result.push(hash[a.nhom]);
                    }
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri, type: a.type });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom === 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row box-thuoc-tinh'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row box-thuoc-tinh'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_danh_dau_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#thuoc_tinh_du_lieu').html(thuoc_tinh);
            $(".box-thuoc-tinh").append("<li style='width:100%; text-align:right'><span type='button' class='btn-danger remove-elem'> Bỏ chọn</span></li>");
            $(".remove-elem").click(function () {
                $(this).parent().siblings('li').find('input').prop('checked', false);
            });
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
            }
            $('#thuoc_tinh_du_lieu').append(`<a target='_blank' href="/thuoctinh/add?loai=5"><i class="icon-add"></i>Thêm thuộc tính theo ứng viên job</a>`);
        }
    });
    showThuocTinhUngVien(id_uv);
}

function showThuocTinhUngVien(id) {
    callAPI(`${API_URL}/thuoctinh/canhan?loai=3&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let thuoc_tinh = '';
            var result = [];
            res.data.forEach(function (hash) {
                return function (a) {
                    if (!hash[a.nhom]) {
                        hash[a.nhom] = { nhom: a.nhom, activities: [] };
                        result.push(hash[a.nhom]);
                    }
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri, type: a.type });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom === 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row box-thuoc-tinh1'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_ung_vien_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row box-thuoc-tinh1'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_ung_vien_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#thuoc_tinh_ung_vien').html(thuoc_tinh);
            $(".box-thuoc-tinh1").append("<li style='width:100%; text-align:right'><span type='button' class='btn-danger remove-elem'> Bỏ chọn</span></li>");
            $(".remove-elem").click(function () {
                $(this).parent().siblings('li').find('input').prop('checked', false);
            });
            if (res.value != null) {
                for (var i = 0; i < res.value.length; i++) {
                    $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
                }
            }
            $('#thuoc_tinh_ung_vien').append(`<a target="_blank" href="/thuoctinh/add?loai=3"><i class="icon-add"></i>Thêm thuộc tính cho ứng viên</a>`);
        }
    });
}

function onCreateThuocTinh(loai_obj) {
    let id_note_uv_job = $('#id_obj').val();
    let id_uv = $('#id_uv').val();
    let thuoc_tinh = [], thuoc_tinh_rieng = [];
    $("input[name^='thuoc_tinh_danh_dau']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0') {
                thuoc_tinh.push(parseInt($(this).val()));
            } else {
                thuoc_tinh_rieng.push(parseInt($(this).val()));
            }
        } catch (e) {
            return e;
        }
    });

    let thuoc_tinh_uv_chung = [], thuoc_tinh_uv_rieng = [];
    $("input[name^='thuoc_tinh_ung_vien']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0') {
                thuoc_tinh_uv_chung.push(parseInt($(this).val()));
            } else {
                thuoc_tinh_uv_rieng.push(parseInt($(this).val()));
            }
        } catch (e) {
            return e;
        }
    });
    //let obj = { "loai_obj": loai_obj, "id_obj": id, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "nguoi_tao": user, "nguoi_sua": user }
    let obj = { "loai_obj": loai_obj, "id_ung_vien": id_uv, "id_obj": id_note_uv_job, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "thuoc_tinh_uv_chung": thuoc_tinh_uv_chung, "thuoc_tinh_uv_rieng": thuoc_tinh_uv_rieng }
    callAPI(`${API_URL}/noteungvienjob/savethuoctinh`, obj, "POST", function (res) {
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
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri, type: a.type });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom == 0) {
                    trang_thai += `Nhóm ${item.nhom}`;
                    trang_thai += "<ul style='width:100%' class='check-box row box-thuoc-tinh'>";
                    item.activities.forEach((child) => {
                        trang_thai += `<li class="col-md-12"><input type='checkbox' name='update_trang_thai_${item.nhom}' data-type='${child.type}' value='${child.gia_tri}'> ${child.ten} </li>`;
                    });
                    trang_thai += "</ul>";
                } else {
                    trang_thai += `Nhóm ${item.nhom}`;

                    trang_thai += `<ul style='width:100%' class='check-box row box-thuoc-tinh'>`;
                    item.activities.forEach((child) => {
                        trang_thai += `<li class='col-md-12'><input type='radio' name='update_trang_thai_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    trang_thai += "</ul>";
                }
            });
            $('#status').html(trang_thai);
            $(".box-thuoc-tinh").append("<li style='width:100%; text-align:right'><span type='button' class='btn-danger remove-elem'> Bỏ chọn</span></li>");
            $(".remove-elem").click(function () {
                $(this).parent().siblings('li').find('input').prop('checked', false);
            });
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name^='update_trang_thai'][value='${res.value[i]}']`).prop('checked', true);
            }

            $('#thuoc_tinh_du_lieu').append(`<a href="/thuoctinh/add?loai=5"><i class="icon-add"></i>Thêm thuộc tính</a>`);
        }
    });
}

function onSaveTrangThai() {
    let id_note_ung_vien_job = $('#id_note_uv_job_modal').val();
    let thuoc_tinh = [];
    $("input[name^='update_trang_thai']:checked").each(function (el) {
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
            $.notify("Thay đổi trạng thái thành công", "success");
            $('#myStatus').modal('hide');
            setTimeout(function () {
                search(1);
            }, 1500);
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
            setTimeout(function () {
                search(1);
            }, 1500);
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

$("#ngay_gio_phong_van_from").blur(function () {
    checkDayInput($("#ngay_gio_phong_van_from"));
});
$("#ngay_gio_phong_van_to").blur(function () {
    checkDayInput($("#ngay_gio_phong_van_to"));
});
$("#ngay_di_lam_from").blur(function () {
    checkDayInput($("#ngay_di_lam_from"));
});
$("#ngay_di_lam_to").blur(function () {
    checkDayInput($("#ngay_di_lam_to"));
});