$(function() {
    callAPI(`${API_URL}/NoteUngVien/view?id=${id}`,
        null,
        "GET",
        function(res) {
            if (res.success) {
                if (res.data != null && res.data != undefined && res.data != "") {
                    
                    $("#id_note_ung_vien").val(res.data.id_note_ung_vien);
                    $("#id_ung_vien").val(res.data.id_ung_vien);
                    $("#ten_ung_vien").val(res.data.ten_ung_vien);
                   
                    $("#ghi_chu").val(res.data.ghi_chu);

                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
});

function onUpdate() {
   
    let id_ung_vien = $('#id_ung_vien').val();
    
    let ghi_chu = $('#ghi_chu').val();

    var obj = {
        "user_name": "", "ghi_chu": ghi_chu, "id_ung_vien": id_ung_vien,
        "nguoi_sua": user, "ngay_sua": Math.floor(new Date() / 1000.0) };

    callAPI(`${API_URL}/NoteUngVien/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};
