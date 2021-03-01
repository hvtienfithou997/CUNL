$(function () {
})
function getNoteUngVien(d) {
    var div = $('<div/>')
        .addClass('loading')
        .text('Loading...');
    callAPI(`${API_URL}/NoteUngVien/search?term=&id_ung_vien=${d.id_ung_vien}&thuoc_tinh=&thuoc_tinh_rieng=&page=1`, null, "GET", function (res) {
        if (res.success) {
            let data_html = "";
            if (res.data.length > 0) {
                res.data.forEach(item => {
                    data_html += `<div class="row">`;
                    data_html += `<div class="col-sm-7 note_uv">${item.ghi_chu}</div>`;
                    data_html += `<div class="col-sm-2">${epochToTime(item.ngay_tao)}</div>`;
                    data_html += `<div class="col-sm-2">${item.nguoi_tao}</div>`;
                    data_html += `</div>`;
                });
            } else {
                data_html = "Không có ghi chú nào cho ứng viên này";
            }
            div.html(data_html)
                .removeClass('loading');
        }
    });
    return div;
}
function search(page) {
    let term = encodeURI($(`[name='term']`).val());

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

    let id_ung_vien = '';
    let ngay_di_lam_from = 0;
    let ngay_di_lam_to = 0; let ngay_tao_from = 0; let ngay_tao_to = 0;

    if ($('#ngay_di_lam_from').val() != "") {
        ngay_di_lam_from = Math.floor(new Date($('#ngay_di_lam_from').val()) / 1000.0);
    }
    if ($('#ngay_di_lam_to').val() != "") {
        ngay_di_lam_to = Math.floor(new Date($('#ngay_di_lam_to').val()) / 1000.0);
    }
    if ($('#ngay_tao_from').val() != "") {
        ngay_tao_from = Math.floor(new Date($('#ngay_tao_from').val()) / 1000.0);
    }

    if ($('#ngay_tao_to').val() != "") {
        ngay_tao_to = Math.floor(new Date($('#ngay_tao_to').val()) / 1000.0);
    }
    let luong_mong_muon_from = $('#luong_mong_muon_from').val() != "" ? parseInt($('#luong_mong_muon_from').val()) : 0;
    let luong_mong_muon_to = $('#luong_mong_muon_to').val() != "" ? parseInt($('#luong_mong_muon_to').val()) : 0;

    let tim_ung_vien_team_khac = $("#tim_ung_vien_team_khac").is(":checked");
    let url_new = `term=${term}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${ngay_di_lam_to}&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to}&thuoc_tinh=${thuoc_tinh.join(',')}&thuoc_tinh_rieng=${thuoc_tinh_rieng.join(',')}&tim_ung_vien_team_khac=${tim_ung_vien_team_khac}`;

    window.history.pushState(window.location.href, "Danh sách ứng viên", `?${url_new}`);

    let tab_ung_vien = $('#table_data').DataTable({
        destroy: true, "ordering": false, "autoWidth": false,
        "dom": 'rt',/*https://datatables.net/reference/option/dom*/
        searchPanes: {
            controls: false, orderable: false
        },
        ajax: function (data, callback) {
            let url = `${API_URL}/UngVien/search?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string}&term=${term}&id_ung_vien=${id_ung_vien}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${ngay_di_lam_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to}&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&tim_ung_vien_team_khac=${tim_ung_vien_team_khac}&page=${page}&page_size=${PAGE_SIZE}`;
            $.get(url, {
            }, function (res) {
                $(".totalRecs").html("Tổng số ứng viên: " + res.total);
                paging(res.total, 'search', page);
                callback({
                    recordsTotal: res.total,
                    recordsFiltered: res.total,
                    data: res.data,
                });
            });
        },
        columns: [{
            "class": "details-control",
            "orderable": false,
            "data": null,
            "defaultContent": "",
            "width": "15px"
        },
        { "data": "id_ung_vien", "name": "id_ung_vien", "searchable": false, "visible": false },
        { "data": "ho_ten_ung_vien", "name": "Tên ứng viên", "searchable": true, "width": "150px" },
        { "data": "so_dien_thoai", "name": "Liên hệ", "searchable": false, "width": "150px" },
        { "data": "dia_chi", "name": "dia_chi", "searchable": false, "width": "200px" },
        { "data": "vi_tri_ung_tuyen", "name": "vi_tri_ung_tuyen", "searchable": false, "width": "150px" },
        { "data": "ghi_chu_cuoi", "name": "ghi_chu_cuoi", "searchable": false, "width": "150px" },
        { "data": "luong_mong_muon", "name": "luong_mong_muon", "searchable": false, "width": "65px" },
        {
            "data": null, "class": "action", "width": "156px"
        }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    let html_button = `<a class='btn btn-warning btn-big btn-w1' href='edit?id=${row.id_ung_vien}'>Sửa</a>&nbsp;<a class='btn btn-success btn-big btn-w1' href='detail/${row.id_ung_vien}'>Xem</a>`;
                    html_button += `&nbsp;<a class="btn btn-info btn-big" href="share?id=${row.id_ung_vien}">Chia sẻ</a><br>`;
                    html_button += `<button class="btn btn-dark  btn-big modall btn-w2" id="${row.id_ung_vien}" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>`;
                    html_button += `&nbsp;<button class="btn btn-danger btn-s-small" id="${row.id_ung_vien}" onclick="deleteRec(this.id)">Xóa</button>`;


                    return html_button;
                },
                "targets": 8
            }, {
                "render": function (data, type, row) {
                    let html_tmp = `<a href="#" data-link="${row.link_cv}" onclick="showCv(this)" data-toggle="modal" data-target="#preview-cv">${row.ho_ten_ung_vien}</a>`;
                    let url_temp = "";

                    return html_tmp;
                },
                "targets": 2
            }
            , {
                "render": function (data, type, row) {
                    let html_tmp = `${row.so_dien_thoai}<br>${row.email}`;

                    return html_tmp;
                },
                "targets": 3
            }, {
                "render": function (data, type, row) {
                    let html_tmp = `${formatCurency(row.luong_mong_muon)}`;

                    return html_tmp;
                },
                "targets": 7
            }
        ],
        pageLength: PAGE_SIZE,
        responsive: true,
        serverSide: true,
        processing: true
    });

    var detailRows = [];

    $('#table_data tbody').off('click').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tab_ung_vien.row(tr);
        var idx = $.inArray(tr.attr('id'), detailRows);

        if (row.child.isShown()) {
            tr.removeClass('details');
            row.child.hide();
            detailRows.splice(idx, 1);
        }
        else {
            tr.addClass('details');
            row.child(getNoteUngVien(row.data())).show();
            if (idx === -1) {
                detailRows.push(tr.attr('id'));
            }
        }
    });
    tab_ung_vien.off('draw').on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });
};
function showCv(e) {
    var link = $(e).attr('data-link');
    if (link != "") {
        var url_temp = "";
        var ext = link.split('.').pop();
        url_temp = ext == "pdf" ? link : "https://view.officeapps.live.com/op/view.aspx?src=" + link;
        $("#cv_ung_vien").empty();
        $("#cv_ung_vien").append(`<iframe id="iframe-cv" src="${url_temp}"></iframe>`);
    } else {
        $("#cv_ung_vien").empty();
        $("#cv_ung_vien").append(`<p>CV của ứng viên này chưa được Upload!</p>`);
    }
}
function showNote(id) {
    callAPI(`${API_URL}/NoteUngVien/search?term=&id_ung_vien=${id}&thuoc_tinh=&thuoc_tinh_rieng=&page=1`, null, "GET", function (res) {
        if (res.success) {
            let data_html = "";
            res.data.forEach(item => {
                data_html += "<tr>";
                data_html += `<td></td>`;
                data_html += `<td>${item.ghi_chu}</td>`;
                data_html += `<td>${item.ngay_tao}</td>`;
                data_html += `<td>${item.nguoi_tao}</td></tr>`;
            });
            $('.table-data').html(data_html);
        }
    });
}

