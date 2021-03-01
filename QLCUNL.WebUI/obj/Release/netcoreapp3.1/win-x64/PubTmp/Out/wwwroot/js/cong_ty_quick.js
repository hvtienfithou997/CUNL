function createNewRow(event) {
    event.preventDefault();
    var a = $('<input>', { text: "", class: "form-control card", value: "#Không rõ", type: "text", name: "ten_cong_ty", placeholder: "Nhập tên công ty" });
    var b = $('<input>', { text: "", class: "form-control card", type: "text", name: "chuc_vu", placeholder: "Người liên hệ / Chức danh" });
    var c = $('<input>', { text: "", class: "form-control card", type: "text", name: "email", placeholder: "Email" });
    var d = $('<input>', { text: "", class: "form-control card", type: "text", name: "sdt", placeholder: "SĐT" });
    var e = $('<input>', { text: "", class: "form-control card", type: "text", name: "zalo", placeholder: "Zalo" });
    var f = $('<input>', { text: "", class: "form-control card", type: "text", name: "skype", placeholder: "Skype" });
    var g = $('<input>', { text: "", class: "form-control card", type: "text", name: "facebook", placeholder: "Facebook" });
    var h = $('<span>', { text: " X", title: "Xóa", class: "text-center text-danger clear-field" });
    var k = $('<span>', { text: "@Html.Raw(ViewBag.thuoc_tinh_checkbox)", class: "check-box-inline" })
    var i = $('<br><div class="scrolling-wrapper">', {}).append(a).append(b).append(c).append(d).append(e).append(f).append(g).append(h).append(k);

    $('#company-contact').append(i);

    $(".clear-field").click(function () {
        $(this).parent().remove();
    });
}

$(document).ready(function () {
    showThuocTinh();
});

function showThuocTinh() {
    callAPI(`${API_URL}/thuoctinh/loai?id=0&type=-1`, null, "GET", function (res) {
        if (res.success) {
            //console.log(res);
            let thuoc_tinh = '';
            //thuoc_tinh = `<input class="d-none" id="id_obj" value="${id}">`;
            var result = [];
            res.data.forEach(function (hash) {
                return function (a) {
                    if (!hash[a.nhom]) {
                        hash[a.nhom] = { nhom: a.nhom, activities: [] };
                        result.push(hash[a.nhom]);
                    }
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri, type: a.type });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom == 0) {
                    thuoc_tinh += `<b>Nhóm ${item.nhom}</b>`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau_${item.nhom}_${child.type}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                } else {
                    thuoc_tinh += `<b>Nhóm ${item.nhom}</b>`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_danh_dau_${item.nhom}_${child.type}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                }
            });

            $('.check-box-inline').html(thuoc_tinh);
            //$('#thuoc_tinh_du_lieu').append(`<a target="_blank" href="/thuoctinh/add?loai=0"><i class="icon-add"></i>Thêm thuộc tính</a>`);
            //if (res.value != null) {
            //    for (var i = 0; i < res.value.length; i++) {
            //        $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
            //    }
            //}
        }
    });
}

function saveCompany() {
    let list_thong_tin = [];
    let thuoc_tinh = [], thuoc_tinh_rieng = [];
    $('#company-contact > div').each(function () {
        let ten_cong_ty = $("input[name='ten_cong_ty']", this).val();
        let chuc_vu = $("input[name='chuc_vu']", this).val();
        let email = $("input[name='email']", this).val();
        let sdt = $("input[name='sdt']", this).val();
        let zalo = $("input[name='zalo']", this).val();
        let skype = $("input[name='skype']", this).val();
        let facebook = $("input[name='facebook']", this).val();

        //let thuoc_tinh_chung = $("input[name^='thuoc_tinh_danh_dau']:checked", this).val();
        if (chuc_vu != null || chuc_vu !== "") {
            var lien_he = [];            
            lien_he.push({ "chuc_vu": chuc_vu, "email": email, "sdt": sdt, "zalo": zalo, "skype": skype, "facebook": facebook });
            list_thong_tin.push({
                "ten_cong_ty": ten_cong_ty, "lien_he": lien_he
            });
        }
        
        //list_thong_tin.push(
        //    object
        //);
    });
    console.log(list_thong_tin);
    $("#company-contact > div > span > ul > li > input[name^='thuoc_tinh_danh_dau']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0') {
                thuoc_tinh.push(parseInt($(this).val()));
            } else {
                thuoc_tinh_rieng.push(parseInt($(this).val()));
            }
        } catch{

        }
    });

    console.log(thuoc_tinh);
    console.log(thuoc_tinh_rieng);

    callAPI(`${API_URL}/CongTy/themlienhe`, list_thong_tin, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm công ty thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}