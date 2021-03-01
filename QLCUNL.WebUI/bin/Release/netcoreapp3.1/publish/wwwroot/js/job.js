$(document).ready(function () {
    $('form input').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            onSubmit(event);
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
    $('input:radio[name="rad_don_gia"]').change(function () {
        if ($(this).val() == 'theoluong') {
            $('#percent_salary').removeClass('d-none');
            $('#don_gia').addClass('d-none');
        } else {
            $('#percent_salary').addClass('d-none');
            $('#don_gia').removeClass('d-none');
        }
    });
    $('#cong_ty').autoComplete({
        resolver: 'custom',
        minLength: 2, preventEnter: true,
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
    $('#cong_ty').on('autocomplete.select', function (evt, item) {
        id_cong_ty = item.id_cong_ty;
        search(1);
    });

    $('#cong_ty_add').autoComplete({
        resolver: 'custom',
        minLength: 2, preventEnter: true,
        formatResult: function (item) {
            return {
                value: item.id_cong_ty,
                text: item.ten_cong_ty
            };
        },
        events: {
            search: function (qry, callback) {
                let obj = { "term": qry, "page_index": 1, "page_size": 1000 };
                callAPI(`${API_URL}/congty/all`, obj, "POST", function (res) {
                    callback(res.data);
                });
            }
        }
    });
    $('#cong_ty_add').on('autocomplete.select', function (evt, item) {
        evt.preventDefault();
        $('#edit-company').html(`<a href="/congty/edit?id=${item.id_cong_ty}" target="_blank" class="btn btn-size btn-warning">Sửa</a>`);
        id_cong_ty = item.id_cong_ty;
    });
    $("#btn_add_cong_ty").click(function () {
        $('.add-cong-ty').removeClass('d-none');
    })
});

var listct = [];

var listjobchecked = [];

function onSubmit(event) {
    var fo = $('#validate-add-job').bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            chuc_danh: {
                validators: {
                    notEmpty: {
                        message: "Nhập chức danh"
                    }
                }
            },
            luong_chinh_thuc: {
                validators: {
                    notEmpty: {
                        message: 'Nhập mức lương'
                    }
                }
            },
            tinh_thanh: {
                validators: {
                    notEmpty: {
                        message: 'Chọn một tỉnh'
                    }
                }
            },
            cong_ty_add_text: {
                validators: {
                    notEmpty: {
                        message: `Bạn phải chọn một công ty hoặc bấm "Tạo mới"`
                    }
                }
            }
        },
        onSuccess: function (e) {
            e.preventDefault();
            let chuc_danh = $('#chuc_danh').val();
            let tinh_thanh_id = $('#tinh_thanh > option:selected').val();
            let tinh_thanh_text = $('#tinh_thanh > option:selected').text();
            let so_luong = $('#so_luong').val() != "" ? $('#so_luong').val() : 0;
            let luong_chinh_thuc = $('#luong_chinh_thuc').val() != "" ? $('#luong_chinh_thuc').val() : 0;
            luong_chinh_thuc = replaceDot(luong_chinh_thuc);
            let luong_chinh_thuc_text = $('#luong_chinh_thuc_text').val();
            let nguoi_lien_he = $('#nguoi_lien_he').val();
            let cong_ty = { "id_cong_ty": $("[name='cong_ty_add']").val() };
            let rad_value = $("input[name='rad_don_gia']:checked").val();
            let don_gia; let don_gia_theo_luong = false;
            let link_job_upload = $("#link_job_upload").val();
            if (rad_value == "giacodinh") {
                don_gia = $('#don_gia').val() != "" ? $('#don_gia').val() : 0;
                don_gia = replaceDot(don_gia);
            } else {
                don_gia = $('#percent_salary').val() != "" ? $('#percent_salary').val() : 0;
                don_gia_theo_luong = true;
            }
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
            $("#tinh_thanh>option:selected").each(function () {
                tinh_thanh.push({ 'id_tinh': $(this).val(), 'ten_tinh': $(this).text() });
            });
            var obj = {
                "chuc_danh": chuc_danh, "tinh_thanh": tinh_thanh, "so_luong": so_luong, "luong_chinh_thuc": luong_chinh_thuc,
                "luong_chinh_thuc_text": luong_chinh_thuc_text, "nguoi_lien_he": nguoi_lien_he, "cong_ty": cong_ty, "don_gia": don_gia,
                "thoi_han_bao_hanh": thoi_han_bao_hanh, "so_lan_doi_toi_da": so_lan_doi_toi_da, "tien_coc": tien_coc, "tien_coc_tra_lai": tien_coc_tra_lai, "ghi_chu": ghi_chu,
                "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "noi_dung": noi_dung, "link_job": link_job, "link_job_upload": link_job_upload,
                "dia_chi": dia_chi, "nguoi_tao": user, "nguoi_sua": user, 'don_gia_theo_luong': don_gia_theo_luong, "owner": user
            };

            callAPI(`${API_URL}/Job/add`, obj, "POST", function (res) {
                if (res.success) {
                    $.notify("Thêm thành công", "success");
                    $('#chuc_danh').val("");
                    $('#so_luong').val("1");
                    $('#luong_chinh_thuc').val("");
                    $('#don_gia').val("0");
                    $("#auto_id").val(res.data);
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
            });
            return false;
        }
    });
};

