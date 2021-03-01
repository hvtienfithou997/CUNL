

function onSubmit() {
    
    let team_name = $('#team_name').val();
    
    var obj = {
        "team_name": team_name, "nguoi_tao": user, "nguoi_sua": user
    };

    callAPI(`${API_URL}/groupuser/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Thêm thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};


function bindDetail() {
    callAPI(`${API_URL}/groupuser/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {                
                $("#team_name").val(`${res.data.team_name}`);                
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}