$(function () {
    callAPI(`${API_URL}/UserJob/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
              
                
                $("#id_user_job").val(res.data.id_user_job);
                $("#job_name").val(res.data.chuc_danh);
                $("#ngay_nhan_job").val(epochToTime(res.data.ngay_nhan_job));
                for (var i = 0; i < res.data.thuoc_tinh.length; i++) {
                    $(`input[name^='thuoc_tinh'][value='${res.data.thuoc_tinh[i].gia_tri}']`).prop('checked', true);
                }                
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})


function onUpdate() {
 
    let thuoc_tinh = [];
    $("input[name^='thuoc_tinh']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch{
        }
    });
    var obj = { "id_job": id, "thuoc_tinh": thuoc_tinh,  "nguoi_sua": user};
    callAPI(`${API_URL}/UserJob/update/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