function bindDetail(id) {
    callAPI(`${API_URL}/Job/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#job_detail").empty();

                $("#job_detail").append(`<li>Chức danh: <span class="font-weight-bold">${res.data.chuc_danh}</span></li>`);

                $("#job_detail").append(`<li>Tỉnh thành :</li>`);
                $("#job_detail").append("<ul>");
                res.data.tinh_thanh.forEach(item => {
                    $("#job_detail").append(`<li>-- Tên tỉnh: <span class="font-weight-bold">${item.ten_tinh}</span></li>`);
                });
                $("#job_detail").append("</ul>");
                $("#job_detail").append(`<li>Số lượng: <span class="font-weight-bold">${res.data.so_luong}</span></li>`);
                $("#job_detail").append(`<li>Lương chính thức: <span class="font-weight-bold">${formatCurency(res.data.luong_chinh_thuc)}</span></li>`);
                $("#job_detail").append(`<li>Lương chính thức text: <span class="font-weight-bold">${res.data.luong_chinh_thuc_text}</span></li>`);
                $("#job_detail").append(`<li>Người liên hệ: <span class="font-weight-bold">${res.data.nguoi_lien_he}</span></li>`);
                if (res.data.cong_ty != null)
                    $("#job_detail").append(`<li>Công ty: <span class="font-weight-bold">${res.data.cong_ty.ten_cong_ty}</span></li>`);
                $("#job_detail").append(`<li>Đơn giá: <span class="font-weight-bold">${res.data.don_gia_theo_luong ? res.data.don_gia + " tháng lương" : formatCurency(res.data.don_gia)}</span></li>`);
                $("#job_detail").append(`<li>Có hợp đồng: <span class="font-weight-bold">${res.data.co_hop_dong ? "CÓ" : "KHÔNG"}</span></li>`);
                $("#job_detail").append(`<li>Thời hạn bảo hành: <span class="font-weight-bold">${res.data.thoi_han_bao_hanh}</span></li>`);
                $("#job_detail").append(`<li>Số lần đổi tối đa: <span class="font-weight-bold">${res.data.so_lan_doi_toi_da}</span></li>`);
                $("#job_detail").append(`<li>Đặt cọc: <span class="font-weight-bold">${res.data.dat_coc ? "CÓ" : "KHÔNG"}</span></li>`);
                $("#job_detail").append(`<li>Tiền cọc: <span class="font-weight-bold">${formatCurency(res.data.tien_coc)}</span></li>`);
                if (res.data.tien_coc_tra_lai > 0) {
                    $("#job_detail").append(`<li>Tiền trả lại: <span class="font-weight-bold">${res.data.tien_coc_tra_lai > 1000 ? formatCurency(res.data.tien_coc_tra_lai) : res.data.tien_coc_tra_lai * 100 + "%"}</span></li>`);
                }
                $("#job_detail").append(`<li>Ghi chú: <span class="font-weight-bold">${res.data.ghi_chu}</span></li>`);
                $("#job_detail").append(`<li>Nội dung: <span class="font-weight-bold">${res.data.noi_dung}</span></li>`);
                $("#job_detail").append(`<li>Link job: <span class="font-weight-bold">${res.data.link_job}</span></li>`);
                $("#job_detail").append(`<li>Địa chỉ: <span class="font-weight-bold">${res.data.dia_chi}</span></li>`);
                if (res.data.thuoc_tinh != null) {
                    let thuoc_tinh = [];
                    res.data.thuoc_tinh.forEach(item => {
                        thuoc_tinh.push(`${item.ten}`);
                    });
                    $("#job_detail").append(`<li>Thuộc tính: <span class="font-weight-bold">${thuoc_tinh.join("/")}</span></li>`);
                }
                $("#job_detail").append(`<li>Ngày tạo: <span class="font-weight-bold">${epochToTime(res.data.ngay_tao)}</span></li>`);
                $("#job_detail").append(`<li>Người tạo: <span class="font-weight-bold">${res.data.nguoi_tao}</span></li>`);
                $("#job_detail").append(`<li>Ngày sửa: <span class="font-weight-bold">${epochToTime(res.data.ngay_sua)}</span></li>`);
                $("#job_detail").append(`<li>Người sửa: <span class="font-weight-bold">${res.data.nguoi_sua}</span></li>`);
                $('#job_detail li ').addClass("list-group-item");
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
    $("input[name^='thuoc_tinh']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng.push(parseInt($(this).val()));
        } catch{
        }
    });

    let url_new = `term=${term}&id_cong_ty=${id_cong_ty}&thuoc_tinh=${thuoc_tinh.join(',')}&thuoc_tinh_rieng=${thuoc_tinh_rieng.join(',')}&page=${page}`;

    window.history.pushState(window.location.href, "Danh sách job", `?${url_new}`);

    callAPI(`${API_URL}/Job/search?term=${term}&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&id_cong_ty=${id_cong_ty}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        if (res.success) {
            console.log(res);
            $("#all_job").empty();
            $(".totalRecs").html("Tổng số Job: " + res.total);
            if (res.data != null && res.data.length > 0) {
                let html_data = "";
                res.data.forEach(item => {
                    html_data += `<tr>`;
                    html_data += `<td><input type="checkbox" value="${item.id_job}" name="jobUnchecked" onclick="jobchecked()" id="jobUnchecked"></td>`;
                    html_data += `<td><a href="#" data-toggle="modal" data-link="${item.link_job_upload}" onclick="showPreviewJob(this)" data-target="#preview-job">${item.chuc_danh}<br>${item.auto_id}</a></td>`;
                    html_data += `<td>`;
                    if (item.tinh_thanh != null)
                        item.tinh_thanh.forEach(child => {
                            html_data += ` - ${child.ten_tinh}`;
                        });
                    html_data += `</td>`;
                    html_data += `<td>${item.so_luong}</td>`;
                    html_data += `<td>${formatCurency(item.luong_chinh_thuc)}</td>`;

                    html_data += `<td>${item.nguoi_lien_he}</td>`;
                    if (item.cong_ty != null)
                        html_data += `<td>${item.cong_ty.ten_cong_ty}</td>`;
                    else
                        html_data += `<td></td>`;

                    item.owner = item.owner != null ? item.owner : " ";
                    html_data += `<td>${item.owner}</td>`;
                    html_data += `<td><a class="btn btn-warning btn-big btn-w1" href="edit?id=${item.id_job}">Sửa</a>`;
                    html_data += `&nbsp;<a class="btn btn-success btn-big btn-w1" href="detail/${item.id_job}">Xem</a>`;
                    html_data += `&nbsp;<a class="btn btn-info btn-big" href="share?id=${item.id_job}">Chia sẻ</a>`;
                    html_data += `<a class="btn btn-dark btn-big btn-big" href="userjob?id=${item.id_job}">Gán JOB</a>`;
                    html_data += `<button class="btn btn-secondary modall btn-big" id="${item.id_job}" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>`;
                    html_data += `<button class="btn btn-danger btn-s-small" id="${item.id_job}"
                             onclick="deleteRec(this.id)">Xóa</button></td>`;
                    html_data += `</tr>`;
                });
                $("#all_job").html(html_data);
            }
            paging(res.total, 'search', page);
        } else {
            $("#all_job").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function showPreviewJob(e) {
    var link = $(e).attr('data-link');
    if (link != "") {
        var url_temp = "";
        var ext = link.split('.').pop();
        url_temp = ext == "pdf" ? link : "https://view.officeapps.live.com/op/view.aspx?src=" + link;
        $("#link_job_upload").empty();
        $("#link_job_upload").append(`<iframe id="iframe-cv" src="${url_temp}"></iframe>`);
    } else {
        $("#link_job_upload").empty();
        $("#link_job_upload").append(`<p>Link Job này chưa được Upload!</p>`);
    }
}
function deleteRec(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/job/delete?id=${id}`,
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
    callAPI(`${API_URL}/thuoctinh/canhan?term=&loai=4&id=${id}`, null, "GET", function (res) {
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
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom === 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_danh_dau' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#thuoc_tinh_du_lieu').html(thuoc_tinh);
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name='thuoc_tinh_danh_dau'][value='${res.value[i]}']`).prop('checked', true);
            }
            $('#thuoc_tinh_du_lieu').append(`<a href="/thuoctinh/add"><i class="icon-add"></i>Thêm thuộc tính</a>`);
        }
    });
}
function onCreateThuocTinh(loai_obj) {
    let id = $('#id_obj').val();
    let thuoc_tinh = [];
    $("input[name='thuoc_tinh_danh_dau']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch{
        }
    });

    let obj = { "loai_obj": loai_obj, "id_obj": id, "thuoc_tinh": thuoc_tinh, "nguoi_tao": user, "nguoi_sua": user }

    callAPI(`${API_URL}/thuoctinhdulieu/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thành công", "success");
            $('#myModal').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function jobchecked() {
    listjobchecked = [];

    $("input[name='jobUnchecked']:checked").each(function (el) {
        try {
            listjobchecked.push($(this).val());
        } catch{
        }
    });
    enableDisableButton();
}

function enableDisableButton() {
    if ($("input[type='checkbox']:checked", "#all_job").length > 0) {
        $("#btn-gan").removeClass("disabled");
    } else {
        $("#btn-gan").addClass("disabled");
    }
}
function onCheckAll() {
    listjobchecked = [];
    $("input[type='checkbox']", "#all_job").prop("checked", $("#check_all").is(":checked"));
    $("input[name='jobUnchecked']:checked").each(function (el) {
        try {
            listjobchecked.push($(this).val());
        } catch{
        }
    });
    enableDisableButton();
}

function onCheckAllNhanVien() {
    listnhanvienchecked = [];
    $("input[type='checkbox']", "#div_data1").prop("checked", $("#check_all_nv").is(":checked"));
    $("input[name='nhanvienUnchecked']:checked").each(function (el) {
        try {
            listnhanvienchecked.push($(this).val());
        } catch{
        }
    });
}

function createCompany(event) {
    event.preventDefault();
    $('.add-cong-ty').removeClass('d-none');
}
function saveCompany(event) {
    event.preventDefault();

    let ten_cong_ty = $('#new-cong-ty').val();
    var obj = { "ten_cong_ty": ten_cong_ty }
    callAPI(`${API_URL}/CongTy/indexcongty`, obj, "POST", function (res) {
        if (res.success) {
            console.log(res);
            $.notify("Thêm thành công", "success");
            $('.add-cong-ty').addClass('d-none');
            $("#cong_ty").prepend($("<option></option>")
                .text(ten_cong_ty)
                .val(res.data));
            $("#cong_ty").val(res.data);
            $('#edit-company').html(`<a href="/congty/edit?id=${res.data}" target="_blank" class="btn btn-warning">Sửa</a>`);
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function copyValue() {
    var copyText = document.getElementById("link_job_upload");
    copyText.select();
    copyText.setSelectionRange(0, 99999);
    document.execCommand("copy");

    $.notify(`Đã copy ` + copyText.value, "success");
}

$('#extend-add').click(function () {
    var me = $(this);
    if ($(this).hasClass('show')) {
        me.text("-Thu gọn");
        me.removeClass('show');
    } else {
        me.addClass('show');
        me.text("+Mở rộng");
    }
});