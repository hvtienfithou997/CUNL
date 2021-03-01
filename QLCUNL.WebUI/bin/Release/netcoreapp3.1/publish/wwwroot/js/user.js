function onSubmit() {
    let user_name = $('#user_name').val();
    let full_name = $('#full_name').val();
    let id_team = $('#id_team').val();
    let password = $('#password').val();
    let email = $('#email').val();
    let type = $("#type").val();
    let app_id = $("#app_id").val();
    let tim_ung_vien_team_khac = $("#tim_ung_vien_team_khac").is(":checked");
    let tien_coc = $("#tien_coc").val();
    let bao_hanh = $("#bao_hanh").val();
    let so_lan_doi = $("#so_lan_doi").val();
    let roles = [];
    $("[name='role']:checked").each(function () {
        roles.push($(this).val());
    });
    var obj = {
        "user_name": user_name, "full_name": full_name, "id_team": id_team, "password": password, "email": email,
        "app_id": app_id, "type": type, "old_password": "", "roles": roles, "settings":
        {
            "tim_ung_vien_team_khac": tim_ung_vien_team_khac,
            "tien_coc": tien_coc,
            "bao_hanh": bao_hanh,
            "so_lan_doi": so_lan_doi
        }
    };

    callAPI(`${API_URL}/user/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function search(page) {
    let term = $(`[name='term']`).val();
    if (typeof page === 'undefined') {
        page = 1;
    }
    let id_team = $('#group_user').val();
    
    callAPI(`${API_URL}/User/search?term=${term}&id_team=${id_team}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        if (res.success && res.data != null) {
            $(".totalRecs").html("Tổng số người dùng: " + res.total);
            console.log(res);
            let html = "";
            res.data.forEach(item => {
                html += `<tr>`;
                html += `<td>${item.user_name} (${item.full_name})<br>${item.email}</td>`;
                html += `<td>${item.email}</td>`;
                html += `<td>${item.team_name}</td>`;
                html += `<td>${item.type == 0 ? "USER" : "ADMIN"}</td>`;
                html += `<td>${epochToTime(item.last_login)} <br>${item.browser != null ? item.browser : ""}</td><td class="last-button"><a class="btn btn-warning" href="edit?id=${item.id_user}">Sửa</a>`;
                html += `<a class="btn btn-danger delete" href="#" id="${item.id_user}" onclick="deleteRec(this.id)">Xóa</a>`;
                html += `<button class="btn btn-danger" data-toggle="modal" data-target="#modal-reset-password" id="${item.id_user}" onclick="showForm(this.id)">Reset mật khẩu</button></td>`;
                html += `</tr>`;
            });
            $("#div_data").html(html);
        }
    });
}
function showForm(id) {
    let html = `<input type=text value="${id}" class='d-none' id="id-user">`;
    html += `<h4>Nhập mật khẩu mới</p>`;
    html += `<input type="password" class="form-control" id="password-input">`;
    $('#reset-password').html(html);
}
function resetPassword() {
    let id = $('#id-user').val();
    let reset_password = $('#password-input').val();
    var obj = {
        "id_user": id, "password": reset_password
    };
    callAPI(`${API_URL}/User/reset`, obj, "PUT", function (res) {
        console.log(res);
        if (res.success) {
            $.notify("Đặt lại mật khẩu thành công", "success");
            $('#modal-reset-password').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function bindDetail(id) {
    callAPI(`${API_URL}/User/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                console.log(res.data);
                $("#user_detail").empty();
                $("#user_detail").append(`<li>Id :<span class="font-weight-bold">${res.data.id_user}</span></li>`);
                $("#user_detail").append(`<li>User name :<span class="font-weight-bold">${res.data.user_name}</span></li>`);
                $("#user_detail").append(`<li>Full Name :<span class="font-weight-bold">${res.data.full_name}</span></li>`);
                $("#user_detail").append(`<li>Id Team :<span class="font-weight-bold">${res.data.id_team}</span></li>`);
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/user/delete?id=${id}`,
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