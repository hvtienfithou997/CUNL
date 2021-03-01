function onSubmit() {
    let id_job = $('#id_job').val();
    let ngay_nhan_job = $('#ngay_nhan_job').val();
    let ngay_nhan_job_convert = Math.floor(new Date(ngay_nhan_job) / 1000.0);
    let thuoc_tinh = [];
    $("input[name^='thuoc_tinh']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch{
        }
    });
    //let trang_thai = $('input[name=trang_thai]:checked').val();
    var obj = {
        "id_job": id_job, "ngay_nhan_job": ngay_nhan_job_convert, "thuoc_tinh": thuoc_tinh,
        "nguoi_tao": user, "nguoi_sua": user
    };
    callAPI(`${API_URL}/UserJob`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function bindDetail(id) {
    callAPI(`${API_URL}/UserJob/view?id=${id}`, null, "GET", function (res) {
        //console.log(res.data);
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#userjob_detail").empty();
                $("#userjob_detail").append(`<li>JOB: <span class="font-weight-bold">${res.data.chuc_danh}</span></li>`);
                $("#userjob_detail").append(`<li>Ngày nhận: <span class="font-weight-bold">${epochToTime(res.data.ngay_nhan_job)}</span></li>`);
                console.log(res.data.thuoc_tinh);
                let html_thuoc_tinh = "<ul>";
                if (res.data.thuoc_tinh != null) {
                    res.data.thuoc_tinh.forEach(tt => {
                        html_thuoc_tinh += `<li><span class="font-weight-bold">${tt.ten}</span></li>`;

                    });
                }
                html_thuoc_tinh += `</ul>`;

                $("#userjob_detail").children().addClass('list-group-item');

                callAPI(`${API_URL}/ungvien/allungvienbyuser?id_user_job=${id}`, null, "GET", function (res1) {
                    console.log(res1 + "+" + id);
                    if (res1.success) {
                        $('#list_ung_vien_by_user_job').empty();
                        if (res1.data != null && res1.data != undefined && res1.data != "") {
                            $("#list_ung_vien_by_user_job").append("<li class='list-group-item'>Danh sách ứng viên:</li>");
                            $.each(res1.data, function (key, value) {
                                $("#list_ung_vien_by_user_job").append(`<li class="list-group-item"> -- <span class="font-weight-bold">${value.ho_ten_ung_vien}</span></li>`);
                            });

                        }
                    } else {
                        $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                    }
                });
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function search(page) {
    let term = encodeURI($(`[name='term']`).val());

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

    let url_new = `term=${term}&thuoc_tinh=${thuoc_tinh.join(',')}&thuoc_tinh_rieng=${thuoc_tinh_rieng.join(',')}&page=${page}`;
    window.history.pushState(window.location.href, "Danh sách user job", `?${url_new}`);
    let tab_data = $('#table_data').DataTable({
        destroy: true, "ordering": false, "autoWidth": false,
        "dom": 'rt',/*https://datatables.net/reference/option/dom*/
        searchPanes: {
            controls: false, orderable: false
        },
        ajax: function (data, callback) {
            let url = `${API_URL}/UserJob/search?term=${term}&id_ung_vien=&ngay_nhan_job_from=0&ngay_nhan_job_to=0&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&page=${page}&page_size=${PAGE_SIZE}`;
            $.get(url, {
            }, function (res) {
                $(".totalRecs").html("Tổng số JOB: " + res.total);
                paging(res.total, 'search', page);
                callback({
                    recordsTotal: res.total,
                    recordsFiltered: res.total,
                    data: res.data,
                });
            });
        },
        columns: [
            {
                "class": "details-control",
                "orderable": false,
                "data": null,
                "defaultContent": "",
                "width": "15px"
            },
            { "data": "id_user_job", "name": "id_user_job", "searchable": false, "visible": false },
            { "data": "chuc_danh", "name": "chuc_danh", "searchable": true,"width":"40%" },
            { "data": "cong_ty", "name": "cong_ty", "searchable": false,"width":"20%" },
            { "data": "ngay_nhan_job", "name": "ngay_nhan_job", "searchable": false, "width": "50px" },
            { "data": "id_job", "name": "id_job", "searchable": false, "visible": false },
            { "data": "id_user_job", "name": "id_user_job", "searchable": false, "visible": false },
            { "data": "so_luong", "name": "so_luong", "searchable": false, "width": "15px" },
            {
                "data": null,"width":"156px"
            }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    let html_button = `<a class="btn btn-info btn-big" href="/NoteUngVienJob/ungvientheojob?id_job=${row.id_job}">Ứng viên đã gán</a>&nbsp;<a class='btn btn-success btn-big' href='detail/${row.id_user_job}'>Xem</a>`;
                    html_button += `<br>`;
                    html_button += `<a class="btn btn-primary btn-big" href="AllUngVien?id_user_job=${row.id_user_job}&id_job=${row.id_job}">Gán ứng viên</a>&nbsp;<a class='btn btn-warning btn-big' href='edit?id=${row.id_user_job}'>Sửa</a>`;

                    return html_button;
                },
                "targets": 8
            }, {
                "render": function (data, type, row) {
                    let html_tmp = `<a target="_blank" href="/job/detail/${row.id_job}">${row.chuc_danh}</a>`;
                    let url_temp = "";

                    return html_tmp;
                },
                "targets": 2
            }
            , {
                "render": function (data, type, row) {
                    let html_tmp = `${row.cong_ty}`;

                    return html_tmp;
                },
                "targets": 3
            }, {
                "render": function (data, type, row) {
                    let html_tmp = `${epochToTime(row.ngay_nhan_job)}`;

                    return html_tmp;
                },
                "targets": 4
            }
        ],
        pageLength: PAGE_SIZE,
        responsive: true,
        serverSide: true,
        processing: true
    });

    let detailRows = [];

    $('#table_data tbody').off('click').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tab_data.row(tr);
        var idx = $.inArray(tr.attr('id'), detailRows);

        if (row.child.isShown()) {
            tr.removeClass('details');
            row.child.hide();
            detailRows.splice(idx, 1);
        }
        else {
            tr.addClass('details');
            row.child(getDetailJob(row.data())).show();
            if (idx === -1) {
                detailRows.push(tr.attr('id'));
            }
        }
    });
    
    tab_data.off('draw').on('draw', function () {
        console.log(detailRows);
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });    
}
function getDetailJob(d) {
    var div = $('<div/>')
        .addClass('loading')
        .text('Loading...');
    callAPI(`${API_URL}/job/view?id=${d.id_job}`, null, "GET", function (res) {
        if (res.success) {
            let data_html = "";
            let item = res.data;
            data_html += `<div class="row">`;
            data_html += `<div class="col-sm-7"><ul>`;
            data_html += `<li>Đơn giá: <span class="font-weight-bold">${formatCurency(item.don_gia)}</span></li>`;
            data_html += `<li>Liên hệ: <span class="font-weight-bold">${formatCurency(item.nguoi_lien_he)}</span></li>`;
            data_html += `<li>Lương chính thức: <span class="font-weight-bold">${formatCurency(item.luong_chinh_thuc)}</span></li>`;
            data_html += `</ul></div>`;
            data_html += `<div class="col-sm-4"><ul>`;
            data_html += `<li>Làm việc tại:`;
            item.tinh_thanh.forEach(tt => {
                data_html += `${tt.ten_tinh} `;
            });
            data_html += `</li>`;
            if (item.cong_ty != null) {
                data_html += `<li>Công ty: <span class="font-weight-bold">${item.cong_ty.ten_cong_ty}</span></li>`;
            }
            data_html += `<li>Chủ sở hữu: <span class="font-weight-bold">${item.nguoi_tao}</span></li>`;
            data_html += `</ul></div>`;            
            

            div.html(data_html)
                .removeClass('loading');
        }
    });
    return div;
}

function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/userjob/delete?id=${id}`,
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