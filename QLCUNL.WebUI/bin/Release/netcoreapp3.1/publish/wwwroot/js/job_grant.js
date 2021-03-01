$(function () {
    callAPI(`${API_URL}/Job/view?id=${id}`,null, "GET", function (res) {
            if (res.success) {
                if (res.data != null && res.data != undefined && res.data != "") {
                    

                    $("#id_job").val(res.data.id_job);
                    $("#chuc_danh").val(res.data.chuc_danh);
                    
                    let html_tinh = '';
                    res.data.tinh_thanh.forEach(item => {
                        html_tinh += `<div class="row form-group">`;
                        html_tinh +=`<div class="col-md-12"><input type="text" class="form-control" readonly value="${item.ten_tinh}"></div>`;
                        html_tinh += `</div>`;
                    });
                    $('#tinh_thanh').html(html_tinh);
                    $("#so_luong").val(res.data.so_luong);
                    $("#luong_chinh_thuc").val(res.data.luong_chinh_thuc);
                    $("#luong_chinh_thuc_text").val(res.data.luong_chinh_thuc_text);
                    $("#nguoi_lien_he").val(res.data.nguoi_lien_he);
                    if (res.data.cong_ty != null)
                        $(`#cong_ty`).val(res.data.cong_ty.ten_cong_ty);

                    $("#don_gia").val(res.data.don_gia);

                    if (res.data.co_hop_dong) {
                        $("#co_hop_dong").prop('checked', true);
                    }

                    $("#thoi_han_bao_hanh").val(res.data.thoi_han_bao_hanh);
                    $("#so_lan_doi_toi_da").val(res.data.so_lan_doi_toi_da);
                    //$("#dat_coc").val(res.data.dat_coc);
                    switch (res.data.dat_coc) {
                        case "true":
                            $("#dat_coc").prop('checked', true);
                            break;
                        case "false":
                            $("#dat_coc").prop('checked', false);
                            break;
                        default:
                            $("#dat_coc").prop('checked', false);
                            break;
                    }
                    $("#tien_coc").val(res.data.tien_coc);
                    $("#ghi_chu").val(res.data.ghi_chu);
                    $("#noi_dung").val(res.data.noi_dung);
                    $("#link_job").val(res.data.link_job);
                    $("#dia_chi").val(res.data.dia_chi);
                    for (var i = 0; i < res.data.thuoc_tinh.length; i++) {
                        $(`input[name^='thuoc_tinh'][value='${res.data.thuoc_tinh[i].gia_tri}']`).prop('checked', true);
                    }
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
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
        if (res.success) {
            $.notify(res.msg, "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}