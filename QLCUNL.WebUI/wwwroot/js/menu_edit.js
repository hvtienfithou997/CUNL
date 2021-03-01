$(function () {
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
                required: "Bạn phải nhập url"
            },
            order: {
                required: "Bạn phải nhập order"
            }
        }
    });

    callAPI(`${API_URL}/menu/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                
                $("#id_menu").val(res.data.id_menu);
                $("#ten_menu").val(res.data.ten_menu);
                $("#url").val(res.data.url);
                $("#order").val(res.data.order);
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})

function onUpdate(event) {
    event.preventDefault();
    if ($('#validate-menu').valid()) {
        let id_menu = $('#id_menu').val();
        let ten_menu = $('#ten_menu').val();
        let url = $('#url').val();
        let order = $('#order').val();
        var obj = {
            "id_menu": id_menu, "ten_menu": ten_menu, "url": url, "order": order, "nguoi_sua": user
        };

        callAPI(`${API_URL}/Menu/${id_menu}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Sửa thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};