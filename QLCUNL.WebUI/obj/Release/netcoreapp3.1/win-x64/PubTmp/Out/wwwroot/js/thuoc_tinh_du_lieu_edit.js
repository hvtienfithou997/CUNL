$(function () {
    let url = document.location.href;

    let id = url.substring(url.lastIndexOf('=') + 1);
    callAPI(`${API_URL}/thuoctinh/view?id=${id}`, null, "GET", function (res) {
        
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#id").val(res.data.id);
                $("#loai").val(res.data.loai);
                $("#gia_tri").val(res.data.gia_tri);
                $("#ten").val(res.data.ten);
                $("#nhom").val(res.data.nhom);
                $("#type").val(res.data.dia_chi);


            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})