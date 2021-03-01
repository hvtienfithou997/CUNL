$(function () {
    var form = $('#validate-congty');
    form.validate({
        errorClass: 'errors',
        rules: {
            ten_cong_ty: {
                required: true
            },
            dien_thoai: {
                required: true
            },
            dia_chi: {
                required: true
            }
        },
        messages: {
            ten_cong_ty: {
                required: "Bạn phải nhập tên công ty"
            },
            dien_thoai: {
                required: "Bạn phải nhập số điện thoại"
            },
            dia_chi: {
                required: 'Bạn phải nhập địa chỉ'
            }
        }
    });

    callAPI(`${API_URL}/congty/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#ten_cong_ty").val(res.data.ten_cong_ty);
                $("#so_dkkd").val(res.data.so_dkkd);
                $("#dien_thoai").val(res.data.dien_thoai);
                $("#dia_chi").val(res.data.dia_chi);
                $("#website").val(res.data.website);
                $("#ghi_chu").val(res.data.ghi_chu);

                for (var i = 0; i < res.data.thuoc_tinh.length; i++) {
                    let tt = res.data.thuoc_tinh[i];
                    $(`input[name='thuoc_tinh_nhom_${tt.nhom}'][value='${tt.gia_tri}']`).prop('checked', true);
                    $(`input[name^='thuoc_tinh'][value='${tt.gia_tri}']`).prop('checked', true);
                }
                $("#info_tax").val(res.data.info_tax);
                $("#info_gui_hop_dong").val(res.data.info_gui_hop_dong);
                $("#ma_so_thue").val(res.data.ma_so_thue);
                let html_lien_he = ``;
                html_lien_he += `<div class="div-element">Chức vụ</div>`;
                html_lien_he += `<div class="div-element">Email</div>`;
                html_lien_he += `<div class="div-element">SDT</div>`;
                html_lien_he += `<div class="div-element">Zalo</div>`;
                html_lien_he += `<div class="div-element">Skype</div>`;
                html_lien_he += `<div class="div-element">Facebook</div>`;
                html_lien_he += `</div>`;

                let count_lien_he = 0;
                res.data.lien_he.forEach(item => {
                    count_lien_he++;
                    html_lien_he += `<div class="form-inline" name="lien_he" data-id="${count_lien_he}">`;
                    html_lien_he += `<input class="form-control element" name="chuc_vu" value="${item.chuc_vu}">`;
                    html_lien_he += `<input class="form-control element" name="email" value="${item.email}">`;
                    html_lien_he += `<input class="form-control element" name="sdt" value="${item.sdt}">`;
                    html_lien_he += `<input class="form-control element" name="zalo" value="${item.zalo}">`;
                    html_lien_he += `<input class="form-control element" name="skype" value="${item.skype}">`;
                    html_lien_he += `<input class="form-control element" name="facebook" value="${item.facebook}">`;

                    html_lien_he += `<span class=" text-danger clear-field" title="Xóa dòng này">X</span>`;
                    html_lien_he += `</div>`;
                });
                $("#div_lien_he").html(html_lien_he);
                $('.clear-field').click(function () {
                    $(this).parent().remove();
                });

                $("#tai_khoan_ngan_hang").val(res.data.tai_khoan_ngan_hang);
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
})

function onUpdate(event) {
    event.preventDefault();
    if ($('#validate-congty').valid()) {
        let ten_cong_ty = $('#ten_cong_ty').val();
        let so_dkkd = $('#so_dkkd').val();
        let dien_thoai = $('#dien_thoai').val();
        let dia_chi = $('#dia_chi').val();
        let website = $('#website').val();
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
        let info_tax = $('#info_tax').val();
        let info_gui_hop_dong = $('#info_gui_hop_dong').val();
        let ma_so_thue = $('#ma_so_thue').val();
        let lien_he = [];

        $("[name='lien_he']").each(function () {
            let chuc_vu = $("[name='chuc_vu']", this).val();
            let email = $("[name='email']", this).val();
            let sdt = $("[name='sdt']", this).val();
            let zalo = $("[name='zalo']", this).val();
            let skype = $("[name='skype']", this).val();
            let facebook = $("[name='facebook']", this).val();
            lien_he.push(
                { 'chuc_vu': chuc_vu, 'email': email, 'sdt': sdt, 'zalo': zalo, 'skype': skype, 'facebook': facebook }
            );
        });

        let tai_khoan_ngan_hang = $('#tai_khoan_ngan_hang').val();

        var obj = {
            "id_cong_ty": id, "ten_cong_ty": ten_cong_ty, "so_dkkd": so_dkkd, "dien_thoai": dien_thoai, "dia_chi": dia_chi, "website": website,
            "ghi_chu": ghi_chu, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "info_tax": info_tax, "info_gui_hop_dong": info_gui_hop_dong, "ma_so_thue": ma_so_thue,
            "lien_he": lien_he, "tai_khoan_ngan_hang": tai_khoan_ngan_hang, "nguoi_sua": user
        };

        callAPI(`${API_URL}/CongTy/${id}`, obj, "PUT", function (res) {
            if (res.success) {
                $.notify("Sửa thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};

function addContact(event) {
    event.preventDefault();

    var a = $('<input>', { text: "", class: "form-control element", type: "text", name: "chuc_vu", placeholder: "Chức vụ" });
    var b = $('<input>', { text: "", class: "form-control element", type: "text", name: "email", placeholder: "Email" });
    var c = $('<input>', { text: "", class: "form-control element", type: "text", name: "sdt", placeholder: "SĐT" });
    var d = $('<input>', { text: "", class: "form-control element", type: "text", name: "zalo", placeholder: "Zalo" });
    var e = $('<input>', { text: "", class: "form-control element", type: "text", name: "skype", placeholder: "Skype" });
    var f = $('<input>', { text: "", class: "form-control element", type: "text", name: "facebook", placeholder: "Facebook" });
    var h = $('<span>', { text: " X", title: "Xóa", class: "text-center text-danger clear-field" });
    var g = $('<div class="form-inline" name="lien_he">', {}).append(a).append(b).append(c).append(d).append(e).append(f).append(h);
    $("#div_lien_he").append(g);
    $('.clear-field').click(function () {
        $(this).parent().remove();
    });
}
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