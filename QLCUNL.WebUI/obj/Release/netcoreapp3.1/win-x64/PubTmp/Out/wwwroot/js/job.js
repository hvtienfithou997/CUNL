$(document).ready(function () {

    var my_url = sessionStorage.getItem('myUrl');
    
    $("#load_iframe").append(`<iframe src="${my_url}" width="1024" height="1024" frameborder="0" allowfullscreen></iframe>`);


    $("#btn_expand_thuoc_tinh").click(function () {
        $("#thuoc_tinh_2").toggleClass("show");
        if ($("#thuoc_tinh_2").hasClass("show")) {
            $("#btn_expand_thuoc_tinh").addClass("btn-info").text("- Thu gọn tìm thuộc tính");
        } else {
            $("#btn_expand_thuoc_tinh").removeClass("btn-info").text("+ Mở rộng tìm thuộc tính");
            $("#thuoc_tinh_down").removeClass("show");
        }
    });
    enableDisableButton();
    var choose = sessionStorage.getItem('myEmailChoose');
    $("#btn_expand_thuoc_tinh_down").click(function () {
        $("#thuoc_tinh_down").toggleClass("show");
        if ($("#thuoc_tinh_down").hasClass("show")) {
            $("#btn_expand_thuoc_tinh_down").addClass("btn-info").text("- Thu gọn tìm thuộc tính");
        } else {
            $("#btn_expand_thuoc_tinh_down").removeClass("btn-info").text("+ Mở rộng tìm thuộc tính");
        }
    });

    callAPI(`${API_URL}/user/appsetting`, null, "GET", function (res) {
        if (res.success) {
            $("#thoi_han_bao_hanh").val(res.data.bao_hanh);
            $("#so_lan_doi_toi_da").val(res.data.so_lan_doi);
            $("#tien_coc").val(res.data.tien_coc);
        }
    });
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
        fd.append('name', $("#chuc_danh").val());
        $.ajax({
            url: '/job/uploadjd',
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.success) {
                    $("#link_job_upload").val(`${document.location.origin}/${res.file_path}`);
                    $("#link_job").val(`${document.location.origin}/${res.file_path}`);
                } else {
                    $.notify(`${res.msg}`, "error");
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

    $("#btn_add_cong_ty").click(function (event) {
        event.preventDefault();
        $('.add-cong-ty').removeClass('d-none');
    });
    $("#btn-dismiss").click(function (event) {
        event.preventDefault();
        $(this).parent().parent().addClass('d-none');
    });

    $(".field-thuoc-tinh").append("<span type='button' class='float-right btn-danger remove-elem'> Bỏ chọn</span>");
    $(".remove-elem").click(function (e) {
        $(this).siblings('ul').find('input').prop('checked', false);
    });

    
    
});


$('#link_job').on('keyup', function () {
    var showBtn = true;
    $('#link_job').each(function () {
        showBtn = showBtn && ($(this).val() !== "");
        if (showBtn) {
            $("#preview_jd").removeClass('d-none');
        } else {
            $("#preview_jd").addClass('d-none');
        }

    });
    
});

$("#preview_jd").click(function () {
    sessionStorage.setItem('myUrl', $('#link_job').val());
});




var listct = [];

var listjobchecked = [];
var fo;
function onSubmit(event) {
    fo = $('#validate-add-job').bootstrapValidator({
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
            },
            auto_id: {
                validators: {
                    notEmpty: {
                        message: `Bạn phải nhập Auto_ID`
                    }
                }
            }
        },
        onError: function (e) {
            // Do something ...
        },
        onSuccess: function (e) {
            e.preventDefault();
            let id_auto = $("#auto_id").val();
            let chuc_danh = $('#chuc_danh').val();
            let tinh_thanh_id = $('#tinh_thanh > option:selected').val();
            let tinh_thanh_text = $('#tinh_thanh > option:selected').text();
            let so_luong = $('#so_luong').val() != "" ? $('#so_luong').val() : 0;
            let luong_chinh_thuc = $('#luong_chinh_thuc').val() != "" ? $('#luong_chinh_thuc').val() : 0;
            luong_chinh_thuc = replaceDot(luong_chinh_thuc);
            let luong_chinh_thuc_text = $('#luong_chinh_thuc_text').val();
            let nguoi_lien_he = $('#nguoi_lien_he').val();
            let cong_ty = { "id_cong_ty": $("#contain-id").val().length == 0 ? $("[name='cong_ty_add']").val() : $("#contain-id").val() };
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

            let do_uu_tien = $("#do_uu_tien").val();
            if (do_uu_tien !== "") {
                do_uu_tien = do_uu_tien;
            }
            else {
                $.notify(`Bạn chưa chọn cấp độ ưu tiên`, "success");
            }
            $("input[name^='thuoc_tinh']:checked").each(function (el) {
                try {
                    if ($(this).attr('data-type') === '0')
                        thuoc_tinh.push(parseInt($(this).val()));
                    else {
                        thuoc_tinh_rieng.push(parseInt($(this).val()));
                    }
                } catch (e) {
                    return e;
                }
            });
            let noi_dung = $('#noi_dung').val();
            let link_job = $('#link_job').val();
            let dia_chi = $('#dia_chi').val();
            let tinh_thanh = [];
            $("#tinh_thanh>option:selected").each(function () {
                tinh_thanh.push({ 'id_tinh': $(this).val(), 'ten_tinh': $(this).text() });
            });
            let time = $('#ngay_nhan_hd').val();
            if (time !== "") {
                let time_check = toDate(time);
                if (isNaN(time_check)) {
                    $.notify("Định dạng ngày tháng không hợp lệ", "error");
                    return;
                }
            }
            let ngay_nhan_hd = Math.floor(toDate($('#ngay_nhan_hd').val()).getTime() / 1000.0);
            if (isNaN(ngay_nhan_hd)) {
                ngay_nhan_hd = 0;
            }
            var obj = {
                "id_auto": id_auto, "chuc_danh": chuc_danh, "tinh_thanh": tinh_thanh, "so_luong": so_luong, "luong_chinh_thuc": luong_chinh_thuc, "ngay_nhan_hd": ngay_nhan_hd,
                "luong_chinh_thuc_text": luong_chinh_thuc_text, "nguoi_lien_he": nguoi_lien_he, "cong_ty": cong_ty, "don_gia": don_gia,
                "thoi_han_bao_hanh": thoi_han_bao_hanh, "so_lan_doi_toi_da": so_lan_doi_toi_da, "tien_coc": tien_coc, "tien_coc_tra_lai": tien_coc_tra_lai, "ghi_chu": ghi_chu,
                "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "noi_dung": noi_dung, "link_job": link_job, "link_job_upload": link_job_upload,
                "dia_chi": dia_chi, "nguoi_tao": user, "nguoi_sua": user, 'don_gia_theo_luong': don_gia_theo_luong, "owner": user, "do_uu_tien": do_uu_tien
            };

            callAPI(`${API_URL}/Job/add`, obj, "POST", function (res) {
                if (res.success) {
                    $.notify("Thêm mới JOB thành công", "success");
                    $('#chuc_danh').val("");
                    $('#do_uu_tien').val("");
                    $('#so_luong').val("1");
                    $('#luong_chinh_thuc').val("");
                    $('#don_gia').val("0");
                    $("#auto_id").val(res.data);
                    $("#noi_dung").val("");
                    $("#dia_chi").val("");
                    $("#link_job").val("");
                    $("#luong_chinh_thuc_text").val("");
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
                    $("#job_detail").append(`<li>-- <span class="font-weight-bold">${item.ten_tinh}</span></li>`);
                });
                $("#job_detail").append("</ul>");
                $("#job_detail").append(`<li>Số lượng: <span class="font-weight-bold">${res.data.so_luong}</span></li>`);
                $("#job_detail").append(`<li>Lương chính thức: <span class="font-weight-bold">${formatCurency(res.data.luong_chinh_thuc)}</span></li>`);
                $("#job_detail").append(`<li>Lương chính thức text: <span class="font-weight-bold">${res.data.luong_chinh_thuc_text}</span></li>`);
                $("#job_detail").append(`<li>Người liên hệ: <span class="font-weight-bold">${res.data.nguoi_lien_he}</span></li>`);
                if (res.data.cong_ty != null)
                    $("#job_detail").append(`<li>Công ty: <span class="font-weight-bold"><a href='/congty/detail/${res.data.cong_ty.id_cong_ty}'>${res.data.cong_ty.ten_cong_ty}</a></span></li>`);
                $("#job_detail").append(`<li>Đơn giá: <span class="font-weight-bold">${res.data.don_gia_theo_luong ? res.data.don_gia + " tháng lương" : formatCurency(res.data.don_gia)}</span></li>`);
                $("#job_detail").append(`<li>Có hợp đồng: <span class="font-weight-bold">${res.data.co_hop_dong ? "CÓ" : "KHÔNG"}</span></li>`);
                $("#job_detail").append(`<li>Ngày nhận hợp đồng: <span class="font-weight-bold">${epochToTime(res.data.ngay_nhan_hd)}</span></li>`);
                $("#job_detail").append(`<li>Thời hạn bảo hành: <span class="font-weight-bold">${res.data.thoi_han_bao_hanh}</span></li>`);
                $("#job_detail").append(`<li>Số lần đổi tối đa: <span class="font-weight-bold">${res.data.so_lan_doi_toi_da}</span></li>`);
                $("#job_detail").append(`<li>Đặt cọc: <span class="font-weight-bold">${res.data.dat_coc ? "CÓ" : "KHÔNG"}</span></li>`);
                $("#job_detail").append(`<li>Tiền cọc: <span class="font-weight-bold">${formatCurency(res.data.tien_coc)}</span></li>`);
                if (res.data.tien_coc_tra_lai > 0) {
                    $("#job_detail").append(`<li>Tiền trả lại: <span class="font-weight-bold">${res.data.tien_coc_tra_lai > 1000 ? formatCurency(res.data.tien_coc_tra_lai) : res.data.tien_coc_tra_lai * 100 + "%"}</span></li>`);
                }
                $("#job_detail").append(`<li>Ghi chú: <span class="font-weight-bold">${res.data.ghi_chu}</span></li>`);
                //var content = res.data.noi_dung.replace(/-/g, '<br/> -');
                if (typeof res.data.noi_dung !== 'undefined' && res.data.noi_dung.length > 0) {
                    $("#job_detail").append(`<li>Nội dung: <textarea rows="10" disabled style="width:100%; border:none; background-color:#fff">${res.data.noi_dung}</textarea></li>`);
                }
                $("#job_detail").append(`<li>Link job: <span class="font-weight-bold"><a target="_blank" href='${res.data.link_job}'>${res.data.link_job}</a></span><button class="btn btn-info" id="preview" style="float:right">Xem trước</button></li>`);
                $("#job_detail").append(`<li>Địa chỉ: <span class="font-weight-bold">${res.data.dia_chi}</span></li>`);
                if (res.data.thuoc_tinh != null) {
                    let thuoc_tinh = [];
                    res.data.thuoc_tinh.forEach(item => {
                        thuoc_tinh.push(`${item.ten}`);
                    });
                    $("#job_detail").append(`<li>Thuộc tính: <span class="font-weight-bold">${thuoc_tinh.join("/")}</span></li>`);
                }
                $("#job_detail").append(`<li>Ngày tạo: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_tao)}</span></li>`);
                $("#job_detail").append(`<li>Người tạo: <span class="font-weight-bold">${res.data.nguoi_tao}</span></li>`);
                $("#job_detail").append(`<li>Ngày sửa: <span class="font-weight-bold">${epochToTimeWithHour(res.data.ngay_sua)}</span></li>`);
                $("#job_detail").append(`<li>Người sửa: <span class="font-weight-bold">${res.data.nguoi_sua}</span></li>`);
                $('#preview').click(function () {
                    var url_temp = "";
                    let url = res.data.link_job;
                    var ext = url.split('.').pop();
                    let type_file = ["pdf", "docx"];
                    url_temp = ext != "pdf" ? "https://view.officeapps.live.com/op/view.aspx?src=" + url : url;
                    $("#job_detail").append(`<li><iframe id="iframe-cv" src="${url_temp}"></iframe></li>`);
                });

                $('#job_detail li ').addClass("list-group-item");
                let html_note = "";
                if (res.all_note != null) {
                    res.all_note.forEach(el => {
                        html_note += `<ul class="list-group row">`;

                        html_note += `<li class="list-group-item">${el.noi_dung}</li>`;
                        html_note += `<li class="list-group-item">${epochToTime(el.ngay_tao)}</li>`;
                        html_note += `<li class="list-group-item">${el.nguoi_tao}</li>`;
                        html_note += `</ul>`;
                    });

                    $("#job-note").html(html_note);
                    $("#job-note ul li").css('display', 'inline');
                }
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

    let thuoc_tinh = []; let thuoc_tinh_rieng = []; let op = $("[name='op']:checked").val();
    $("input[name^='thuoc_tinh']:checked", "#div_thuoc_tinh").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng.push(parseInt($(this).val()));
        } catch (e) {
            return e;
        }
    });

    let thuoc_tinh2 = []; let thuoc_tinh_rieng2 = []; let op2 = $("[name='op2']:checked").val();
    $("input[name^='thuoc_tinh']:checked", "#thuoc_tinh_2").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh2.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng2.push(parseInt($(this).val()));
        } catch (e) {
            return e;
        }
    });

    let value_filter = $("#value_filter").val();
    if (value_filter === "1") {
        value_filter = 1;
    }
    else {
        value_filter = 2;
    }

    let field_sort = $("#field_sort").val();
    let sort = $("#sort").val();

    if (id_cong_ty.length === 0) {
        id_cong_ty = $("[name='cong_ty']").val();
    }
    let ngay_nhan_hd = 0;
    let ngay_tao_to = 0;
    let ngay_tao_from = 0;
    if ($("#ngay_nhan_hd").val() !== "") {
        ngay_nhan_hd = Math.floor(toDate($('#ngay_nhan_hd').val()).getTime() / 1000.0);
    }
    if ($("#ngay_tao_from").val() !== "") {
        ngay_tao_from = Math.floor(toDate($('#ngay_tao_from').val()).getTime() / 1000.0);
    }
    if ($("#ngay_tao_to").val() !== "") {
        ngay_tao_to = Math.floor(toDate($('#ngay_tao_to').val()).getTime() / 1000.0);
        ngay_tao_to = (ngay_tao_to + 86399);
    }

    let url_new = `term=${term}&id_cong_ty=${id_cong_ty}&thuoc_tinh=${thuoc_tinh.join(',')}&thuoc_tinh_rieng=${thuoc_tinh_rieng.join(',')}&page=${page}&field_sort=${field_sort}&sort=${sort}`;
    window.history.pushState(window.location.href, "Danh sách job", `?${url_new}`);

    let tab_ung_vien = $('#table_data').DataTable({
        destroy: true, "ordering": false, "autoWidth": false,
        "dom": 'rt',/*https://datatables.net/reference/option/dom*/
        searchPanes: {
            controls: false, orderable: false
        },
        "language": {
            "emptyTable": "Không tìm thấy JOB phù hợp"
        },
        ajax: function (data, callback) {
            let url = `${API_URL}/Job/search?term=${term}&value_filter=${value_filter}&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&ngay_nhan_hd=${ngay_nhan_hd}&ngay_tao_to=${ngay_tao_to}&ngay_tao_from=${ngay_tao_from}&op=${op}&thuoc_tinh2=${thuoc_tinh2}&thuoc_tinh_rieng2=${thuoc_tinh_rieng2}&op2=${op2}&id_cong_ty=${id_cong_ty}&field_sort=${field_sort}&sort=${sort}&page=${page}&page_size=${PAGE_SIZE}`;
            $.get(url, {
            }, function (res) {
                $("#tutorial").addClass("d-none");
                $(".totalRecs").html("Tổng số Job: " + res.total);
                paging(res.total, 'search', page);
                callback({
                    recordsTotal: res.total,
                    recordsFiltered: res.total,
                    data: res.data
                });
            }).fail(function () {
                $(".totalRecs").html("Tổng số Job: 0");
                callback({
                    recordsTotal: 0,
                    recordsFiltered: 0,
                    data: []
                });
            });
        },
        columns: [{
            "class": "details-control",
            "orderable": false,
            "data": null,
            "defaultContent": "",
            "width": "15px"
        },
        { "data": "id_job", "name": "id_job", "searchable": false, "visible": false },
        {
            "data": "chuc_danh", "name": "Chức danh ID JOB", "searchable": true, "width": "30%"
        },
        { "data": "luong_chinh_thuc", "name": "Lương chính (VND)", "searchable": false, "width": "200px" },
        { "data": "nguoi_lien_he", "name": "Liên hệ", "searchable": false, "width": "150px" },

        { "data": `ngay_nhan_hd`, "name": "Ngày nhận hợp đồng", "searchable": false, "width": "150px" },
        {
            "data": null, "class": "action", "width": "160px"
        }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    let html_button = "";
                    if (row.is_owner) {
                        html_button += `<a class="btn btn-dark btn-big btn-big" href="userjob?id=${row.id_job}">Gán User (${row.so_luong_user_job})</a>`;
                    }
                    html_button += `<button class="btn btn-secondary modall btn-big" id="${row.id_job}" data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>`;
                    if (row.is_owner) {
                        html_button += `<a target="_blank" class="btn btn-dark btn-big btn-big" href="/NoteUngVienJob/viewungvientheojob?id_job=${row.id_job}">Xem ứng viên</a>`;
                    }
                    if (row.is_user_job) {
                        html_button += `<a title='Xem/Gán các ứng viên vào JOB này' class="btn btn-primary btn-big" href="/UserJob/AllUngVien?id_user_job=${row.id_user_job}&id_job=${row.id_job}">Gán ứng viên</a>`;
                        html_button += `<a class="btn btn-info btn-big" href="/NoteUngVienJob/ungvientheojob?id_job=${row.id_job}">Ứng viên đã gán</a>`;
                        html_button += `<button class="btn btn-secondary modall btn-big" id="${row.id_user_job}" data-toggle="modal" data-target="#modalTrangThai" onclick="onShowTrangThai(this.id)">Đổi trạng thái</button>`;
                    }

                    return html_button;
                },
                "targets": 6
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = "";
                    html_tmp += `<p>${row.ngay_nhan_hd > 0 ? epochToTime(row.ngay_nhan_hd) : ""} /</p>`;

                    html_tmp += ` ${row.ngay_tao > 0 ? epochToTime(row.ngay_tao) : ""}`;
                    html_tmp += `<ul class="ul_thong_ke_ung_vien">`;
                    if (row.ung_vien_thong_ke != null) {
                        row.ung_vien_thong_ke.forEach(item => {
                            html_tmp += `<li><span class='tt_eclp' title='${item.ten} ${item.total}'>${item.ten}:</span> <span class='tt_eclp_va'>${item.total}</span></li>`;
                        });
                    }
                    if (row.ung_vien_da_gui != null) {
                        html_tmp += `<li><span class='tt_eclp'>ƯV gửi NTD:</span><span class='tt_eclp_va'>${row.ung_vien_da_gui}</span></li>`;
                    }

                    if (row.ung_vien_da_gui > 0) {
                        let seen = "";
                        let reply = "";
                        seen = row.ntd_da_xem == true ? `${tick}` : `${cancel}`;
                        reply = row.ntd_da_phan_hoi == true ? `${tick}` : `${cancel}`;
                        html_tmp += `<li><span class='tt_eclp11 font-weight-bold'>NTD đã xem :</span> <span class='tt_eclp_va1 float-right'>${seen}</span></li>`;

                        if (row.ntd_da_phan_hoi) {
                            html_tmp += `<li><a href="/nhatuyendung/lognhatuyendung?id=${row.id_nha_tuyen_dung}" target="_blank"><span class='tt_eclp11 font-weight-bold'>NTD đã phản hồi :</span> <span class='tt_eclp_va1 float-right'>${reply}</span></a></li>`;
                        } else {
                            html_tmp += `<li><span class='tt_eclp11 font-weight-bold'>NTD đã phản hồi :</span> <span class='tt_eclp_va1 float-right'>${reply}</span></li>`;
                        }
                    }

                    html_tmp += `</ul>`;
                    return html_tmp;
                },
                "targets": 5
            }
            , {
                "render": function (data, type, row) {
                    let html_tmp = `${formatCurency(row.luong_chinh_thuc)}`;
                    if (row.luong_chinh_thuc_text != "") {
                        html_tmp += `<br>(${row.luong_chinh_thuc_text})`;
                    }
                    return html_tmp;
                },
                "targets": 3
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = "";

                    var email = extractEmails(row.nguoi_lien_he);
                    if (email != null) {
                        let text = email.toString();
                        html_tmp += `<kbd class="kbd1"><input type='checkbox' onclick="mailChecked()" value='${
                            formatEmails(text)}' name='choose_email'/> ${row.nguoi_lien_he}</kbd>`;
                        return html_tmp;
                    } else {
                        html_tmp += `${(row.nguoi_lien_he)}`;
                        return html_tmp;
                    }
                },
                "targets": 4
            },
            {
                "render": function (data, type, row) {
                    let html_tmp = `<span class="auto_id" onclick="copyText(this)"><code>${row.id_auto}</code></span><br>`;

                    if (row.do_uu_tien > 0) {
                        html_tmp += `<span>&nbsp<kbd class='kbd1'>Cấp ${row.do_uu_tien}</kbd></span><br>`;
                    }

                    html_tmp += `<a href="#" data-toggle="modal" data-link="${row.link_job}" onclick="showPreviewJob(this)" data-target="#preview-job">${row.chuc_danh}</a>`;
                    let cong_ty = row.cong_ty == null ? { 'ten_cong_ty': '', 'id_cong_ty': '' } : row.cong_ty;
                    html_tmp += `<br>`;
                    //html_tmp += `<a target="_blank" data-toggle="modal" data-target="#preview-cong-ty" href="#" onclick="showPreviewCongTy('${cong_ty.id_cong_ty}')"> - ${cong_ty.ten_cong_ty}</a>`;

                    html_tmp += `<span class="badge badge-info search_company" onclick="searchCompany('${cong_ty.ten_cong_ty}')">${cong_ty.ten_cong_ty}</span>`;
                    html_tmp += `<br>`;

                    row.tinh_thanh.forEach(item => {
                        html_tmp += ` - ${item.ten_tinh}`;
                    });
                    html_tmp += `<hr style="margin-top:0px"><div style="font-size:85%">${row.dia_chi}</div>`;
                    html_tmp += `<hr style="margin-top:0px"><div style="font-size:85%">Số lượng: ${row.so_luong}</div>`;

                    return html_tmp;
                },
                "targets": 2
            }
        ],
        pageLength: PAGE_SIZE,
        responsive: true,
        serverSide: true,
        processing: true
    });

    var detailRows = [];

    $('#table_data tbody').off('click').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tab_ung_vien.row(tr);
        var idx = $.inArray(tr.attr('id'), detailRows);

        if (row.child.isShown()) {
            tr.removeClass('details');
            row.child.hide();
            detailRows.splice(idx, 1);
        }
        else {
            tr.addClass('details');
            row.child(getDetailUngVien(row.data())).show();

            if (idx === -1) {
                detailRows.push(tr.attr('id'));
            }
        }
    });
    tab_ung_vien.off('draw').on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });
    callAPI(`${API_URL}/job/thongkethuoctinh?term=${term}&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&op=${op}&thuoc_tinh2=${thuoc_tinh2}&thuoc_tinh_rieng2=${thuoc_tinh_rieng2}&op2=${op2}&id_cong_ty=${id_cong_ty}&page=${page}`, null, "GET", function (res) {
        if (res.success) {
            $(".thong_ke").remove();
            res.data.forEach(item => {
                let ele = $(`input[name^='thuoc_tinh'][data-type='0'][value='${item.k}']`, "#div_thuoc_tinh");
                if (ele) {
                    ele.next().append(` <code class='thong_ke'>(${item.v})</code>`);
                }
            });
            res.data_rieng.forEach(item => {
                let ele = $(`input[name^='thuoc_tinh'][data-type='1'][value='${item.k}']`, "#div_thuoc_tinh");
                if (ele) {
                    ele.next().append(` <code class='thong_ke'>(${item.v})</code>`);
                }
            });
        }
    });
}