function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/ungvien/delete?id=${id}`,
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

function onShowThuocTinh(id) {
    // show ghi chú ứng viên
    $('#id_ung_vien_note').val(id);

    callAPI(`${API_URL}/thuoctinh/canhan?loai=3&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            console.log(res.data)
            let thuoc_tinh = '';
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
            if (res.value != null) {
                for (var i = 0; i < res.value.length; i++) {
                    $(`input[name='thuoc_tinh_danh_dau'][value='${res.value[i]}']`).prop('checked', true);
                }
            }
            $('.add_thuoc_tinh').html(`<a href="/thuoctinh/add"><i class="icon-add"></i>Thêm thuộc tính</a>`);
        }
    });

    callAPI(`${API_URL}/thuoctinh/canhan?loai=2`, null, "GET", function (res) {
        if (res.success) {
            console.log(res.data);
            let thuoc_tinh = '';
            thuoc_tinh = `<input class="d-none" id="id_obj" value="${id}">`;
            thuoc_tinh += `<p>Thuộc tính ghi chú: </p>`;
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
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_ghi_chu' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_ghi_chu' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#thuoc_tinh_ghi_chu').html(thuoc_tinh);
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name='thuoc_tinh_ghi_chu'][value='${res.value[i]}']`).prop('checked', true);
            }
        }
    });
}
function onCreateThuocTinh(loai_obj) {
    //Lưu thông tin note ứng viên
    let id_ung_vien = $('#id_ung_vien_note').val();
    console.log(id_ung_vien);
    let ghi_chu = $('#ghi_chu').val();
    let thuoc_tinh = [];
    $("input[name='thuoc_tinh_danh_dau']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch (e) {
            return e;
        }
    });
    let thuoc_tinh_ghi_chu = [];
    $("input[name='thuoc_tinh_ghi_chu']:checked").each(function (el) {
        try {
            thuoc_tinh_ghi_chu.push(parseInt($(this).val()));
        } catch (e) {
            return e;
        }
    });
    let obj = { "id_ung_vien": id_ung_vien, "thuoc_tinh": thuoc_tinh, "ghi_chu": ghi_chu, "thuoc_tinh_ghi_chu": thuoc_tinh_ghi_chu }

    callAPI(`${API_URL}/ungvien/savenote`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thành công", "success");
            $('#myModal').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

$("#btn_search").click(function () {
    search(1);
});

function searchBy() {
    let value = $('#type-search').val();
    if (value == "bydaywork") {
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