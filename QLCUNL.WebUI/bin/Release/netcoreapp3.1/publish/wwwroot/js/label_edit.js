
$(function () {
    let url = document.location.href;
    let id = url.substring(url.lastIndexOf('/') + 1);
    callAPI(`${API_URL}/Label/${id}`, { "id": id }, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                //console.log(res);
                $("#id_label").val(res.data.id_label);
                $("#ten_label").val(res.data.ten_label);
                $("#ghi_chu").val(res.data.ghi_chu);

            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})

function onUpdate() {
    let id_label = $('#id_label').val();
    let ten_label = $('#ten_label').val();
    let ghi_chu = $('#ghi_chu').val();

    var obj = {
        "ten_label": ten_label, "ghi_chu": ghi_chu,
        "nguoi_sua": user, "ngay_sua": Math.floor(new Date() / 1000.0) };

    callAPI(`${API_URL}/Label/${id_label}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};