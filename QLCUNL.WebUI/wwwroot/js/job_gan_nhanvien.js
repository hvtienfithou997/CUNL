let listnhanvienchecked = [];

function onSubmit() {
    let user_name = $('#user_name').val();
    let full_name = $('#full_name').val();
    let id_team = $('#id_team').val();
    let password = $('#password').val();
    let email = $('#email').val();
    let type = $("#type").val();
    var obj = {
        "user_name": user_name, "full_name": full_name, "id_team": id_team, "password": password, "email": email,
        "nguoi_tao": user, "nguoi_sua": user, "type": type
    };

    callAPI(`${API_URL}/user/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function search1() {
 
    if (listjobchecked.length > 0) {
        $('#btn-gan').data('target', 'myModal1');

        let term = $(`[name='term1']`).val();
        callAPI(`${API_URL}/User/all?term=${term}`,
            null,
            "GET",
            function (res) {
                if (res.success && res.data != null) {
                    let html = "";
                    res.data.forEach(item => {
                        html += `<tr>`;
                        html += `<td><input type="checkbox" value="${item.id_user
                            }" name="nhanvienUnchecked" onclick="nhanvienchecked()" id="nhanvienUnchecked"></td>`;
                        html += `<td>${item.user_name}</td><td>${item.full_name}</td><td>${item.email}</td><td>${item
                            .id_team}</td>`;
                        html += `</tr>`;
                    });
                    $("#div_data1").html(html);
                }
            });
    } else {
        $("#div_data1").empty();

        $.notify(`Vui lòng chọn JOB`, "error");
    }
}

function tim() {
    search1(1);
}

function nhanvienchecked() {
    listnhanvienchecked = [];
    $("input[name='nhanvienUnchecked']:checked").each(function (el) {
        try {
            listnhanvienchecked.push($(this).val());
        } catch{
        }
    });
}

function onAssignJobs_NhanVien() {
    if (listjobchecked.length > 0) {
        if (listnhanvienchecked.length > 0) {
            $.each(listjobchecked, function (index, value) {
                var obj = { "id": value, "users": listnhanvienchecked }

                callAPI(`${API_URL}/UserJob/grant`, obj, "POST", function (res) {
                    if (res.success) {
                        $.notify("Gán thành công", "success");
                        $('#myModal1').modal('toggle');
                    } else {
                        $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                    }
                });
            });
        }
        else {
            $.notify(`Vui lòng chọn nhân viên`, "error");
        }
    }
    else {
        $.notify(`Vui lòng chọn job`, "error");
    }
}

function onOneAssignJobs_NhanVien(id) {
    if (listjobchecked.length > 0) {
        let list_nv = [];
        list_nv.push(id);
        if (list_nv.length > 0) {
            $.each(listjobchecked, function (index, value) {
                var obj = { "id": value, "users": list_nv }
                callAPI(`${API_URL}/UserJob/grant`, obj, "POST", function (res) {
                    if (res.success) {
                        $.notify("Gán thành công", "success");
                    } else {
                        $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                    }
                });
            });
        }
        else {
            $.notify(`Vui lòng chọn nhân viên`, "error");
        }
    }
    else {
        $.notify(`Vui lòng chọn job`, "error");
    }
}