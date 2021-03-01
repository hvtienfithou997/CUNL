$(function () {
    callAPI(`${API_URL}/nhatuyendung/searchview?term=&page=1&token=${token}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        if (res.success) {
            let html_data = "";
            let content = "";

            res.data.forEach(item => {
                html_data += `<input class='d-none' id="id_ntd" value="${item.id}"/>`
                html_data += `<p class="font-weight-bold">Tên nhà tuyển dụng: ${item.id_nha_tuyen_dung}</p>`;
                html_data += `<p class="font-weight-bold">Tên JOB: ${item.chuc_danh} (${item.id_auto})</p>`;
                //html_data += `<p class="font-weight-bold">Lương chính thức : ${formatCurency(item.luong_chinh_thuc)}</p>`
                html_data += `<p class="font-weight-bold">Địa chỉ: ${item.dia_chi}</p>`;
                content += `${item.noi_dung}`;
            });
            $("#info").html(html_data);
            $("#content-send").text(content)

            let id_obj = $("#id_ntd").val();

            let noi_dung = "truy cập website";
            let thuoc_tinh = [];
            thuoc_tinh.push(-1);
            var obj = { "loai": 6, "loai_du_lieu": 0, "id_obj": id_obj, "thuoc_tinh": thuoc_tinh, "noi_dung": noi_dung, "nguoi_tao": id }
            setTimeout(function () {
                if (log != "n") {
                    callAPI(`${API_URL}/nhatuyendung/addlogntd`, obj, "Post", function (res) {
                    })
                }
            }, 2000)
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
});

function search(page) {
    if (typeof page === 'undefined') {
        page = 1;
    }
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
            let url = `${API_URL}/nhatuyendung/ungvienjob?id=${id}&id_ung_vien=${id_ung_vien}&token=${token}`;
            $.get(url,
                {
                },
                function (res) {
                    callback({
                        recordsTotal: res.total,
                        recordsFiltered: res.total,
                        data: res.data,
                    });
                }).fail(function () {
                    callback({
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    });
                });
        },
        columns: [{
            "data": "stt", "class": "action", "width": "20px"
        },
        { "data": "ho_ten_ung_vien", "name": "Tên ứng viên", "searchable": true, "width": "150px" },
        { "data": "link_cv", "name": "link_cv", "searchable": false, "width": "150px" },
        { "data": "ngay_gio_phong_van", "name": "ngay_gio_phong_van", "searchable": false, "width": "150px" },

        { "data": "ghi_chu", "name": "ghi_chu", "searchable": false, "width": "65px" },
        {
            "data": null, "class": "action", "width": "156px"
        }
        ],
        "columnDefs": [
            { "searchable": false, "orderable": false, "targets": 0 },
            {
                "render": function (data, type, row) {
                    let html_button = "";
                    html_button += `<button class="btn btn-dark  btn-big modall btn-w21" id="${row.id_ung_vien}" data-link="${row.link_cv}" data-name="${row.ho_ten_ung_vien}" data-noidung="${row.noi_dung}" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id,'${row.id_note_ung_vien_job}',this)">Xem chi tiết</button>`;

                    return html_button;
                },
                "targets": 5
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = `<p>${row.ho_ten_ung_vien}</p>`;

                    return html_tmp;
                },
                "targets": 1
            }
            , {
                "render": function (data, type, row) {
                    let html_tmp = `${formatCurency(row.luong_mong_muon)}`;

                    return html_tmp;
                },
                "targets": 2
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = `${row.ngay_gio_phong_van != -1 ? epochToTimeWithHour(row.ngay_gio_phong_van) : ""}`;

                    return html_tmp;
                },
                "targets": 3
            },
            {
                "render": function (data, type, row) {
                    let ct = row.ghi_chu_nha_tuyen_dung.substring(0, row.ghi_chu_nha_tuyen_dung.indexOf("*"));

                    let html_tmp = `${ct}`;

                    return html_tmp;
                },
                "targets": 4
            }
        ],
        pageLength: PAGE_SIZE,
        responsive: true,
        serverSide: true,
        processing: true,
        "fnDrawCallback": function () {
            $(".auto_id").unbind('click').bind('click', function () {
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

function onShowThuocTinh(ide, id_note_uv_job, e) {
    $("#tom_tat_ung_vien").addClass('d-none');
    $('#id_ung_vien_note_job').val(id_note_uv_job);
    $("#agree-interview").prop("checked", false);
    //$("#ngay_gio_phong_van").prop("disabled", true);

    $("#ngay_gio").addClass("d-none");
    $("#dismiss-interview").prop("checked", false);
    $('#ghi_chu').val('');
    showCv(e);
    var name = $(e).attr('data-name');
    getNoteTomTat(id_note_uv_job);
    let thuoc_tinh = [];
    thuoc_tinh.push(-1);
    let noi_dung = `xem cv ứng viên ${name}`;
    var obj = { "loai": 5, "loai_du_lieu": 0, "id_obj": id_note_uv_job, "thuoc_tinh": thuoc_tinh, "noi_dung": noi_dung, "nguoi_tao": id }

    setTimeout(function () {
        if (log != "n") {
            callAPI(`${API_URL}/nhatuyendung/addlogntd`, obj, "Post", function (res) {
            })
        }
    }, 2000)
}

function getNoteTomTat(d) {
    callAPI(`${API_URL}/nhatuyendung/tomtatungvien?id=${d}`, null, "get", function (res) {
        if (res.success) {
            res.data.forEach(function (item) {
                if (item.noi_dung.length > 0) {
                    $("#thong_tin_ttuv").text("Thông tin tóm tắt ứng viên");
                    $("#ghi_chu_tom_tat").val(item.noi_dung);
                    $("#tom_tat_ung_vien").removeClass('d-none');
                }
            });
        }
    })
}

function onCreateThuocTinh() {
    let trang_thai_nha_tuyen_dung = "";
    let id_note_ung_vien_job = $('#id_ung_vien_note_job').val();
    let ghi_chu = $('#ghi_chu').val();
    let gio_phong_van = $("#ngay_gio_phong_van").val();
    let thuoc_tinh = [];
    if ($('#dismiss-interview').is(':checked')) {
        trang_thai_nha_tuyen_dung = "*(HT) NTD Từ chối PV";
    } else if ($('#agree-interview').is(':checked')) {
        trang_thai_nha_tuyen_dung = "*(HT) NTD Đồng ý PV";
    }
    thuoc_tinh.push(-1);
    let time = 0;
    if (gio_phong_van != "") {
        time = Math.floor(new Date(toDateWithHour(gio_phong_van)) / 1000.0);
    }
    deletePhongVan();
    let obj = {
        "id_note_ung_vien_job": id_note_ung_vien_job, "ngay_gio_phong_van": time, "ghi_chu_nha_tuyen_dung": { "loai": 5, "loai_du_lieu": 1, "id_obj": id_note_ung_vien_job, "noi_dung": ghi_chu + trang_thai_nha_tuyen_dung, "thuoc_tinh": thuoc_tinh, "nguoi_tao": id }
    }
    callAPI(`${API_URL}/nhatuyendung/ghichunhatuyendung`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Ghi chú thành công", "success");
            setTimeout(function () {
                search(1);
            }, 1500)
            $('#myModal').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

$("#agree-interview").click(function () {
    if ($(this).is(":checked")) {
        $("#dismiss-interview").prop("checked", false);
        let dt = new Date();
        dt.setDate(dt.getDate() + 1);
        let date_str = `${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}`;
        date_str += ` ${dt.getDate().toString().padStart(2, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getFullYear().toString().padStart(4, '0')}`;
        //$("#ngay_gio_phong_van").prop("disabled", false);
        
        $("#ngay_gio").removeClass("d-none");
        $("#ngay_gio_phong_van").val(`${date_str}`);
    } else {
        //$("#ngay_gio_phong_van").prop("disabled", true);
        $("#ngay_gio").addClass("d-none");
        $("#ngay_gio_phong_van").val("");
    }
});

$("#dismiss-interview").click(function () {
    $("#agree-interview").prop("checked", false);
})

function deletePhongVan() {
    if ($('#dismiss-interview').is(':checked')) {
        let id = $('#id_ung_vien_note_job').val();
        let time = 0;
        if (time !== "") {
            time = -1;
        }
        var obj = {
            "ngay_gio_phong_van": time
        }
        callAPI(`${API_URL}/nhatuyendung/giophongvan?id=${id}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Đã xóa thời gian phỏng vấn ứng viên.", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
}

