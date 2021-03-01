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
    $("input[name^='thuoc_tinh']:checked", "#div_thuoc_tinh").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng.push(parseInt($(this).val()));
        } catch (ex) {
            console.log(ex);
        }
    });
    let thuoc_tinh_string = thuoc_tinh.join(',');
    let thuoc_tinh_rieng_string = thuoc_tinh_rieng.join(',');

    let id_ung_vien = '';
    let ngay_di_lam_from = 0;
    let ngay_di_lam_to = 0; let ngay_tao_from = 0; let ngay_tao_to = 0;

    if ($('#ngay_di_lam_from').val() !== "") {
        ngay_di_lam_from = Math.floor(toDate($('#ngay_di_lam_from').val()).getTime() / 1000.0);
    }
    if ($('#ngay_di_lam_to').val() !== "") {
        ngay_di_lam_to = Math.floor(toDate($('#ngay_di_lam_to').val()).getTime() / 1000.0);
    }
    if ($('#ngay_tao_from').val() !== "") {
        ngay_tao_from = Math.floor(toDate($('#ngay_tao_from').val()).getTime() / 1000.0);
    }

    if ($('#ngay_tao_to').val() !== "") {
        ngay_tao_to = Math.floor(toDate($('#ngay_tao_to').val()).getTime() / 1000.0);
    }
    let luong_mong_muon_from = $('#luong_mong_muon_from').val() !== "" ? parseInt($('#luong_mong_muon_from').val()) : 0;
    let luong_mong_muon_to = $('#luong_mong_muon_to').val() !== "" ? parseInt($('#luong_mong_muon_to').val()) : 0;

    let tim_ung_vien_team_khac = $("#tim_ung_vien_team_khac").is(":checked");
    let field_sort = $("#field_sort").val();
    let sort = $("#sort").val();

    let url_new = `term=${term}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${ngay_di_lam_to}&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to}&thuoc_tinh=${thuoc_tinh.join(',')}&thuoc_tinh_rieng=${thuoc_tinh_rieng.join(',')}&tim_ung_vien_team_khac=${tim_ung_vien_team_khac}&field_sort=${field_sort}&sort=${sort}`;

    window.history.pushState(window.location.href, "Danh sách ứng viên", `?${url_new}`);

    let tab_ung_vien = $('#table_data').DataTable({
        destroy: true, "ordering": false, "autoWidth": false,
        "dom": 'rt', /*https://datatables.net/reference/option/dom*/
        searchPanes: {
            controls: false,
            orderable: false
        },
        "language": {
            "emptyTable": "Không tìm thấy ứng viên"
        },
        ajax: function (data, callback) {
            let url =
                `${API_URL}/UngVien/search?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string
                }&term=${term}&id_ung_vien=${id_ung_vien}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${
                ngay_di_lam_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to
                }&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&tim_ung_vien_team_khac=${
                tim_ung_vien_team_khac}&field_sort=${field_sort}&sort=${sort}&page=${page}&page_size=${PAGE_SIZE}`;
            $.get(url,
                {
                },
                function (res) {
                    if (false) {
                        $("#table_data_processing").addClass("d-none");
                        $("#table_data_wrapper").addClass("d-none");
                        $("#tutorial").append(
                            "<h5><kbd>Thông báo:</kbd> Hình như bạn chưa có ứng viên nào?, ứng viên sẽ xuất hiện khi bạn nhấn <a class='btn-size btn btn-primary' style='color: #fff' href='/ungvien/Add'>Tạo mới</a> hoặc được mọi người chia sẻ. <br> Đây là dữ liệu mẫu sau khi tạo xong:<h5>");
                        let html_exam = "";
                        html_exam +=
                            `<div class="child-tutorial"><table id="table_data" class="table table-striped dataTable no-footer" role="grid">`;
                        html_exam += `<thead class="thead-light">`;
                        html_exam += `<tr role="row">`;
                        html_exam +=
                            `<th class="sorting_disabled details-control" rowspan="1" colspan="1" style="width: 15px;"></th>`;
                        html_exam +=
                            `<th class="sorting_disabled" rowspan="1" colspan="1" style="width: 150px;">Tên</th>`;
                        html_exam +=
                            `<th class="sorting_disabled" rowspan="1" colspan="1" style="width: 150px;">Liên hệ</th>`;
                        html_exam +=
                            `<th class="sorting_disabled" rowspan="1" colspan="1" style="width: 50px;">Địa chỉ</th>`;
                        html_exam +=
                            `<th class="sorting_disabled" rowspan="1" colspan="1" style="width: 200px;">V/trí ứ/tuyển</th>`;
                        html_exam +=
                            `<th class="sorting_disabled" rowspan="1" colspan="1" style="width: 150px;">Ghi chú	</th>`;
                        html_exam +=
                            `<th class="sorting_disabled" rowspan="1" colspan="1" style="width: 150px;">Lương mong muốn</th>`;

                        html_exam +=
                            `<th style="width: 156px;" class="sorting_disabled action" rowspan="1" colspan="1"></th></tr></thead>`;
                        html_exam += `<tbody>
                           <tr role="row" class="odd">
                              <td class=" details-control"></td>
                              <td><a href="#" >Nguyễn Gia Bách</a></td>
                              <td>0912312323<br>bach@gmail.com</td>
                              <td>Hà Đông, Hà Nội, Việt Nam</td>
                              <td>Nhân viên Kinh doanh</td>
                              <td></td>
                              <td>10.000.000</td>
                              <td class=" action">&nbsp;<button class="btn btn-success btn-big" title="Gán nhanh ứng viên vào job bằng Auto ID của JOB">Gán vào JOB</button><br><button class="btn btn-dark  btn-big modall btn-w2">Đánh dấu</button></td>
                           </tr>
                           <tr role="row" class="even">
                              <td class=" details-control"></td>
                              <td><a href="#" >Nguyễn Văn Thạch</a></td>
                              <td>919230123<br>thach@gmail.com</td>
                              <td>Hà Đông Hà Nội</td>
                              <td>Nhân viên PT phần mềm</td>
                              <td></td>
                              <td>10.000.000</td>
                              <td class=" action">&nbsp;<button class="btn btn-success btn-big" title="Gán nhanh ứng viên vào job bằng Auto ID của JOB">Gán vào JOB</button><br><button class="btn btn-dark  btn-big modall btn-w2">Đánh dấu</button></td>
                           </tr>
                        </tbody>`;
                        html_exam += `</table>`;
                        html_exam += `</div>`;
                        $("#tutorial").append(html_exam);
                    } else {
                        $("#tutorial").addClass("d-none");
                        $(".totalRecs").html("Tổng số ứng viên: " + res.total);
                        paging(res.total, 'search', page);
                        callback({
                            recordsTotal: res.total,
                            recordsFiltered: res.total,
                            data: res.data
                        });
                    }
                }).fail(function () {
                    $(".totalRecs").html("Tổng số ứng viên: 0");
                    callback({
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
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
        { "data": "so_dien_thoai", "name": "Liên hệ", "searchable": false, "width": "150px", "visible": false },
        { "data": "vi_tri_ung_tuyen", "name": "vi_tri_ung_tuyen", "searchable": false, "width": "150px" },

        { "data": "luong_mong_muon", "name": "luong_mong_muon", "searchable": false, "width": "65px" },
        { "data": "ngay_tao", "name": "ngay_tao", "searchable": false, "width": "150px" },
        {
            "data": null, "class": "action", "width": "156px"
        }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    let html_button = "";
                    /*html_button +=`<a class='btn btn-warning btn-big btn-w1' href='edit?id=${row.id_ung_vien}'>Sửa</a>&nbsp;<a class='btn btn-success btn-big btn-w1' href='detail/${row.id_ung_vien}'>Xem</a>`;
                    html_button += `&nbsp;<a class="btn btn-info btn-big" href="share?id=${row.id_ung_vien}">Chia sẻ</a><br>`;
                    */
                    html_button += `<button class="btn btn-success btn-big" title="Gán nhanh ứng viên vào job bằng Auto ID của JOB" data-toggle="modal" data-target="#auto_id_job" id="${row.id_ung_vien}" onclick="assignCandidateToJob(this.id)">Gán vào JOB (${row.so_luong_job_da_gan})</button><br>`;
                    html_button += `<button class="btn btn-dark  btn-big modall btn-w2" id="${row.id_ung_vien}" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>`;

                    return html_button;
                },
                "targets": 8
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = `<a href="#" data-link="${row.link_cv}" onclick="showCv(this)" data-toggle="modal" data-target="#preview-cv">${row.ho_ten_ung_vien}</a>`;
                    html_tmp += `<br>${row.so_dien_thoai}<br>${row.email}`;

                    return html_tmp;
                },
                "targets": 2
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = "";
                    if (row.thuoc_tinh != null) {
                        row.thuoc_tinh.forEach(tt => {
                            html_tmp += `<p>${tt.ten}</p>`;
                        });
                    }
                    return html_tmp;
                },
                "targets": 7
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
                "targets": 5
            }, {
                "render": function (data, type, row) {
                    let html_tmp = `${epochToTime(row.ngay_tao)}`;

                    return html_tmp;
                },
                "targets": 6
            }
        ],
        pageLength: PAGE_SIZE,
        responsive: true,
        serverSide: true,
        processing: true,
        "fnDrawCallback": function () {
            $(".auto_id").unbind('click').bind('click', function () {
                alert('x');
            });
        }
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
            row.child(getNoteUngVienById(row.data())).show();
            //row.child(NhaTuyenDungViewUngVien(row.data())).show();

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
    callAPI(`${API_URL}/ungvien/thongkethuoctinh?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string
        }&term=${term}&id_ung_vien=${id_ung_vien}&ngay_di_lam_from=${ngay_di_lam_from}&ngay_di_lam_to=${
        ngay_di_lam_to}&luong_mong_muon_from=${luong_mong_muon_from}&luong_mong_muon_to=${luong_mong_muon_to
        }&ngay_tao_from=${ngay_tao_from}&ngay_tao_to=${ngay_tao_to}&tim_ung_vien_team_khac=${
        tim_ung_vien_team_khac}&field_sort=${field_sort}&sort=${sort}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
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
            if (res.value != null) {
                for (var i = 0; i < res.value.length; i++) {
                    $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
                }
            }
            $('.add_thuoc_tinh').html(`<a target="_blank" href="/thuoctinh/add?loai=3"><i class="icon-add"></i>Thêm thuộc tính</a>`);
        }
    });

    // thuộc tính ghi chú
    callAPI(`${API_URL}/thuoctinh/canhan?loai=6`, null, "GET", function (res) {
        if (res.success) {
            console.log(res);
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
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri, type: a.type });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom === 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_ghi_chu_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_ghi_chu_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#thuoc_tinh_ghi_chu').html(thuoc_tinh);
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name^='thuoc_tinh_ghi_chu'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
            }
        }
    });
}
function onCreateThuocTinh(loai_obj) {
    //Lưu thông tin note ứng viên
    let id_ung_vien = $('#id_ung_vien_note').val();

    let ghi_chu = $('#ghi_chu').val();
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
    let thuoc_tinh_ghi_chu = [];
    $("input[name^='thuoc_tinh_ghi_chu']:checked").each(function (el) {
        try {
            thuoc_tinh_ghi_chu.push(parseInt($(this).val()));
        } catch (e) {
            return e;
        }
    });

    let obj = { "id_ung_vien": id_ung_vien, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "ghi_chu": ghi_chu, "thuoc_tinh_ghi_chu": thuoc_tinh_ghi_chu }

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

function assignCandidateToJob(id) {
    $("#id-candidate").val(id);
}

function saveAssignCandidateToJob(event) {
    event.preventDefault();
    let auto_id = $("#auto_id").val();
    let id_ung_vien = $("#id-candidate").val();
    let arr_id_ung_vien = [];
    arr_id_ung_vien.push(id_ung_vien);
    var obj = {
        "auto_id": auto_id, "id_ung_vien": arr_id_ung_vien
    };
    callAPI(`${API_URL}/noteungvienjob/getautoid?id_auto=${auto_id}&id_ung_vien=${arr_id_ung_vien}`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Gán ứng viên cho JOB thành công", "success");
            $('#auto_id_job').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function getNoteUngVienById(d) {
    var div = $('<div/>')
        .addClass('loading row_detail');
    let data_button = "";

    data_button += `<div style='text-align: right; width: 100%;'><a target="_blank" class="btn btn-warning btn-big btn-w1" href="edit?id=${d.id_ung_vien}">Sửa</a>`;
    data_button += `&nbsp;<a target="_blank" class="btn btn-success btn-big btn-w1" href="detail/${d.id_ung_vien}">Xem</a>`;
    data_button += `&nbsp;<a target="_blank" class="btn btn-info btn-big" href="share?id=${d.id_ung_vien}">Chia sẻ</a>`;
    data_button += `&nbsp;<button class="btn btn-danger btn-s-small" id="${d.id_ung_vien}" onclick="deleteRec(this.id)">Xóa</button></div><hr>`;
    div.append(data_button);
    callAPI(`${API_URL}/NoteUngVienJob/getnotebyid?id_ung_vien=${d.id_ung_vien}`, null, "GET", function (res) {
        let data_html = "";
        if (res.success && res.data.length > 0) {
            data_html = `<div id='div_ghi_chu_theo_job'>`;
            data_html += `<div class="row" style='border-bottom: 1px dotted #fbe4c5;'>`;
            data_html += `<div class="col-sm-4"><h5>JOB</h5></div>`;
            data_html += `<div class="col-sm-2"><h5>Trạng thái</h5></div>`;
            data_html += `<div class="col-sm-2"><h5>Ngày phỏng vấn</h5></div>`;
            data_html += `<div class="col-sm-2"><h5>Tóm tắt ứng viên</h5></div>`;
            data_html += `<div class="col-sm-2"><h5></h5></div>`;

            data_html += `</div>`;
            let count = 1;
            res.data.forEach(item => {
                data_html += `<div class="row" style='border-bottom:1px dotted #ccc;padding-top:5px;padding-bottom:5px'>`;
                data_html += `<div class="col-sm-4">- (<span title="ID của JOB" class="auto_id" onclick="copyText(this)"><code>${item.auto_id_job}</code></span>)<a target="_blank" href="/job/detail/${item.id_job}">${item.ten_job}</a> - <br><a target="_blank" href="/congty/detail/${item.id_cong_ty}">${item.cong_ty}</a></div>`;
                let html_trang_thai = "";
                if (item.thuoc_tinh != null) {
                    item.thuoc_tinh.forEach(tt => {
                        html_trang_thai += `<p>- ${tt.ten}</p>`;
                    });
                }
                data_html += `<div class="col-sm-2">${html_trang_thai}</div>`;
                data_html += `<div class="col-sm-2">${epochToTime(item.ngay_gio_phong_van)}</div>`;
                data_html += `<div class="col eye_zone">${item.ghi_chu !== null ? item.ghi_chu : ""}`;

                data_html += `<a type="button" target="_blank" href="/noteungvienjob/edit?id=${item.id_note_ung_vien_job}" class="btn-warning float-right" title="Sửa thông tin ứng viên theo JOB này"><svg class="bi bi-pencil" width="1em" height="1em" viewBox="0 0 16 16" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
  <path fill-rule="evenodd" d="M11.293 1.293a1 1 0 011.414 0l2 2a1 1 0 010 1.414l-9 9a1 1 0 01-.39.242l-3 1a1 1 0 01-1.266-1.265l1-3a1 1 0 01.242-.391l9-9zM12 2l2 2-9 9-3 1 1-3 9-9z" clip-rule="evenodd"></path>
  <path fill-rule="evenodd" d="M12.146 6.354l-2.5-2.5.708-.708 2.5 2.5-.707.708zM3 10v.5a.5.5 0 00.5.5H4v.5a.5.5 0 00.5.5H5v.5a.5.5 0 00.5.5H6v-1.5a.5.5 0 00-.5-.5H5v-.5a.5.5 0 00-.5-.5H3z" clip-rule="evenodd"></path>
</svg></a></div>`;
                data_html += `<p id='content_eye_${d.id_ung_vien}_${count}' title=''><svg style='margin-top: -5px;' class="eye_info" enable-background="new 0 0 512 512" height="30" viewBox="0 0 512 512" width="18" xmlns="http://www.w3.org/2000/svg"><g><g><g><circle cx="256" cy="256" fill="#ff9e22" r="256"/></g></g><path d="m87.279 277.633 227.562 227.562c94.547-22.24 168.881-96.886 190.654-191.612l-57.583-57.583z" fill="#e17726"/><path d="m447.912 256s-85.239 105.085-190.388 105.085-190.387-105.085-190.387-105.085 85.239-105.085 190.388-105.085 190.387 105.085 190.387 105.085z" fill="#eef4ff"/><path d="m447.912 256s-85.239 105.085-190.388 105.085v-210.17c105.149 0 190.388 105.085 190.388 105.085z" fill="#d6e9f8"/><g><g><circle cx="257.525" cy="255.992" fill="#804111" r="98.769"/><path d="m356.294 255.992c0 54.549-44.221 98.769-98.769 98.769v-197.538c54.549 0 98.769 44.221 98.769 98.769z" fill="#62320d"/><path d="m319.63 255.992c0 34.299-27.805 62.105-62.105 62.105-27.591 0-50.98-17.992-59.073-42.884l-.165-37.926c7.936-25.159 31.457-43.4 59.238-43.4 34.299.001 62.105 27.806 62.105 62.105z" fill="#464646"/><path d="m198.292 237.287c-1.863 5.904-2.872 12.186-2.872 18.705 0 6.71 1.077 13.165 3.046 19.219l37.837-18.918z" fill="#eef4ff"/><path d="m319.63 255.992c0-34.299-27.805-62.105-62.105-62.105v124.21c34.299 0 62.105-27.805 62.105-62.105z" fill="#2d2d2d"/></g></g></g></svg></p>`;
                data_html += `</div>`;
                ++count;
            });

            data_html += `</div>`;
        }

        div.append(data_html)
            .removeClass('loading');
        NhaTuyenDungViewUngVien(d);
    });

    callAPI(`${API_URL}/note/getnotebyobject?id_obj=${d.id_ung_vien}`, null, "GET", function (res) {
        if (res.success && res.data.length > 0) {
            let data_html = "<div id='div_ghi_chu_ung_vien' class='row'>";
            res.data.forEach(item => {
                if (item.ghi_chu != "") {
                    data_html += `<div class='col-sm-6'>${item.noi_dung}</div>`;
                    data_html += `<div class='col-sm-2'>${epochToTime(item.ngay_tao)}</div>`;
                    data_html += `<div class='col-sm-2'>${item.nguoi_tao}</div>`;
                }
            });
            data_html += `</div><hr>`;
            div.append(data_html).removeClass('loading');
        }
    });
    return div;
}

function NhaTuyenDungViewUngVien(d) {
    callAPI(`${API_URL}/ungvien/ungvienwasseen?id_ung_vien=${d.id_ung_vien}`, null, "GET", function (res) {
        if (res.success) {
            let count = 1;
            console.log(res);
            res.data.forEach(function (item) {
                //$('.view_log').append(`<p title='${item.ghi_chu}'><svg style='margin-top: -5px;' class="eye_info" enable-background="new 0 0 512 512" height="30" viewBox="0 0 512 512" width="18" xmlns="http://www.w3.org/2000/svg"><g><g><g><circle cx="256" cy="256" fill="#ff9e22" r="256"/></g></g><path d="m87.279 277.633 227.562 227.562c94.547-22.24 168.881-96.886 190.654-191.612l-57.583-57.583z" fill="#e17726"/><path d="m447.912 256s-85.239 105.085-190.388 105.085-190.387-105.085-190.387-105.085 85.239-105.085 190.388-105.085 190.387 105.085 190.387 105.085z" fill="#eef4ff"/><path d="m447.912 256s-85.239 105.085-190.388 105.085v-210.17c105.149 0 190.388 105.085 190.388 105.085z" fill="#d6e9f8"/><g><g><circle cx="257.525" cy="255.992" fill="#804111" r="98.769"/><path d="m356.294 255.992c0 54.549-44.221 98.769-98.769 98.769v-197.538c54.549 0 98.769 44.221 98.769 98.769z" fill="#62320d"/><path d="m319.63 255.992c0 34.299-27.805 62.105-62.105 62.105-27.591 0-50.98-17.992-59.073-42.884l-.165-37.926c7.936-25.159 31.457-43.4 59.238-43.4 34.299.001 62.105 27.806 62.105 62.105z" fill="#464646"/><path d="m198.292 237.287c-1.863 5.904-2.872 12.186-2.872 18.705 0 6.71 1.077 13.165 3.046 19.219l37.837-18.918z" fill="#eef4ff"/><path d="m319.63 255.992c0-34.299-27.805-62.105-62.105-62.105v124.21c34.299 0 62.105-27.805 62.105-62.105z" fill="#2d2d2d"/></g></g></g></svg></p>`)
                $(`#content_eye_${d.id_ung_vien}_${count}`).prop('title', `${item.ghi_chu}`);
                ++count;
            })
        }
    })
}

$("#ngay_di_lam_from").blur(function () {
    checkDayInput($("#ngay_di_lam_from"));
});
$("#ngay_di_lam_to").blur(function () {
    checkDayInput($("#ngay_di_lam_to"));
});
$("#ngay_tao_from").blur(function () {
    checkDayInput($("#ngay_tao_from"));
});
$("#ngay_tao_to").blur(function () {
    checkDayInput($("#ngay_tao_to"));
});