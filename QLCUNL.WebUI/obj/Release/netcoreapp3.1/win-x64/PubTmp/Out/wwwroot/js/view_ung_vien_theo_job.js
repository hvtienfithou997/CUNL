function search(page) {


    let tab_ung_vien = $('#table_data').DataTable({
        destroy: true, "ordering": false, "autoWidth": false,
        "dom": 'rt',/*https://datatables.net/reference/option/dom*/
        searchPanes: {
            controls: false, orderable: false
        },
        "language": {
            "emptyTable": "Không tìm thấy ứng viên theo JOB"
        },
        ajax: function (data, callback) {
            let url = `${API_URL}/NoteUngVienJob/getnotebyidjob?id_job=${id_job}`;
            $.get(url, {
            }, function (res) {
               
                $(".totalRecs").html("Tổng ứng viên trong Job: " + res.total);
                paging(res.total, 'search', page);
                callback({
                    recordsTotal: res.total,
                    recordsFiltered: res.total,
                    data: res.data
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
        { "data": "ho_ten_ung_vien", "name": "Họ tên", "searchable": true, "width": "150px" },

        { "data": "thuoc_tinh", "name": "Trạng thái", "searchable": false, "width": "50px" },
        { "data": "ngay_gio_phong_van", "name": "Giờ phỏng vấn", "searchable": false, "width": "200px" },
        { "data": "ngay_di_lam", "name": "Đi làm", "searchable": false, "width": "150px" },
        { "data": "luong_chinh_thuc", "name": "Lương thử/chính", "searchable": false, "width": "150px" },
        { "data": "ghi_chu", "name": "Ghi chú", "searchable": false, "width": "65px" }
        ],
        "columnDefs": [{
            "render": function (data, type, row) {
                let html_tmp = "";
                row.thuoc_tinh.forEach(item => {
                    html_tmp += `- ${item.ten}<br>`;
                });
                return html_tmp;
            },
            "targets": 3
        }, {
            "render": function (data, type, row) {
                return epochToTimeWithHour(row.ngay_gio_phong_van)
            },
            "targets": 4
        }, {
            "render": function (data, type, row) {
                return epochToTime(row.ngay_di_lam)
            },
            "targets": 5
        },
        {
            "render": function (data, type, row) {
                let html_tmp = `${formatCurency(row.luong_chinh_thuc)}`;

                return html_tmp;
            },
            "targets": 5
        }, {
            "render": function (data, type, row) {
                let html_tmp = `<a href="#" data-toggle="modal" data-link="${row.id_ung_vien}" onclick="showPreviewJob(this)" data-target="#preview-job">${row.ho_ten_ung_vien}</a><br><b class="auto_id">${row.email}<br>${row.so_dien_thoai}</b>`;


                return html_tmp;
            },
            "targets": 2
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
            row.child(getDetailUngVien(row.data())).show();

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
    // Lấy chi tiết job được chọn


}
function showPreviewCV(e) {
    var link = $(e).attr('data-link');
    if (link != "") {
        var url_temp = "";
        var ext = link.split('.').pop();
        url_temp = ext == "pdf" ? link : "https://view.officeapps.live.com/op/view.aspx?src=" + link;
        $("#link_file_upload").empty();
        $("#link_file_upload").append(`<iframe id="iframe-cv" src="${url_temp}"></iframe>`);
    } else {
        $("#link_file_upload").empty();
        $("#link_file_upload").append(`<p>Không có CV</p>`);
    }
}
function getDetailUngVien(row) {
    var div = $('<div/>')
        .addClass('loading row_detail')
        .text('Loading...');
    callAPI(`${API_URL}/UngVien/viewinjob?id=${row.id_ung_vien}&id_job=${row.id_job}&id_user_job=${row.id_user_job}`, null, "GET", function (res) {
        let data_html = "";
        
        if (res.success) {
            data_html += `<kbd><div class="text-light float-right" style="color:#fff; background-color:#000;">Người thực hiện: ${res.data.user_job_name}</div>`;
            data_html += `<div>Ứng tuyển: <strong>${res.data.vi_tri_ung_tuyen}</strong></div>`;
            data_html += `<div>Địa chỉ: <strong>${formatCurency(res.data.dia_chi)}</strong></div>`;
            data_html += `<div>Lương mong muốn: <strong>${formatCurency(res.data.luong_mong_muon)}</strong></div>`;
            data_html += `<div>CV: <a href="#" data-toggle="modal" data-link="${res.data.link_cv}" onclick="showPreviewCV(this)" data-target="#preview-job">${res.data.link_cv}</a></div>`;
            data_html += `<div>${res.data.noi_dung.replace(/-/g, '<br>-')}</div></kbd>`;
            
        }
        div.html(data_html)
            .removeClass('loading');
    });
    return div;
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
                    html_job += `<h6>Địa chỉ Job:</h6>`;
                    res.data.tinh_thanh.forEach((item) => {
                        html_job += `<span><strong> - ${item.ten_tinh} </strong></span>`;
                    })
                    html_job += `<h6>Số lượng: <strong>${res.data.so_luong} (người)</strong></h6>`;
                    html_cong_ty += `<h6>Tên công ty: <strong>${res.data.cong_ty.ten_cong_ty}</strong></h6>`;
                    html_cong_ty += `<h6>Địa chỉ: <strong>${res.data.cong_ty.dia_chi != null ? res.data.cong_ty.dia_chi : ""}</strong></h6>`;
                    html_cong_ty += `<h6>Điện thoại: <strong>${res.data.cong_ty.dien_thoai != null ? res.data.cong_ty.dien_thoai : ""}</strong></h6>`;
                    html_cong_ty += `<h6>Người gửi HĐ: <strong>${res.data.cong_ty.info_gui_hop_dong != null ? res.data.cong_ty.info_gui_hop_dong : ""}</strong></h6>`;
                    $('.info-job-left').html(html_job);
                    $('.info-job-right').html(html_cong_ty);
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
}