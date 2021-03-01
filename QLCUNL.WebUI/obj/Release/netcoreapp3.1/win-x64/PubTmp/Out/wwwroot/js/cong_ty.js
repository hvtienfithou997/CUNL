$(document).ready(function () {
    enableDisableButton();
    var choose = sessionStorage.getItem('myEmailChoose');

    if (choose != null) {
        var getValue = choose.replace(/,/g, '\n');
        $("#nguoi_nhan").val(getValue);
    }

    //$("#ten_nguoi_nhan").val(getValue);
    $(".field-thuoc-tinh").append("<span type='button' class='float-right btn-danger remove-elem'> Bỏ chọn</span>");
    $(".remove-elem").click(function () {
        $(this).siblings('ul').find('input').prop('checked', false);
    });
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
});
function addContact(event) {
    event.preventDefault();
    var a = $('<input>', { text: "", class: "form-control element", type: "text", name: "chuc_vu", placeholder: "Nguời liên hệ - Chức vụ" });
    var b = $('<input>', { text: "", class: "form-control element", type: "text", name: "email", placeholder: "Email" });
    var c = $('<input>', { text: "", class: "form-control element", type: "text", name: "sdt", placeholder: "SĐT" });
    var d = $('<input>', { text: "", class: "form-control element", type: "text", name: "zalo", placeholder: "Zalo" });
    var e = $('<input>', { text: "", class: "form-control element", type: "text", name: "skype", placeholder: "Skype" });
    var f = $('<input>', { text: "", class: "form-control element", type: "text", name: "facebook", placeholder: "Facebook" });
    var h = $('<span>', { text: " X", title: "Xóa", class: "text-center text-danger clear-field" });
    var g = $('<span class="form-inline">', {}).append(a).append(b).append(c).append(d).append(e).append(f).append(h);
    $('#contact').append(g);

    $(".clear-field").click(function () {
        $(this).parent().remove();
    });
}

