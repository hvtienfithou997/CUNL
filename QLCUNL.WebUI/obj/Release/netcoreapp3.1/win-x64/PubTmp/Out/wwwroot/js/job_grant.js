$(function () {
    callAPI(`${API_URL}/Job/view?id=${id}`,null, "GET", function (res) {
            if (res.success) {
                if (res.data != null && res.data != undefined && res.data != "") {
                    $("#chuc_danh").html(res.data.chuc_danh);
                    let html_tinh = '';
                    res.data.tinh_thanh.forEach(item => {
                        html_tinh += `${item.ten_tinh},`;
                    });
                    $('#tinh_thanh').html(html_tinh);
                    $("#so_luong").html(res.data.so_luong);
                    $("#luong_chinh_thuc").html(formatCurency(res.data.luong_chinh_thuc));
                    $("#luong_chinh_thuc_text").html(res.data.luong_chinh_thuc_text);
                    $("#nguoi_lien_he").html(res.data.nguoi_lien_he);
                    if (res.data.cong_ty != null)
                        $(`#cong_ty`).html(res.data.cong_ty.ten_cong_ty);

                    $("#don_gia").html(res.data.don_gia);

                    if (res.data.co_hop_dong) {
                        $("#co_hop_dong").prop('checked', true);
                    }

                    $("#thoi_han_bao_hanh").html(res.data.thoi_han_bao_hanh);
                    $("#so_lan_doi_toi_da").html(res.data.so_lan_doi_toi_da);
                    
                    
                    $("#ghi_chu").html(res.data.ghi_chu);
                    $("#noi_dung").val(res.data.noi_dung);
                    $("#link_job").html(res.data.link_job);
                    $("#dia_chi").html(res.data.dia_chi);
                    
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
    });
    callAPI(`${API_URL}/UserJob/viewbyjob?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            console.log(res);
            let html = "";
            res.data.forEach(item => {
                $(`[name='user_shared'][value='${item.id_user}']`).prop("checked", true);
                //<li class="list-group-item">Chức danh: <span class="font-weight-bold">Kế toán nội bộ</span></li>
                html +=`<li class="list-group-item">`;
                html += `${item.id_user} ${epochToTime(item.ngay_nhan_job)}`;
                html += `</li>`;
            });
            $("#ul_user_job").append(html);
        }
    });
});


function onGrant() {
    let users = [];
    $("[name='user_shared']:checked").each(function () {
        users.push($(this).val());
    });
    var obj = {
        'users': users, 'id': id
    };
    callAPI(`${API_URL}/userjob/grant`, obj, "POST", function (res) {
        console.log(res);
        if (res.success) {
            $.notify(res.msg, "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}