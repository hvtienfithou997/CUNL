$(function () {
    callAPI(`${API_URL}/nhatuyendung/view?id=${id_ntd}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data != null && res.data != undefined && res.data != "") {
                $("#id_nha_tuyen_dung").val(res.data.id_nha_tuyen_dung);
                $("#id_job").val(res.data.id_job);
                $("#id_user_job").val(res.data.id_user_job);
                $("#noi_dung").val(res.data.noi_dung);
            }
            SearchNoteUvJob(1);
            if (res.value != null && res.value != undefined && res.value != "") {
                res.value.forEach(function (item) {
                    var a = $('<button>', { text: "Bỏ", class: `btn-danger float-right remove-selected-${item.id_ung_vien}` });
                    var b = $('<input>', { disabled: "", style: "width:70%", class: "form-control text-primary", type: "text", value: `${item.ho_ten_ung_vien}` });
                    var c = $('<input>', { disabled: "", name: "ung-vien-selected", class: "form-control text-primary d-none", type: "text", value: `${item.id_note_ung_vien_job}` });
                    var d = $(`<textarea placeholder="Ghi chú ứng viên gửi cho nhà tuyển dụng" id="ttuv_${item.id_ung_vien}" name="ghi_chu_ung_vien_ntd" class="form-control" rows="2">`, {});
                    var e = $('<li class="list-group-item">', {}).append(a).append(b).append(c).append(d);
                    $("#ung-vien-selected").append(e);

                    setTimeout(function () {
                        $(`#ung_vien_checked_${item.id_ung_vien}`).attr("disabled", true);
                        //console.log($(`#ung_vien_checked_${item.id_ung_vien}`))
                    }, 1000)
                    getNoteTomTat(item.id_note_ung_vien_job, item.id_ung_vien);
                    countResult();
                    $(`.remove-selected-${item.id_ung_vien}`).click(function () {
                        $(this).parent().remove();
                        $(`#ung_vien_checked_${item.id_ung_vien}`).attr("disabled", false);
                        countResult();
                    });
                });
            }
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
});

function selectUngVien(event, me, id_uv) {
    event.preventDefault();
    $(`#${id_uv}`).attr("disabled", true);
    let name = $(me).data('name');
    let id = $(me).data('id');
    let id_ung_vien = $(me).data('iduv');
    var a = $('<button>', { text: "Bỏ", class: `btn-danger float-right remove-selected-${id_uv}` });
    var b = $('<input>', { disabled: "", style: "width:70%", class: "form-control text-primary", type: "text", value: `${name}` });
    var c = $('<input>', { disabled: "", name: "ung-vien-selected", class: "form-control text-primary d-none", type: "text", value: `${id}` });
    var d = $(`<textarea placeholder="Ghi chú ứng viên gửi cho nhà tuyển dụng" id="ttuv_${id_ung_vien}" name="ghi_chu_ung_vien_ntd" class="form-control" rows="2">`, {});
    var e = $('<li class="list-group-item">', {}).append(a).append(b).append(c).append(d);
    $("#ung-vien-selected").append(e);
    getNoteTomTat(id, id_ung_vien);

    $.notify(`Đã chọn Note ${name}`, "success");
    countResult();
    $(`.remove-selected-${id_uv}`).click(function () {
        $(this).parent().remove();
        $(`#${id_uv}`).attr("disabled", false);
        countResult();
    });
}

function getNoteTomTat(d, id_ung_vien) {
    callAPI(`${API_URL}/nhatuyendung/tomtatungvien?id=${d}`, null, "get", function (res) {
        if (res.success) {
            res.data.forEach(function (item) {
                $(`#ttuv_${id_ung_vien}`).val(item.noi_dung);
            });
        }
    })
}

function getTomTatUngVien(id) {
    callAPI(`${API_URL}/UngVien/view?id=${id}`, null, "GET", function (res) {
        if (res.success) {
            if (res.data.noi_dung != "") {
                $(`#ttuv_${id}`).val(res.data.noi_dung);
            }
        }
    });
}

