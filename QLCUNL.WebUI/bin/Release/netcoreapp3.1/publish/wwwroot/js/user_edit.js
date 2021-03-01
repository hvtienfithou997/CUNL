$(function () {
    callAPI(`${API_URL}/User/view?id=${id}`, null, "GET", function (res) {
        console.log(res);
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#id_user").val(res.data.id_user);
                $("#user_name").val(res.data.user_name);
                $("#full_name").val(res.data.full_name);
                $("#id_team").val(res.data.id_team);
                $("#password").val(res.data.password);
                $("#app_id").val(res.data.app_id);
                $("#type").val(res.data.type);
                $("#trang_thai").val(res.data.trang_thai);
                if (res.data.settings != null) {
                    $("#tim_ung_vien_team_khac").prop("checked", res.data.settings.tim_ung_vien_team_khac);
                    $("#bao_hanh").val(res.data.settings.bao_hanh);
                    $("#so_lan_doi").val(res.data.settings.so_lan_doi);
                    $("#tien_coc").val(res.data.settings.tien_coc);
                }
                if (res.data.roles != null)
                    res.data.roles.forEach(item => {
                        $(`[name='role'][value='${item}']`).prop("checked", true)
                    });
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
});

function onUpdate() {
    let user_name = $('#user_name').val();
    let full_name = $('#full_name').val();
    let id_team = $('#id_team').val();
    let type = $("#type").val();
    let app_id = $("#app_id").val();
    let trang_thai = $("#trang_thai > option:selected").val();
    let tim_ung_vien_team_khac = $("#tim_ung_vien_team_khac").is(":checked");
    let tien_coc = $("#tien_coc").val();
    let bao_hanh = $("#bao_hanh").val();
    let so_lan_doi = $("#so_lan_doi").val();
    let roles = [];
    $("[name='role']:checked").each(function () {
        roles.push($(this).val());
    });
    var obj = {
        "id_user": id, "user_name": user_name, "full_name": full_name, "id_team": id_team, "type": type, "app_id": app_id, "roles": roles, "trang_thai": trang_thai,
        "settings":
        {
            "tim_ung_vien_team_khac": tim_ung_vien_team_khac,
            "tien_coc": tien_coc,
            "bao_hanh": bao_hanh,
            "so_lan_doi": so_lan_doi
        }
    };
    callAPI(`${API_URL}/User/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function onChangePass() {
    let password = $('#password').val();
    let oldPassword = $('#old-password').val();

    let newPassword = $('#new-password').val();
    let reNewPassword = $('#re-new-password').val();
    if (oldPassword != "" && newPassword != "" && reNewPassword != "") {
        if (oldPassword == password) {
            if (newPassword.length >= 6 && reNewPassword.length >= 6) {
                if (newPassword == reNewPassword) {
                    var obj = {
                        "id_user": id, "password": newPassword, "old_password": password
                    };
                    callAPI(`${API_URL}/User/change`, obj, "PUT", function (res) {
                        if (res.success) {
                            $.notify("Đổi mật khẩu thành công", "success");
                        } else {
                            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                        }
                    });
                } else {
                    $('.valid-value').html("Mật khẩu mới không khớp");
                }
            } else {
                $('.valid-value').html("Mật khẩu mới phải nhiều hơn 6 ký tự");
            }
        } else {
            $('.valid-value').html("Mật khẩu cũ không chính xác");
        }
    } else {
        $('.valid-value').html("Bạn cần phải nhập đầy đủ các trường");
    }
}

function editForUser() {
    let full_name = $('#full_name').val();
    var obj = {
        "id_user": id, "full_name": full_name
    }
    callAPI(`${API_URL}/User/useredit`, obj, "PUT", function (res) {

        if (res.success) {
            $.notify("Đổi thông tin thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}