$(function () {
    callAPI(`${API_URL}/thuoctinh/view?id=${id}`, null, "GET", function (res) {
        console.log(res.data);
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                
                $("#loai").val(res.data.loai);
                $("#gia_tri").val(res.data.gia_tri);
                $("#ten").val(res.data.ten);
                $("#nhom").val(res.data.nhom);
                $("#type").val(res.data.type);
                

            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})

function onUpdate() {
    let loai = $('#loai').val();
    let gia_tri = $('#gia_tri').val();
    let ten = $('#ten').val();
    let nhom = $('#nhom').val();
    let type = $('#type').val();
    
   
   

    var obj = {
        "nguoi_sua": user, "loai":loai, "gia_tri":gia_tri,"ten":ten,"nhom":nhom,"type":type
    };

    callAPI(`${API_URL}/ThuocTinh/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};