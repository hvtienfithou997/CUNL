function onSubmit() {
    let loai_obj = $('#loai_obj ').val();
    if (loai_obj == null) { loai_obj = "0" }
    let id_obj = $('#id_obj').val();

    let thuoc_tinh = [];
    $("input[name^='thuoc_tinh']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch{
        }
    });

    var obj = { "thuoc_tinh": thuoc_tinh, "loai_obj": loai_obj, "id_obj": id_obj, "nguoi_tao": user, "nguoi_sua": user };

    callAPI(`${API_URL}/thuoctinhdulieu`,
        obj,
        "POST",
        function (res) {
            if (res.success) {
                $.notify("Thêm thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
};
$(function () {
    $("#loai_obj").change(function () {
        let loai = $(this).val();

        callAPI(`${API_URL}/thuoctinh/loai?id=${loai}&type=1`, null, "GET", function (res) {
            if (res.success) {
                let data = res.data;
                let html_op = `<div class="form-group"><ul class="check-box">`;
                data.forEach(item => {
                    html_op += `<li><input type="checkbox" name="thuoc_tinh" value="${item.gia_tri}"> <span class="font-weight-bold">${item.ten}</span></li>`;
                    
                });
                html_op += "</ul></div>";
                $("#div_thuoc_tinh").html(html_op);
            }
        });
    });
});


function search(term) {

    callAPI(`${API_URL}/ThuocTinhdulieu/search?term=${term}&page=1`, null, "GET", function (res) {
        console.log(res.data)
        $("#table_data").html(DIV_LOADING);
        if (res.success) {
            $("#table_data").empty();

            if (res.data != null && res.data.length > 0) {
                let html_data = "";
                res.data.forEach(item => {

                    html_data += `<tr>`;
                    switch (item.loai) {

                        case 0:
                            item.loai = "Công ty";
                            break;
                        case 1:
                            item.loai = "User Job";
                            break;
                        case 2:
                            item.loai = "Note Ứng Viên";
                            break;
                        case 3:
                            item.loai = "Ứng Viên";
                            break;
                        case 4:
                            item.loai = "Job";
                            break;
                        case 5:
                            item.loai = "Note Ứng Viên Job";
                            break;
                        default:
                    }
                    html_data += `<td>${item.loai}</td>`;
                    html_data += `<td>${item.gia_tri}</td>`;
                    html_data += `<td>${item.ten}</td>`;
                    html_data += `<td>${item.nhom}</td>`;
                    item.type = item.type == "0" ? "Share" : "Private";
                    html_data += `<td>${item.type}</td>`;
                    html_data += `<td><a href="edit?id=${item.id}">Sửa</a> | <a href="detail/${item.id}">Xem</a></td>`;

                    html_data += `</tr>`;
                });
                $("#table_data").html(html_data);
            }
        } else {
            $("#table_data").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });

}