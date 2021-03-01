$(document).ready(function () {
    var form = $('#validate-menu');
    form.validate({
        errorClass: 'errors',
        rules: {
            ten_menu: {
                required: true
            },
            url: {
                required: true
            },
            order: {
                required: true
            }

        },
        messages: {
            ten_menu: {
                required: "Bạn phải nhập tên menu"
            },
            url: {
                required: "Bạn phải url cua menu"
            },
            order: {
                required: "Bạn phải nhập order"
            }
        }
    });
});

function onSubmit(event) {
    event.preventDefault();
    if ($('#validate-menu').valid()) {
        let ten_menu = $('#ten_menu').val();
        let url = $('#url').val();
        let order = $('#order').val();
        var obj = {
            "ten_menu": ten_menu, "url": url, "order": order, "nguoi_tao": user, "nguoi_sua": user
        };

        callAPI(`${API_URL}/Menu/add`, obj, "POST", function (res) {
            if (res.success) {
                $.notify("Thêm thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};

function bindDetail(id) {
    callAPI(`${API_URL}/Menu/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                let detail_html = '';
                detail_html += `<li>Tên menu :<span class="font-weight-bold">${res.data.ten_menu}</span></li>`;
                detail_html += `<li>URL :<span class="font-weight-bold">${res.data.url}</span></li>`;
                detail_html += `<li>Thứ tự :<span class="font-weight-bold">${res.data.order}</span></li>`;
                $("#menu_detail").html(detail_html);
                $('#menu_detail').children().addClass("list-group-item");
                $('#menu_detail li').children().addClass("list-group list-group-flush").children().addClass("list-group-item");
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function search(page) {
    let term = $("[name='term']").val();

    callAPI(`${API_URL}/Menu/search?term=${term}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        $("#div_data").html(DIV_LOADING);
        if (res.success) {
            $("#div_data").empty();
            $(".totalRecs").html("Tổng số menu: " + res.total);
            if (res.data != null && res.data.length > 0) {
                let html_data = "";
                res.data.forEach(item => {
                    html_data += `<tr>`;
                    html_data += `<td>${item.ten_menu}</td>`;
                    html_data += `<td>${item.url}</td>`;
                    html_data += `<td>${item.order}</td>`;

                    html_data += `<td><a class="btn-big btn btn-warning" href="/menu/edit?id=${item.id_menu
                        }">Sửa</a><a class="btn-big btn btn-success" href="/menu/detail/${item.id_menu
                        }">Xem</a><a class="btn-big btn btn-info" href="/menu/share?id=${item.id_menu
                        }">Chia sẻ</a><button class="btn-big btn btn-danger" id="${item.id_menu}"
                             onclick="deleteRec(this.id)">Xóa</button></td>`;
                    html_data += `</tr>`;
                });

                $("#div_data").html(html_data);
            }
            /*START_PAGING*/
            paging(res.total, 'search', page);
            /*END_PAGING*/
        } else {
            $("#div_data").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/menu/delete?id=${id}`,
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

