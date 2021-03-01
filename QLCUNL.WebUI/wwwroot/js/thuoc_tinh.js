$(document).ready(function () {
    //$("#loai option[value='1']").remove();
});

function onSubmit() {
    let loai = $('#loai').val();
    if (loai == null) { loai = "0" }
    else if (loai == "-1") {
        $("#message-error").html("Bạn bắt buộc phải chọn loại thuộc tính");
        return;
    }

    let ten = $('#ten').val();
    let nhom = $('#nhom').val();
    let type = $('#type').val();
    let nguoi_tao = $("#nguoi_tao >option:selected").val();
    var obj = {
        "loai": loai,
        "gia_tri": 0,
        "ten": ten,
        "nhom": nhom,
        "type": type, "nguoi_tao": nguoi_tao
    };

    callAPI(`${API_URL}/thuoctinh/add`,
        obj,
        "POST",
        function (res) {
            if (res.success) {
                $.notify("Thêm thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
};

function search(page) {
    let term = $(`[name='term']`).val();
    if (typeof page === 'undefined') {
        page = 1;
    }
    let loai = $('#loai').val() != '' ? $('#loai').val() : -1;
    callAPI(`${API_URL}/ThuocTinh/search?term=${term}&loai=${loai}&type=-1&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        $("#all_thuoc_tinh").html(DIV_LOADING);
        if (res.success) {

            $("#all_thuoc_tinh").empty();
            $(".totalRecs").html("Tổng số thuộc tính: " + res.total);
            if (res.data != null && res.data.length > 0) {
                let html_data = "";

                res.data.forEach(item => {

                    html_data += `<tr>`;
                    switch (item.loai) {
                        case 0:
                            item.loai = "Công ty";
                            break;
                        case 1:
                            item.loai = "JOB đã gán";
                            break;
                        case 2:
                            item.loai = "Note Ứng Viên";
                            break;
                        case 3:
                            item.loai = "Ứng Viên";
                            break;
                        case 4:
                            item.loai = "JOB";
                            break;
                        case 5:
                            item.loai = "Note Ứng Viên theo JOB";
                            break;
                        case 6:
                            item.loai = "Note";
                    }
                    html_data += `<td>${item.ten}</td>`;
                    html_data += `<td>${item.nhom}</td>`;
                    html_data += `<td>${item.loai}</td>`;
                    item.type = item.type == "0" ? shared : private;
                    html_data += `<td>${item.type}</td>`;
                    html_data += `<td>${item.nguoi_tao}</td>`;
                    html_data += `<td><a class="btn-rieng btn btn-warning" href="edit?id=${item.id}" style="color:#fff;">Sửa</a><a class="btn-rieng btn btn-success" href="detail/${item.id}">Xem</a>`;
                    html_data += `<button class="btn-rieng btn btn-danger" id="${item.id}" onclick="deleteRec(this.id)">Xóa</button>`;
                    html_data += `</tr>`;
                });
                $("#all_thuoc_tinh").html(html_data);
            }
            paging(res.total, 'search', page);
        } else {
            $("#all_thuoc_tinh").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/thuoctinh/delete?id=${id}`,
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

function bindDetail(id) {
    callAPI(`${API_URL}/thuoctinh/view?id=${id}`, null, "GET", function (res) {

        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#thuoc_tinh_detail").empty();
                switch (res.data.loai) {
                    case 0:
                        res.data.loai = "Công ty";
                        break;
                    case 1:
                        res.data.loai = "User Job";
                        break;
                    case 2:
                        res.data.loai = "Note Ứng Viên";
                        break;
                    case 3:
                        res.data.loai = "Ứng Viên";
                        break;
                    case 4:
                        res.data.loai = "Job";
                        break;
                    case 5:
                        res.data.loai = "Note Ứng Viên Job";
                        break;
                    default:
                }

                $("#thuoc_tinh_detail").append(`<li>Loại: <span class="font-weight-bold">${res.data.loai}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Giá trị: <span class="font-weight-bold">${res.data.gia_tri}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Tên: <span class="font-weight-bold">${res.data.ten}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Nhóm: <span class="font-weight-bold">${res.data.nhom}</span></li>`);
                res.data.type = res.data.type == "0" ? shared : private;
                $("#thuoc_tinh_detail").append(`<li>Trạng thái: <span class="font-weight-bold">${res.data.type}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Ngày tạo: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_tao)}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Người tạo: <span class="font-weight-bold">${res.data.nguoi_tao}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Ngày sửa: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_sua)}</span></li>`);
                $("#thuoc_tinh_detail").append(`<li>Người sửa: <span class="font-weight-bold">${res.data.nguoi_sua}</span></li>`);
                $('#thuoc_tinh_detail li ').addClass("list-group-item");
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}