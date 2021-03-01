$(document).ready(function() {
    $("#loai option[value='1']").remove();
});
$(function () {
    callAPI(`${API_URL}/thuoctinh/view?id=${id}`,
        null,
        "GET",
        function(res) {
            
            if (res.success) {
                if (res.data != null && res.data != undefined && res.data != "") {

                    $("#loai").val(res.data.loai);
                    $("#gia_tri").val(res.data.gia_tri);
                    $("#ten").val(res.data.ten);
                    $("#nhom").val(res.data.nhom);
                    if (res.data.type == "0") {
                        $("#type").val("0");
                    } else {
                        $("#type").val("1");
                    }

                    $("#nguoi_tao").val(res.data.nguoi_tao);


                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
});

function onUpdate() {
    let loai = $('#loai').val();
    let gia_tri = $('#gia_tri').val();
    let ten = $('#ten').val();
    let nhom = $('#nhom').val();
    let type = $('#type').val();
    
    let nguoi_tao = $("#nguoi_tao >option:selected").val();
   

    var obj = {
        "nguoi_sua": user, "loai": loai, "gia_tri": gia_tri, "ten": ten, "nhom": nhom, "type": type, "nguoi_tao": nguoi_tao
    };

    callAPI(`${API_URL}/ThuocTinh/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};