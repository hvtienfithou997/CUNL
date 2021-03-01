
$(function () {
    let url = document.location.href;
    let id = url.substring(url.lastIndexOf('/') + 1);
    callAPI(`${API_URL}/tinhthanh/${id}`, { "id": id }, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                
                $("#id_tinh").val(res.data.id_tinh);
                $("#ten_tinh").val(res.data.ten_tinh);
                $("#ten_viet_tat").val(res.data.ten_viet_tat);
                
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})

function onUpdate() {
    let id_tinh = $('#id_tinh').val();
    let ten_tinh = $('#ten_tinh').val();
    let ten_viet_tat = $('#ten_viet_tat').val();

    var obj = {
        "ten_tinh": ten_tinh, "ten_viet_tat": ten_viet_tat,
        "nguoi_sua": user, "ngay_sua": Math.floor(new Date() / 1000.0) };

    callAPI(`${API_URL}/TinhThanh/${id_tinh}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};
