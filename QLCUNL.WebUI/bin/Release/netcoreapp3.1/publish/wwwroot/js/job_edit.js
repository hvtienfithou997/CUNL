var listct = [];
$(function () {
    $('form input').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            onUpdate(event);
            return false;
        }
    });
    $('#btn_upload').click(function () {
        var fd = new FormData();
        var files = $('#file_upload')[0].files[0];
        fd.append('file', files);
        $.ajax({
            url: '/job/uploadjd',
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.success) {
                    $("#link_job_upload").val(`${document.location.origin}/${res.file_path}`);
                } else {
                    $.notify(`${res.msg}`, "error")
                }
            }
        });
    });
    $('#validate-edit-job').validate({
        errorClass: 'errors',
        rules: {
            chuc_danh: {
                required: true
            },
            tinh_thanh: {
                required: true
            },
            so_luong: {
                required: true
            },
            luong_chinh_thuc: {
                required: true
            },
            cong_ty: {
                required: true
            }
        },
        messages: {
            chuc_danh: {
                required: "Bạn phải nhập chức danh"
            },
            tinh_thanh: {
                required: "Bạn phải chọn một tỉnh"
            },
            so_luong: {
                required: 'Bạn phải điền số lượng cần tuyển'
            },
            luong_chinh_thuc: {
                required: 'Bạn cần nhập mức lương'
            },
            cong_ty: {
                required: "Bạn phải chọn một công ty"
            }
        }
    });

    $('input:radio[name="rad_don_gia"]').change(function () {
        if ($(this).val() == 'theoluong') {
            $('#percent_salary').removeClass('d-none');
            $('#don_gia').addClass('d-none');
        } else {
            $('#percent_salary').addClass('d-none');
            $('#don_gia').removeClass('d-none');
        }
    });
});