function countResult() {
    var num = $("#ung-vien-selected").find("li").length;
    if (num > 0) {
        $("#count-selected").removeClass('d-none')
        $("#count-selected").html(`(${num})`)
    }
    else {
        $("#count-selected").addClass('d-none');
    }
}
let list_note_ung_vien = [];
let list_ghi_chu = [];
function onUpdate(event) {
    event.preventDefault();
    let id = id_ntd;
    let id_nha_tuyen_dung = $("#id_nha_tuyen_dung").val();
    let id_job = $("#id_job").val();
    let id_user_job = $("#id_user_job").val();
    let noi_dung = $("#noi_dung").val();
    list_note_ung_vien = [];
    list_ghi_chu = [];
    $("input[name=ung-vien-selected]").each(function () {
        let val = $(this).val();

        list_note_ung_vien.push(val);
    })

    $("textarea[name=ghi_chu_ung_vien_ntd]").each(function () {
        let val = $(this).val();
        list_ghi_chu.push(val)
    })

    var nha_td = { "id_nha_tuyen_dung": id_nha_tuyen_dung, "id_job": id_job, "id_user_job": id_user_job, "noi_dung": noi_dung, "nguoi_tao": user, "nguoi_sua": user }

    var obj = {
        "nha_tuyen_dung": nha_td, "note_ung_vien_share": { "id": list_note_ung_vien, "obj_type": 6 }, "note_ung_vien_gui_nha_tuyen_dung": list_ghi_chu
    }

    callAPI(`${API_URL}/nhatuyendung/${id}`, obj, "PUT", function (res) {
        if (res.success) {
            $.notify("Sửa thông tin thành công", "success");
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
};

function SearchNoteUvJob(page) {
    let term = $(`[name='term']`).val();
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
        } catch{
        }
    });
    let thuoc_tinh_string = thuoc_tinh.join(',');
    let thuoc_tinh_rieng_string = thuoc_tinh_rieng.join(',');
    let ngay_gio_phong_van_from = 0;
    let ngay_gio_phong_van_to = 0;
    let ngay_di_lam_from = 0;
    let ngay_di_lam_to = 0;
    let luong_thu_viec_from = $('#luong_thu_viec_from').val() != "" ? $('#luong_thu_viec_from').val() : 0;
    let luong_thu_viec_to = $('#luong_thu_viec_to').val() != "" ? $('#luong_thu_viec_to').val() : 0;
    let luong_chinh_thuc_from = $('#luong_chinh_thuc_from').val() != "" ? $('#luong_chinh_thuc_from').val() : 0;
    let luong_chinh_thuc_to = $('#luong_chinh_thuc_to').val() != "" ? $('#luong_chinh_thuc_to').val() : 0;

    if ($('#ngay_gio_phong_van_from').val() != "") {
        ngay_gio_phong_van_from = Math.floor(toDate($('#ngay_gio_phong_van_from').val()).getTime() / 1000.0);
    }
    if ($('#ngay_gio_phong_van_to').val() != "") {
        ngay_gio_phong_van_to = Math.floor(toDate($('#ngay_gio_phong_van_to').val()).getTime() / 1000.0);
    }
    if ($('#ngay_di_lam_from').val() != "") {
        ngay_di_lam_from = Math.floor(toDate($('#ngay_di_lam_from').val()).getTime() / 1000.0);
    }
    if ($('#ngay_di_lam_to').val() != "") {
        ngay_di_lam_to = Math.floor(toDate($('#ngay_di_lam_to').val()).getTime() / 1000.0);
    }

    if (typeof term === 'undefined') {
        term = '';
    }
    if (typeof id_ung_vien === 'undefined' || typeof id_ung_vien === 'object') {
        id_ung_vien = '';
    }
    if (typeof id_user === 'undefined' || typeof id_user === 'object') {
        id_user = '';
    }
    let id_job = $("#id_job").val();
    if (id_job === '' || typeof id_job === 'object') {
        id_job = $('#id_job').val();
    }

    if (isNaN(ngay_gio_phong_van_from)) {
        ngay_gio_phong_van_from = 0;
    }
    if (isNaN(ngay_gio_phong_van_to)) {
        ngay_gio_phong_van_to = 0;
    }
    if (isNaN(ngay_di_lam_from)) {
        ngay_di_lam_from = 0;
    }
    if (isNaN(ngay_di_lam_to)) {
        ngay_di_lam_to = 0;
    }
    if (typeof luong_thu_viec_from === 'undefined') {
        luong_thu_viec_from = 0;
    }
    if (typeof luong_thu_viec_to === 'undefined') {
        luong_thu_viec_to = 0;
    }
    if (typeof luong_chinh_thuc_from === 'undefined') {
        luong_chinh_thuc_from = 0;
    }
    if (typeof luong_chinh_thuc_to === 'undefined') {
        luong_chinh_thuc_to = 0;
    }

    callAPI(
        `${API_URL}/NoteUngVienJob/search?thuoc_tinh=${thuoc_tinh_string}&thuoc_tinh_rieng=${thuoc_tinh_rieng_string
        }&term=${term}&id_user=${id_user}&id_job=${id_job}&id_ung_vien=${id_ung_vien}&ngay_gio_phong_van_from=${
        ngay_gio_phong_van_from}&ngay_gio_phong_van_to=${ngay_gio_phong_van_to}&ngay_di_lam_from=${ngay_di_lam_from
        }&ngay_di_lam_to=${ngay_di_lam_to}&luong_thu_viec_from=${luong_thu_viec_from}&luong_thu_viec_to=${
        luong_thu_viec_to}&luong_chinh_thuc_from=${luong_chinh_thuc_from}&luong_chinh_thuc_to=${luong_chinh_thuc_to
        }&page=${page}&page_size=${PAGE_SIZE}`,
        null,
        "GET",
        function (res) {
            if (res.success) {
                $("#div_data").empty();
                $(".totalRecs").html("Tổng số bản ghi: " + res.total);
                if (res.data != null && res.data.length > 0) {
                    let html_data = "";
                    res.data.forEach(item => {
                        html_data += `<tr>`;
                        html_data += `<td class="d-none"><input type="type" id="id_note_uv_job" value="${item.id_note_ung_vien_job}"/></td>`;
                        html_data += `<td><a href="/ungvien/detail/${item.id_ung_vien}" target="_blank">${item.ho_ten_ung_vien}<br>${item.so_dien_thoai = item.so_dien_thoai != "" ? item.so_dien_thoai : ""}<br>${item.email = item.email != "" ? item.email : ""}</a></td>`;
                        let html_thuoc_tinh = "";
                        if (item.thuoc_tinh != null) {
                            item.thuoc_tinh.forEach(tt => {
                                html_thuoc_tinh += `<p>- ${tt.ten}</p>`;
                            });
                        }
                        html_data += `<td style='vertical-align:bottom'>${html_thuoc_tinh}`;
                        html_data += `<td style='vertical-align:bottom'>${item.ngay_gio_phong_van > 0 ? "<p>" + epochToTimeWithHour(item.ngay_gio_phong_van) + "</p>" : ""}`;
                        html_data += `<td width:150px>${item.ngay_di_lam > 0 ? epochToTime(item.ngay_di_lam) : ""}</td>`;
                        html_data += `<td>${item.luong_thu_viec ? formatCurency(item.luong_thu_viec) : ""}/${item.luong_thu_viec > 0 ? formatCurency(item.luong_chinh_thuc) : ""}</td>`;
                        html_data += `<td><input type="button" data-name='${item.ho_ten_ung_vien}' data-id="${item.id_note_ung_vien_job}" data-iduv="${item.id_ung_vien}" value="Chọn" onclick="selectUngVien(event, this, this.id)" class='btn btn-big btn-info noteungvienChecked' title='Chọn note ứng viên chia sẻ cho nhà tuyển dụng' name="noteungvienChecked" id="ung_vien_checked_${item.id_ung_vien}"></td>`;

                        html_data += `</tr>`;
                    });
                    $("#div_data").html(html_data);
                }
                paging(res.total, 'search', page);
            } else {
                $("#div_data").empty();
                $.notify(`Lỗi xảy ra ${res.msg}`, "error");
            }
        });
}