function searchCompany(name) {
    $("#cong_ty").val(name);
    search();
}

function getDetailUngVien(row) {
    var div = $('<div/>')
        .addClass('loading row_detail')
        .text('Loading...');

    let html_thuoc_tinh = "";
    if (row.thuoc_tinh != null) {
        row.thuoc_tinh.forEach(tt => {
            html_thuoc_tinh += `<li><kbd> - ${tt.ten}</kbd></li>`;
        });
    }

    callAPI(`${API_URL}/Job/viewdetail?id=${row.id_job}`, null, "GET", function (res) {
        let data_html = "";

        data_html += `<div><kbd class="kbd1">Chủ sở hữu: ${row.owner.length > 0 ? row.owner : row.nguoi_tao}</kbd>`;
        if (typeof res.extra !== 'undefined' && res.extra.length > 0) {
            data_html += ` - <span id="nguoi-nhan-job">`;
            data_html += "<code>Nguời đã nhận JOB: </code>";
            res.extra.forEach(item => {
                data_html += `<span style="text-decoration: underline;"> `;
                data_html += `+ ${item.id_user} (${epochToTime(item.ngay_nhan_job)})`;
                data_html += `</span>`;
            });
            data_html += `</span>`;
        }
        data_html += `</div>`;

        data_html += `<div style='text-align: right; width: 100%;'>`;
        if (row.is_user_job) {
            data_html += `<a target="_blank" class="btn btn-info btn-big btn-w1" href="/ungvien/add?auto_id=${row.id_auto}">Tạo ứng viên theo JOB</a>&nbsp`;
        }

        data_html += `<a target="_blank" class="btn btn-warning btn-big btn-w1" href="edit?id=${row.id_job}">Sửa</a>`;
        data_html += `&nbsp;<a target="_blank" class="btn btn-success btn-big btn-w1" href="detail/${row.id_job}">Xem</a>`;
        data_html += `&nbsp;<a target="_blank" class="btn btn-info btn-big" href="share?id=${row.id_job}">Chia sẻ</a>`;
        data_html += `<button class="btn btn-danger btn-s-small" id="${row.id_job}" onclick="deleteRec(this.id)">Xóa</button>`;
        data_html += "</div>";
        data_html += `<h6>${html_thuoc_tinh}</h6><hr>`;
        if (res.success) {
            data_html += `<kbd><div><strong>${typeof res.data.cong_ty.ten_cong_ty !== 'undefined' ? res.data.cong_ty.ten_cong_ty : ""}</strong></div>`;
            if (res.data.dia_chi != null && res.data.dia_chi.length > 0)
                data_html += `<div>Địa chỉ: ${res.data.dia_chi}</div>`;
            if (res.data.noi_dung != "")
                data_html += `<div><textarea rows="15" disabled style="width:100%; border:none; background-color:#fff">${res.data.noi_dung}</textarea></div></kbd>`;
        }
        div.html(data_html)
            .removeClass('loading');
    });

    return div;
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
function showPreviewCongTy(id) {
    callAPI(`${API_URL}/congty/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let data = res.data;
            $("#detail_cong_ty").empty();
            let html = ``;
            html += `<h4 class="font-weight-bold list-group list-group-flush">${data.ten_cong_ty}</h4>`;
            html += `<ul class="list-group list-group-flush">`;
            if (data.dia_chi != null)
                html += `<li class="list-group-item">Địa chỉ: <span class="font-weight-bold list-group list-group-flush">${data.dia_chi}</span></li>`;
            if (data.dien_thoai != null)
                html += `<li class="list-group-item">Điện thoại:<span class="font-weight-bold list-group list-group-flush">${data.dien_thoai}</span></li>`;
            if (data.lien_he != null && data.lien_he.length > 0) {
                let html_lien_he = "<ul>";
                html += `<li class="list-group-item">Liên hệ:`;

                data.lien_he.forEach(item => {
                    html_lien_he += `<li>${item.chuc_vu}</li>`;
                    html_lien_he += `<li>${item.email}</li>`;
                    html_lien_he += `<li>${item.sdt}</li>`;
                });
                html_lien_he += "</ul>";
                html += html_lien_he;
                html += `</li>`;
            }
            if (data.ghi_chu != null)
                html += `<li class="list-group-item">Ghi chú:<span class="font-weight-bold list-group list-group-flush">${data.ghi_chu}</span></li>`;

            html += `</ul>`;
            $("#detail_cong_ty").html(html);
        }
    });
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
                    hash[a.nhom].activities.push({ id: a.id, ten: a.ten, gia_tri: a.gia_tri, type: a.type });
                };
            }(Object.create(null)));

            result.forEach((item) => {
                if (item.nhom === 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row box-thuoc-tinh'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row box-thuoc-tinh'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='thuoc_tinh_danh_dau_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#thuoc_tinh_du_lieu').html(thuoc_tinh);
            $(".box-thuoc-tinh").append("<li style='width:100%; text-align:right'><span type='button' class='btn-danger remove-elem'> Bỏ chọn</span></li>");
            $(".remove-elem").click(function () {
                $(this).parent().siblings('li').find('input').prop('checked', false);
            });
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
            }
            $('#thuoc_tinh_du_lieu').append(`<a target='_blank' href="/thuoctinh/add?loai=4"><i class="icon-add"></i>Thêm thuộc tính</a>`);
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
            console.log(e);
        }
    });

    let obj = { "loai_obj": loai_obj, "id_obj": id, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng, "nguoi_tao": user, "nguoi_sua": user };

    callAPI(`${API_URL}/thuoctinhdulieu/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thành công", "success");
            $('#myModal').modal('hide');
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function onShowTrangThai(id) {
    callAPI(`${API_URL}/thuoctinh/canhan?term=&loai=1&id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let thuoc_tinh = '';
            thuoc_tinh = `<input class="d-none" id="id_obj_trang_thai" value="${id}">`;
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
                if (item.nhom === 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='trang_thai_danh_dau_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                } else {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    thuoc_tinh += "<ul class='check-box row'>";
                    item.activities.forEach((child) => {
                        thuoc_tinh += `<li class='col-md-12'><input type='radio' name='trang_thai_danh_dau_${item.nhom}' data-type='${child.type}' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                    });
                    thuoc_tinh += "</ul>";
                }
            });
            $('#trang_thai_user_job').html(thuoc_tinh);
            $('#trang_thai_user_job').append(`<a target='_blank' href="/thuoctinh/add?loai=1"><i class="icon-add"></i>Thêm thuộc tính</a>`);
            for (var i = 0; i < res.value.length; i++) {
                $(`input[name^='thuoc_tinh_danh_dau'][data-type='${res.value[i].v}'][value='${res.value[i].k}']`).prop('checked', true);
            }
        }
    });
}
function onCreateTrangThai(loai_obj) {
    let id = $('#id_obj_trang_thai').val();
    let thuoc_tinh = [], thuoc_tinh_rieng = [];
    $("input[name^='trang_thai_danh_dau']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0') {
                thuoc_tinh.push(parseInt($(this).val()));
            } else {
                thuoc_tinh_rieng.push(parseInt($(this).val()));
            }
        } catch (e) {
            console.log(e);
        }
    });

    let obj = { "loai_obj": loai_obj, "id_obj": id, "thuoc_tinh": thuoc_tinh, "thuoc_tinh_rieng": thuoc_tinh_rieng };

    callAPI(`${API_URL}/thuoctinhdulieu/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thành công", "success");
            $('#modalTrangThai').modal('hide');
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
        } catch (e) {
            console.log(e);
        }
    });
    enableDisableButton();
}

function enableDisableButton() {
    if ($("input[type='checkbox']:checked", "#all_job").length > 0) {
        $("#btn_send_mail").removeClass("disabled");
    } else {
        $("#btn_send_mail").addClass("disabled");
    }
}
function onCheckAll() {
    listjobchecked = [];
    $("input[type='checkbox']", "#all_job").prop("checked", $("#check_all").is(":checked"));
    $("input[name='jobUnchecked']:checked").each(function (el) {
        try {
            listjobchecked.push($(this).val());
        } catch (e) {
            console.log(e);
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
        } catch (e) {
            console.log(e);
        }
    });
}

function createCompany(event) {
    event.preventDefault();
    $('.add-cong-ty').removeClass('d-none');

    addContact(event);
}
function saveCompany(event) {
    event.preventDefault();
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
    let ten_cong_ty = $('#new-cong-ty').val();
    var obj = { "ten_cong_ty": ten_cong_ty, "lien_he": lien_he };
    callAPI(`${API_URL}/CongTy/indexcongty`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm công ty thành công, hãy tiếp tục tạo JOB", "warning");

            $('.add-cong-ty').addClass('d-none');
            $("#cong_ty_add").val(ten_cong_ty);
            $("#contain-id").val(res.data);
            $('#edit-company').html(`<a href="/congty/edit?id=${res.data}" target="_blank" class="btn-size btn btn-warning">Sửa</a>`);
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

$("#link_job_upload").click(function (event) {
    event.preventDefault();
    let link_job_upload = document.getElementById("link_job_upload");
    copyValue(link_job_upload);
});
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

$("#btn-preview").click(function (event) {
    event.preventDefault();
    var noidung = $("#noi_dung").val();
    if (noidung !== "") {
        var content = noidung.replace(/-/g, '<br/> -');
        $("#content_ung_vien").html(`<textarea rows="20" disabled style="width:100%; border:none; background-color:#fff">${noidung}</textarea>`);
    } else {
        $("#content_ung_vien").html("<p>Chưa có nội dung!</p>");
    }
});

function copy() {
}

function getAssignedUser(id, ele) {
    callAPI(`${API_URL}/UserJob/viewbyjob?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            let html = "<code>Nguời đã nhận JOB: </code>";
            res.data.forEach(item => {
                $(`[name='user_shared'][value='${item.id_user}']`).prop("checked", true);

                html += `<span style="text-decoration: underline;"> `;
                html += `+ ${item.id_user} (${epochToTime(item.ngay_nhan_job)})`;
                html += `</span>`;
            });
            $("#nguoi-nhan-job", ele).html(html);
        }
    });
}