$(function () {
    callAPI(`${API_URL}/Job/view?id=${id}`,
        null,
        "GET",
        function (res) {
            if (res.success) {
                console.log(res)
                if (res.data != null && res.data != undefined && res.data != "") {
                    $("#chuc_danh").val(res.data.chuc_danh);
                    $("#auto_id").val(res.data.auto_id);
                    $("#tinh_thanh").val(res.data.tinh_thanh.ten_tinh);

                    if (res.data.tinh_thanh != null) {
                        res.data.tinh_thanh.forEach(item => {
                            $(`option[value='${item.id_tinh}']`).prop("selected", true);
                        });
                    }
                    $("#so_luong").val(res.data.so_luong);
                    $("#luong_chinh_thuc").val(formatCurency(res.data.luong_chinh_thuc));
                    $("#luong_chinh_thuc_text").val(res.data.luong_chinh_thuc_text);
                    $("#nguoi_lien_he").val(res.data.nguoi_lien_he);

                    $("#don_gia").val(formatCurency(res.data.don_gia));
                    $('#percent_salary').val(res.data.don_gia);

                    $("#thoi_han_bao_hanh").val(res.data.thoi_han_bao_hanh);
                    $("#so_lan_doi_toi_da").val(res.data.so_lan_doi_toi_da);

                    if (res.data.cong_ty != null) {
                        //$('#cong_ty').autoComplete('set', { value: res.data.cong_ty.id_cong_ty, text: res.data.cong_ty.ten_cong_ty });
                        $("#cong_ty").attr("data-default-text", res.data.cong_ty.ten_cong_ty);
                        $("#cong_ty").attr("data-default-value", res.data.cong_ty.id_cong_ty);
                    }
                    $('#cong_ty').autoComplete({
                        resolver: 'custom',
                        minLength: 2,
                        preventEnter: true,
                        formatResult: function (item) {
                            return {
                                value: item.id_cong_ty,
                                text: item.ten_cong_ty
                            };
                        },
                        events: {
                            search: function (qry, callback) {
                                let req = { "term": qry, "page": 1, "page_size": 50 };
                                callAPI(`${API_URL}/congty/all`, req, "POST", function (res) {
                                    callback(res.data);
                                });
                            }
                        }
                    });

                    $("#tien_coc").val(formatCurency(res.data.tien_coc));
                    $("#owner").val(res.data.owner);
                    $("#tien_coc_tra_lai").val(res.data.tien_coc_tra_lai > 1000 ? formatCurency(res.data.tien_coc_tra_lai) : res.data.tien_coc_tra_lai * 100 + "%");
                    $("#ghi_chu").val(res.data.ghi_chu);
                    $("#noi_dung").val(res.data.noi_dung);
                    $("#link_job").val(res.data.link_job);
                    $("#dia_chi").val(res.data.dia_chi);
                    for (var i = 0; i < res.data.thuoc_tinh.length; i++) {
                        let tt = res.data.thuoc_tinh[i];
                        $(`input[name='thuoc_tinh_nhom_${tt.nhom}'][value='${tt.gia_tri}']`).prop('checked', true);
                        $(`input[name^='thuoc_tinh'][value='${tt.gia_tri}']`).prop('checked', true);
                    }
                }
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
});

function onUpdate(event) {
    event.preventDefault();
    if ($('#validate-edit-job').valid()) {
        let chuc_danh = $('#chuc_danh').val();
        let so_luong = $('#so_luong').val() != "" ? $('#so_luong').val() : 0;
        let luong_chinh_thuc = $('#luong_chinh_thuc').val() != "" ? $('#luong_chinh_thuc').val() : 0;
        luong_chinh_thuc = replaceDot(luong_chinh_thuc);
        let luong_chinh_thuc_text = $('#luong_chinh_thuc_text').val();
        let nguoi_lien_he = $('#nguoi_lien_he').val();
        let cong_ty = { "id_cong_ty": $("[name='cong_ty']").val() };
        let rad_value = $("input[name='rad_don_gia']:checked").val();
        let don_gia_theo_luong = false;
        let don_gia;
        if (rad_value == "giacodinh") {
            don_gia = $('#don_gia').val() != "" ? $('#don_gia').val() : 0;
            don_gia = replaceDot(don_gia);
        } else {
            don_gia = $('#percent_salary').val() != "" ? $('#percent_salary').val() : 0;
            don_gia_theo_luong = true;
        }
        let owner = $("#owner").val();
        if (owner != null || owner != "") {
            owner = owner;
        }
        let link_job_upload = $("#link_job_upload").val();
        //let co_hop_dong = $('input[id=co_hop_dong]:checked').length > 0 ? "true" : "false";
        let thoi_han_bao_hanh = $('#thoi_han_bao_hanh').val() != "" ? $('#thoi_han_bao_hanh').val() : 0;
        let so_lan_doi_toi_da = $('#so_lan_doi_toi_da').val() != "" ? $('#so_lan_doi_toi_da').val() : 0;
        //let dat_coc = $('input[id=dat_coc]:checked').length > 0 ? "true" : "false";
        let tien_coc = $('#tien_coc').val() != "" ? $('#tien_coc').val() : 0;
        let tien_coc_tra_lai = $('#tien_coc_tra_lai').val() != "" ? $('#tien_coc_tra_lai').val() : 0;
        if (isNaN(tien_coc_tra_lai) && tien_coc_tra_lai.indexOf('%') > -1) {
            tien_coc_tra_lai = tien_coc_tra_lai.replace('%', '');
            try {
                tien_coc_tra_lai = parseFloat(tien_coc_tra_lai) / 100;
            } catch (e) {
                tien_coc_tra_lai = 0;
            }
        } else {
            tien_coc_tra_lai = replaceDot(tien_coc_tra_lai);
        }
        tien_coc = replaceDot(tien_coc);
        let ghi_chu = $('#ghi_chu').val();
        let thuoc_tinh = [], thuoc_tinh_rieng = [];
        $("input[name^='thuoc_tinh']:checked").each(function (el) {
            try {
                if ($(this).attr('data-type') === '0')
                    thuoc_tinh.push(parseInt($(this).val()));
                else {
                    thuoc_tinh_rieng.push(parseInt($(this).val()));
                }
            } catch{
            }
        });
        let noi_dung = $('#noi_dung').val();
        let link_job = $('#link_job').val();
        let dia_chi = $('#dia_chi').val();
        let tinh_thanh = [];
        console.log($("#tinh_thanh>option:selected"));
        $("#tinh_thanh>option:selected").each(function () {
            tinh_thanh.push({ 'id_tinh': $(this).val(), 'ten_tinh': $(this).text() });
        });
        var obj = {
            "chuc_danh": chuc_danh, "tinh_thanh": tinh_thanh, "so_luong": so_luong, "luong_chinh_thuc": luong_chinh_thuc,
            "luong_chinh_thuc_text": luong_chinh_thuc_text, "nguoi_lien_he": nguoi_lien_he, "cong_ty": cong_ty, "don_gia": don_gia,
            "thoi_han_bao_hanh": thoi_han_bao_hanh, "so_lan_doi_toi_da": so_lan_doi_toi_da, "tien_coc": tien_coc, "tien_coc_tra_lai": tien_coc_tra_lai, "ghi_chu": ghi_chu,
            "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "noi_dung": noi_dung, "link_job": link_job, "dia_chi": dia_chi, "link_job_upload": link_job_upload,
            "don_gia_theo_luong": don_gia_theo_luong, "owner": owner
        };
        callAPI(`${API_URL}/Job/${id}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Sửa thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};

$('#extend-edit-show').click(function () {
    var me = $(this);
    if ($(this).hasClass('show')) {
        me.text("-Thu gọn");
        me.removeClass('show');
    } else {
        me.addClass('show');
        me.text("+Mở rộng");
    }
});
function copyValue() {
    var copyText = document.getElementById("link_job_upload");
    copyText.select();
    copyText.setSelectionRange(0, 99999);
    document.execCommand("copy");

    $.notify(`Đã copy ` + copyText.value, "success");
}