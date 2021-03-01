
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
            let url = `${API_URL}/NhaTuyenDung/ungvien?id=${id}&id_ung_vien=${id_ung_vien}&token=${token}`;
            $.get(url,
                {
                },
                function (res) {

                    
                    callback({
                        recordsTotal: res.total,
                        recordsFiltered: res.total,
                        data: res.data
                    });

                }).fail(function () {
                    callback({
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    });
                });
        },
        columns: [
            { "data": "ho_ten_ung_vien", "name": "Tên ứng viên", "searchable": true, "width": "150px" },
            { "data": "dia_chi", "name": "vi_tri_ung_tuyen", "searchable": false, "width": "150px" },
            { "data": "luong_mong_muon", "name": "luong_mong_muon", "searchable": false, "width": "65px" },
            {
                "data": null, "class": "action", "width": "156px"
            }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    let html_button = "";
                    html_button += ` <a class="btn btn-success btn-big " href="/nhatuyendung/chitietungvien?id_ung_vien=${row.id_ung_vien}">Xem chi tiết</a>`

                    return html_button;
                },
                "targets": 3
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = `<a href="#" data-link="${row.link_cv}" onclick="showCv(this)" data-toggle="modal" data-target="#preview-cv">${row.ho_ten_ung_vien}</a>`;


                    return html_tmp;
                },
                "targets": 0
            }
            , {
                "render": function (data, type, row) {
                    let html_tmp = `${formatCurency(row.luong_mong_muon)}`;

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



function bindDetail(id_ung_vien) {
    callAPI(`${API_URL}/nhatuyendung/viewungvien?id_ung_vien=${id_ung_vien}`, null, "GET", function (res) {
        if (res.success) {
            console.log(res);

            $(".name-candidate h3").append(`Ứng viên:<b> ${res.data.ho_ten_ung_vien}</b>`);
            $(".ung-vien").append(`Tên ứng viên: <b>${res.data.ho_ten_ung_vien}</b>`);
            $(".dia-chi").append(`Địa chỉ: <b>${res.data.dia_chi}</b>`);
            $(".vi-tri-ung-tuyen").append(`Vị trí ứng tuyển: <b>${res.data.vi_tri_ung_tuyen}</b>`);
            $(".noi-dung").append(`Nội dung:<b> ${res.data.noi_dung}</b>`);

            let url_temp = "";
            let url = res.data.link_cv;
            if (url.length > 0) {
                var ext = url.split('.').pop();
                url_temp = ext == "pdf" ? url : "https://view.officeapps.live.com/op/view.aspx?src=" + url;
                $("#iframe_cv").append(`<iframe id="iframe-cv" src="${url_temp}"></iframe>`);
            } else {
                $("#iframe_cv").append(`<b>Ứng viên chưa tải lên CV</b>`);
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