$("#ngay_nhan_hd").blur(function () {
    checkDayInput($("#ngay_nhan_hd"));
});

$("#auto_id").blur(function () {
    let auto_id = $("#auto_id").val();
    if (auto_id != "") {
        callAPI(`${API_URL}/job/checkidexist?id_auto=${auto_id}`, null, "GET", function (res) {
            if (res.data) {
                $.notify(`Auto ID này đã tồn tại, vui lòng nhập lại!`, "error");
            }
        });
    }
});

function addContact(event) {
    event.preventDefault();
    var a = $('<input>', { text: "", class: "form-control element", type: "text", name: "chuc_vu", placeholder: "Nguời liên hệ - Chức vụ" });
    var b = $('<input>', { text: "", class: "form-control element", type: "text", name: "email", placeholder: "Email" });
    var c = $('<input>', { text: "", class: "form-control element", type: "text", name: "sdt", placeholder: "SĐT" });
    var d = $('<input>', { text: "", class: "form-control element", type: "text", name: "zalo", placeholder: "Zalo" });
    var e = $('<input>', { text: "", class: "form-control element", type: "text", name: "skype", placeholder: "Skype" });
    var f = $('<input>', { text: "", class: "form-control element", type: "text", name: "facebook", placeholder: "Facebook" });
    //var h = $('<span>', { text: " X", title: "Xóa", class: "text-center text-danger clear-field" });
    var g = $('<span class="form-inline">', {}).append(a).append(b).append(c).append(d).append(e).append(f);
    $('#contact').html(g);

    $(".clear-field").click(function () {
        $(this).parent().remove();
    });
}