function onSubmit(event) {
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
            } catch (e) {
            }
        });

        let info_tax = $('#info_tax').val();
        let info_gui_hop_dong = $('#info_gui_hop_dong').val();
        let ma_so_thue = $('#ma_so_thue').val();

        let lien_he = [];
        $('#contact > span').each(function () {
            let chuc_vu = $("input[name='chuc_vu']", this).val();
            let email = $("input[name='email']", this).val();
            let sdt = $("input[name='sdt']", this).val();
            let zalo = $("input[name='zalo']", this).val();
            let skype = $("input[name='skype']", this).val();
            let facebook = $("input[name='facebook']", this).val();
            lien_he.push(
                { 'chuc_vu': chuc_vu, 'email': email, 'sdt': sdt, 'zalo': zalo, 'skype': skype, 'facebook': facebook }
            );
        });

        let tai_khoan_ngan_hang = $('#tai_khoan_ngan_hang').val();
        var obj = {
            "ten_cong_ty": ten_cong_ty, "so_dkkd": so_dkkd, "dien_thoai": dien_thoai, "dia_chi": dia_chi, "website": website,
            "ghi_chu": ghi_chu, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "info_tax": info_tax, "info_gui_hop_dong": info_gui_hop_dong, "ma_so_thue": ma_so_thue,
            "lien_he": lien_he, "tai_khoan_ngan_hang": tai_khoan_ngan_hang, "nguoi_tao": user, "nguoi_sua": user
        };

        callAPI(`${API_URL}/CongTy/add`, obj, "POST", function (res) {
            if (res.success) {
                $.notify("Thêm thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
    }
};

function bindDetail(id) {
    callAPI(`${API_URL}/CongTy/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                let detail_html = '';
                detail_html += `<li>AutoId: <span class="auto_id" onclick="copyText(this)"><code>${res.data.auto_id}</code></span></li>`;
                detail_html += `<li>Tên công ty:<span class="font-weight-bold">${res.data.ten_cong_ty}</span></li>`;
                detail_html += `<li>Số ĐKKD: <span class="font-weight-bold">${res.data.so_dkkd}</span></li>`;
                detail_html += `<li>Điện thoại: <span class="font-weight-bold">${res.data.dien_thoai}</span></li>`;
                detail_html += `<li>Địa chỉ: <span class="font-weight-bold">${res.data.dia_chi}</span></li>`;
                detail_html += `<li>Website: <span class="font-weight-bold">${res.data.website}</span></li>`;
                detail_html += `<li>Ghi chú: <span class="font-weight-bold">${res.data.ghi_chu}</span></li>`;
                let html_thuoc_tinh = "<ul>";
                if (res.data.thuoc_tinh != null) {
                    res.data.thuoc_tinh.forEach(tt => {
                        html_thuoc_tinh += `<li><span class="font-weight-bold">${tt.ten}</span></li>`;
                    });
                }
                html_thuoc_tinh += `</ul>`;
                detail_html += `<li>Thuộc tính: <span class="font-weight-bold">${html_thuoc_tinh}</span></li>`;
                detail_html += `<li>Info tax: <span class="font-weight-bold">${res.data.info_tax}</span></li>`;
                detail_html += `<li>Info gửi hợp đồng: <span class="font-weight-bold">${res.data.info_gui_hop_dong}</span></li>`;
                detail_html += `<li>Mã số thuế: <span class="font-weight-bold">${res.data.ma_so_thue}</span></li>`;
                detail_html += `<li>Liên hệ: </li>`;
                detail_html += `<li>`;

                let html_lien_he = '<div id="box-lien-he" class="row">';

                if (res.data.lien_he != null) {
                    res.data.lien_he.forEach(item => {
                        html_lien_he += `<ul class="col-md-4">`;
                        html_lien_he += `<li>Người liên hệ / Chức vụ: <span class="font-weight-bold">${item.chuc_vu}</span></li>`;
                        html_lien_he += `<li>Email: <span class="font-weight-bold">${item.email}</span></li>`;
                        html_lien_he += `<li>Số điện thoại: <span class="font-weight-bold">${item.sdt}</span></li>`;
                        html_lien_he += `<li>Zalo: <span class="font-weight-bold">${item.zalo}</span></li>`;
                        html_lien_he += `<li>Skype: <span class="font-weight-bold">${item.skype}</span></li>`;
                        html_lien_he += `<li>Facebook: <span class="font-weight-bold">${item.facebook}</span></li>`;
                        html_lien_he += '</ul>';
                    });
                }

                html_lien_he += "</div>";
                detail_html += html_lien_he;
                detail_html += "</li>";
                detail_html += `<li>Tài khoản ngân hàng: <span class="font-weight-bold">${res.data.tai_khoan_ngan_hang}</span></li>`;

                detail_html += `<li>Ngày tạo: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_tao)}</span></li>`;
                detail_html += `<li>Ngày sửa: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_sua)}</span></li>`;
                detail_html += `<li>Người tạo: <span class="font-weight-bold">${res.data.nguoi_tao}</span></li>`;
                detail_html += `<li>Người sửa: <span class="font-weight-bold">${res.data.nguoi_sua}</span></li>`;
                $("#congty_detail").html(detail_html);
                $('#congty_detail').children().addClass("list-group-item");
                $('#congty_detail li').children().addClass("list-group list-group-flush").children().addClass("list-group-item");
                $("#box-lien-he").removeClass("list-group list-group-flush");
                $("#box-lien-he").children().css("border", "none");
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function search(page) {
    let term = encodeURI($(`[name='term']`).val());
    if (typeof page === 'undefined') {
        page = 1;
    }
    let thuoc_tinh = []; let thuoc_tinh_rieng = [];
    $("input[name^='thuoc_tinh']:checked", "#div_thuoc_tinh").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng.push(parseInt($(this).val()));
        } catch (e) {
        }
    });
    let url_new = `term=${term}&thuoc_tinh=${thuoc_tinh.join(',')}&thuoc_tinh_rieng=${thuoc_tinh_rieng.join(',')}&page=${page}`;
    window.history.pushState(window.location.href, "Danh sách công ty", `?${url_new}`);
    callAPI(`${API_URL}/CongTy/search?term=${term}&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        $("#div_data").html(DIV_LOADING);
        if (res.success) {
            $("#tutorial").addClass("d-none");
            $(".data-table").removeClass("d-none");
            $("#div_data").empty();
            $(".totalRecs").html("Tổng số công ty: " + res.total);
            if (res.data != null && res.data.length > 0) {
                let html_data = "";

                res.data.forEach(item => {
                    let html_lien_he = "";

                    html_data += `<tr>`;
                    html_data += `<td><span class="auto_id" onclick="copyText(this)"><code>${item.auto_id}</code><br></span>${item.ten_cong_ty}<br>`;
                    if (item.lien_he !== null && item.lien_he.length > 0) {
                        item.lien_he.forEach(item => {
                            if (item.email !== "") {
                                html_data += `<hr><kbd class="kbd1"><input type='checkbox' onclick="mailChecked()" value='${item.email}' name='choose_email'/> ${item.email}</kbd>`;
                            }
                        });
                    }

                    html_data += "</td>";
                    html_data += `<td>${item.so_dkkd == null ? "" : item.so_dkkd}</td>`;
                    html_data += `<td>${item.ma_so_thue == null ? "" : item.ma_so_thue}</td>`;
                    html_data += `<td>${item.dia_chi == null ? "" : item.dia_chi}</td>`;
                    html_data += `<td>${item.dien_thoai == null ? "" : item.dien_thoai}<br>${item.website == null ? "" : item.website}</td>`;
                    html_data += `<td><a class="btn btn-warning btn-big btn-w1" href="edit?id=${item.id_cong_ty
                        }">Sửa</a>&nbsp;<a class="btn btn-success btn-big btn-w1" href="detail/${item.id_cong_ty
                        }">Xem</a>&nbsp;<a class="btn btn-info btn-big" href="share?id=${item.id_cong_ty
                        }">Chia sẻ</a><button class="btn btn-dark modall btn-big btn-w2" id="${item.id_cong_ty}"
                            data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>
                        <button class="btn btn-danger btn-s-small" id="${item.id_cong_ty}"
                             onclick="deleteRec(this.id)">Xóa</button></td>`;
                    html_data += `</tr>`;
                });

                $("#div_data").html(html_data);
                //$(".lien_he").html(html_lien_he);
            }

            /*START_PAGING*/
            paging(res.total, 'search', page);
            /*END_PAGING*/
        } else {
            $("#div_data").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });

    callAPI(`${API_URL}/congty/thongkethuoctinh?term=${term}&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        if (res.success) {
            $(".thong_ke").remove();
            res.data.forEach(item => {
                let ele = $(`input[name^='thuoc_tinh'][value='${item.k}']`);
                if (ele) {
                    ele.next().append(` <code class='thong_ke'>(${item.v})</code>`);
                }
            });
        }
    });
}
function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/congty/delete?id=${id}`,
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

function onShowThuocTinh(id) {
    callAPI(`${API_URL}/thuoctinh/canhan?loai=0&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let thuoc_tinh = '';
            thuoc_tinh = `<input class="d-none" id="id_obj" value="${id}">`;
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
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau_${item.nhom}_${child.type}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_danh_dau_${item.nhom}_${child.type}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
                }
            });

            $('#thuoc_tinh_du_lieu').html(thuoc_tinh);
            $('#thuoc_tinh_du_lieu').append(`<a target="_blank" href="/thuoctinh/add?loai=0"><i class="icon-add"></i>Thêm thuộc tính</a>`);
            if (res.value != null) {
                for (var i = 0; i < res.value.length; i++) {
                    $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
                }
            }
        }
    });
}

function onCreateThuocTinh(loai_obj) {
    let id = $('#id_obj').val();
    let thuoc_tinh = [], thuoc_tinh_rieng = [];
    $("input[name^='thuoc_tinh_danh_dau']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0') {
                thuoc_tinh.push(parseInt($(this).val()));
            } else {
                thuoc_tinh_rieng.push(parseInt($(this).val()));
            }
        } catch (e) {
        }
    });
    let obj = { "loai_obj": loai_obj, "id_obj": id, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "nguoi_tao": user, "nguoi_sua": user }

    callAPI(`${API_URL}/thuoctinhdulieu/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thành công", "success");
            $('#myModal').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
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
var list_check_send_mail = [];
function sendEmail() {
    mailChecked();
    sessionStorage.setItem('myEmailChoose', list_check_send_mail);
}
function toObject(arr) {
    var rv = {};
    for (var i = 0; i < arr.length; ++i)
        if (arr[i] !== undefined) rv[i] = arr[i];
    return rv;
}

function onSendMail() {
    $("#btn_gui_mail").attr("disabled", true);
    let mail_info = {};
    let info = [];
    let email_nguoi_gui = $("#email_nguoi_gui").val();
    let ten_nguoi_gui = $("#ten_nguoi_gui").val();
    let email_nguoi_nhan = $("#nguoi_nhan").val();
    let ten_nguoi_nhan = $("#ten_nguoi_nhan").val();
    let tieu_de = $("#tieu_de").val();
    //let noi_dung = $("#noi_dung").val();
    let noi_dung = CKEDITOR.instances.noi_dung.getData();
    
    let list = [];
    let list_attact = $("#receive_link_mail").val();
    let list_att;
    if (list_attact !== "") {
        list_att = list_attact.split(",");
    }
    let thong_tin = [];
    let obj = {};

    $('#add_key > div').each(function () {
        let key = $("input[name^='key-']", this).val();
        let text = $("textarea[name^='text-']", this).val();

        let str = text.split('\n');
        obj = {
            key: key,
            "body": str
        }
        thong_tin.push(obj);
    });

    let thong_tin_mail = thong_tin[0].body;
    for (var i = 0; i < thong_tin_mail.length; i++) {
        let a = {};
        a["Email"] = thong_tin_mail[i];
        for (var j = 1; j < thong_tin.length; j++) {
            a[thong_tin[j].key] = thong_tin[j].body[i];
        }
        info.push(a);
    }

    mail_info = {
        "AppId": "VietHunter",
        "EmailGui": email_nguoi_gui,
        "TenGui": ten_nguoi_gui,
        "TieuDeGui": tieu_de,
        "NoiDung": noi_dung,
        "Info": info,
        "NguoiTao": user,
        "ListAttachs": list_att
    };
    
    var data = new FormData();
    data.append("token", token);
    data.append("obj", JSON.stringify(mail_info));
    $.ajax({
        url: "http://123.30.238.32:12019/ajax/AddMail.ashx",
        type: 'post',
        data: data,
        dataType: 'json',
        contentType: false,
        processData: false,
        xhrFields: {
            withCredentials: false
        },
        success: function (response) {
            if (response.Status) {
                checkStatusSend(response.Data);
                $("#btn_gui_mail").attr("disabled", false);
            } else {
                $("#btn_gui_mail").attr("disabled", false);
                alert(response.Message);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
    sessionStorage.clear();
}

//1 mới 2 chờ 3 ok 4 lỗi 5 hủy

function checkStatusSend(res) {
    let arr = [];
    if (res != null) {
        res.forEach(item => {
            arr.push(item.Id);
        });
        var data = new FormData();
        data.append("token", token);
        data.append("obj", JSON.stringify(arr));
        $.ajax({
            url: "http://123.30.238.32:12019/ajax/CheckStatus.ashx",
            type: 'post',
            data: data,
            dataType: 'json',
            contentType: false,
            processData: false,
            xhrFields: {
                withCredentials: false
            },
            success: function (response) {
                if (response.Status) {
                    logSendMail(response.Data);
                } else {
                    console.log(response.Message);
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}

function logSendMail(res) {
    if (res != null) {
        callAPI(`${API_URL}/congty/logsendmail`, res, "POST", function (resp) {
            if (resp.success) {
                $.notify("Thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra ${resp.msg}`, "error");
            }
        });
    }
}

function enableDisableButton() {
    if ($("input[type='checkbox']:checked", "#div_data").length > 0) {
        $("#btn_send_mail").removeClass("disabled");
    } else {
        $("#btn_send_mail").addClass("disabled");
    }
}

function mailChecked() {
    list_check_send_mail = [];
    $("input[name='choose_email']:checked").each(function (el) {
        try {
            list_check_send_mail.push($(this).val());
        } catch (e) {
        }
    });
    enableDisableButton();
}

function moreInfo() {
    var num = $("#add_key").find("div.form_noi_dung").length;
    var a = $(`<input type='text' class='form-control c-input' data-key="key-${num + 1}" id="key-${num + 1}" name="key-${num + 1}" placeholder='Key'/>`);
    var b = $(`<textarea type='text' class='form-control w-textarea' rows='5' id="text-${num + 1}" name="text-${num + 1}" data-text="text-${num + 1}" placeholder='Giá trị'></textarea>`);
    var c = $('<div>', { text: "", class: "form-group form_noi_dung" });
    c.append(a).append(b);
    $("div#add_key").append(c);
}

function UploadFile() {
    var data = new FormData();

    $.each($("#file_upload")[0].files, function (i, file) {
        data.append('file', file);
    });
    $.ajax({
        url: "http://123.30.238.32:12019/ajax/UploadFileAttach.ashx",
        type: 'post',
        data: data,
        dataType: 'json',
        contentType: false,
        processData: false,
        xhrFields: {
            withCredentials: false
        },
        success: function (response) {
            $.notify("Upload thành công, hãy bấm gửi!", "success");
            let arr_path = [];

            response.Result.forEach(fc => {
                if (fc.Sts) {
                    arr_path.push(fc.Path);
                }

                
            });
            $("#receive_link_mail").val(arr_path);
        },
        error: function (error) {
            console.log(error);
        }
    });
}