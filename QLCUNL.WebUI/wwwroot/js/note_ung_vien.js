$(document).ready(function () {
    
    let page_index = 1;
    let page_size = 50;

    let path = API_URL + "/noteungvien/get/all";
    $.ajax({
        url: path,
        type: "POST",
        datatype: "json",
        headers: { "Content-Type": "application/json" },
        data: (JSON.stringify({
            term: '',
            page_index: page_index,
            page_size: page_size
        })),
        success: function (data) {

            if (data.success) {
                $.each(data.data,
                    function (key, value) {
                        $('.table-data').append("<tr>");
                        $('.table-data').append(`<td>${value.id_note_ung_vien}</td>`);
                        $('.table-data').append(`<td>${value.id_ung_vien}</td>`);
                        $('.table-data').append(`<td>${value.user_name}</td>`);
                        $('.table-data').append(`<td>${value.ghi_chu}</td>`);
                        $('.table-data').append(`<td>${value.ngay_tao}</td>`);
                        $('.table-data').append(`<td>${value.nguoi_tao}</td>`);
                        $('.table-data').append(`<td>${value.ngay_sua}</td>`);
                        $('.table-data').append(`<td>${value.nguoi_sua}</td>`);
                        $('.table-data').append(`<td><a href="edit?id=${value.id_note_ung_vien}">Edit</a> | <a href="detail?id=${value.id_note_ung_vien}">Detail</a></tr>`);
                        $('.table-data').append("/<tr>");
                    });
            }
        },
        error: function (e) {
            console.log("error");
        }
    });
});

function onSubmit() {    
    let id_ung_vien = $('#id_ung_vien').val();
    let ghi_chu = $('#ghi_chu').val();
    let user_name = $('#user_name').val();
    var obj = {
        "id_ung_vien": id_ung_vien, "user_name": user_name, "ghi_chu": ghi_chu, "nguoi_tao": user, "ngay_tao": Math.floor(new Date() / 1000.0),
        "nguoi_sua": user, "ngay_sua": Math.floor(new Date() / 1000.0) };    
    callAPI(`${API_URL}/NoteUngVien/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function bindDetail(id) {
    
    callAPI(`${API_URL}/NoteUngVien/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                console.log(res);
                $("#noteungvien_detail").empty();
                $("#noteungvien_detail").append(`<li>Tên ứng viên: <span class="font-weight-bold">${res.data.ten_ung_vien}</span></li>`);
                $("#noteungvien_detail").append(`<li>Tên người dùng :<span class="font-weight-bold">${res.data.user_name}</span></li>`);
                $("#noteungvien_detail").append(`<li>Ghi chú :<span class="font-weight-bold">${res.data.ghi_chu}</span></li>`);
                $("#noteungvien_detail").append(`<li>Ngày tạo :<span class="font-weight-bold">${epochToTime(res.data.ngay_tao)}</span></li>`);
                $("#noteungvien_detail").append(`<li>Ngày sửa :<span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_sua)}</span></li>`);
                $("#noteungvien_detail").children().addClass('list-group-item');
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

