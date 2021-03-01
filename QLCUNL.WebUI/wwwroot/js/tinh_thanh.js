function onSubmit() {
    let ten_tinh = $('#ten_tinh').val();
    let ten_viet_tat = $('#ten_viet_tat').val();

    var obj = {
        "ten_tinh": ten_tinh, "ten_viet_tat": ten_viet_tat, "nguoi_tao": user, "ngay_tao": Math.floor(new Date() / 1000.0),
        "nguoi_sua": user, "ngay_sua": Math.floor(new Date() / 1000.0) };

    callAPI(`${API_URL}/TinhThanh`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};


$(function () {
    let url = document.location.href;
    let id = url.substring(url.lastIndexOf('/') + 1);
    bindDetail(id);
})

function bindDetail(id) {
    callAPI(`${API_URL}/TinhThanh/${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                
                $("#tinhthanh_detail").empty();
                $("#tinhthanh_detail").append(`<li>Id :<span class="font-weight-bold">${res.data.id_tinh}</span></li>`);
                $("#tinhthanh_detail").append(`<li>Ten tinh :<span class="font-weight-bold">${res.data.ten_tinh}</span></li>`);
                $("#tinhthanh_detail").append(`<li>Tên viết tắt :<span class="font-weight-bold">${res.data.ten_viet_tat}</span></li>`);
                
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}