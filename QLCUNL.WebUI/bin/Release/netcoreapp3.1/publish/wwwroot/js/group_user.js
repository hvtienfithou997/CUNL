

function onSubmit() {
    
    let team_name = $('#team_name').val();
    
    var obj = {
        "team_name": team_name, "nguoi_tao": user, "nguoi_sua": user
    };

    callAPI(`${API_URL}/groupuser/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function search() {
    let term = $(`[name='term']`).val();
    callAPI(`${API_URL}/groupuser/search?term=${term}&page=1`, null, "GET", function (res) {
        console.log(res);
        if (res.success && res.data != null) {
            let html = "";
            $('.total-recs').html('Tổng số bản ghi:' + res.total);
            res.data.forEach(item => {
                html += `<tr>`;
                html += `<td>${item.team_name} (${item.app_id})</td><td>${epochToTime(item.ngay_tao)}</td><td>${item.nguoi_tao}</td>`;
                html += `<td><a class="btn btn-warning btn-rieng" href="edit?id=${item.id_team}">Sửa</a>`;
                html += `<button class="btn btn-danger btn-rieng" id="${item.id_team}" onclick="deleteRec(this.id)">Xóa</a></td>`;
                html += `</tr>`;
            });
            $("#div_data").html(html);
        }
    });
}
function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/groupuser/delete?id=${id}`,
            null,
            "DELETE",
            function (res) {
                if (res) {
                    $.notify("Xóa thành công", "success");
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
            });
    } else {
        return false;
    }
};
function bindDetail(id) {

    callAPI(`${API_URL}/groupuser/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                
                $("#user_detail").empty();
                $("#user_detail").append(`<li>Full Name :<span class="font-weight-bold">${res.data.team_name}</span></li>`);                
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}