$(".remove-element").click(function () {
    //$(this).siblings('span').find('input').prop('checked', false);
    $(this).siblings("div").children('ul').find('input').prop('checked', false);
    $(this).siblings("div").children('div').children('ul').find('input').prop('checked', false);
});

function extractEmails(text) {
    return text.match(/([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9._-]+)/gi);
}
function formatEmails(text) {
    return text.replace(/-/g, '');
}

var list_check_send_mail = [];
function sendEmail() {
    mailChecked();
    sessionStorage.setItem('myEmailChoose', list_check_send_mail);
}

function mailChecked() {
    list_check_send_mail = [];
    $("input[name='choose_email']:checked").each(function (el) {
        try {
            list_check_send_mail.push($(this).val());
        } catch (e) {
            console.log(e);
        }
    });
    enableDisableButton();
}

let tick = `<svg xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:cc="http://creativecommons.org/ns#"
   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
   xmlns:svg="http://www.w3.org/2000/svg"
   xmlns="http://www.w3.org/2000/svg"
   xml:space="preserve"
   style="enable-background:new 0 0 512 512;"
   viewBox="0 0 512 512"
   y="0px"
   x="0px" width="15px" height="15px"
   id="Capa_1"
   version="1.1"><metadata
     id="metadata45"><rdf:RDF><cc:Work
         rdf:about=""><dc:format>image/svg+xml</dc:format><dc:type
           rdf:resource="http://purl.org/dc/dcmitype/StillImage" /></cc:Work></rdf:RDF></metadata><defs
     id="defs43" /><path
     id="path2"
     d="M256,0C115.3,0,0,115.3,0,256s115.3,256,256,256s256-115.3,256-256S396.7,0,256,0z"
     style="fill:#77DD00;" /><path
     id="path4"
     d="M512,256c0,140.7-115.3,256-256,256V0C396.7,0,512,115.3,512,256z"
     style="fill:#66BB00;" /><polygon
     id="polygon6"
     points="401.8,212.5 226,388.299 99.699,262.299 142.301,219.699 226,303.699 256,273.699   359.5,170.2 "
     style="fill:#E7E7E7;" /><polygon
     id="polygon8"
     points="401.8,212.5 256,358.299 256,273.699 359.5,170.2 "
     style="fill:#D3D3D8;" /><g
     id="g10" /><g
     id="g12" /><g
     id="g14" /><g
     id="g16" /><g
     id="g18" /><g
     id="g20" /><g
     id="g22" /><g
     id="g24" /><g
     id="g26" /><g
     id="g28" /><g
     id="g30" /><g
     id="g32" /><g
     id="g34" /><g
     id="g36" /><g
     id="g38" /></svg>`;

let cancel = `<svg version="1.1" width="15px" height="15px" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
	 viewBox="0 0 512 512" style="enable-background:new 0 0 512 512;" xml:space="preserve">
<path style="fill:#FF3636;" d="M256,0C115.3,0,0,115.3,0,256s115.3,256,256,256s256-115.3,256-256S396.7,0,256,0z"/>
<path style="fill:#F40000;" d="M512,256c0,140.7-115.3,256-256,256V0C396.7,0,512,115.3,512,256z"/>
<polygon style="fill:#E7E7E7;" points="298.299,256 383.2,340.901 340.901,383.2 256,298.299 171.099,383.2 128.8,340.901
	213.701,256 128.8,171.099 171.099,128.8 256,213.701 340.901,128.8 383.2,171.099 "/>
<polygon style="fill:#D3D3D8;" points="298.299,256 383.2,340.901 340.901,383.2 256,298.299 256,213.701 340.901,128.8
	383.2,171.099 "/>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
<g>
</g>
</svg>`;



