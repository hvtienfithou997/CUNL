let list_note_ung_vien = [];
let list_ghi_chu = [];

function onSubmit(event) {
    event.preventDefault();
    let id_nha_tuyen_dung = $("#id_nha_tuyen_dung").val();
    let id_job = $("#id_job").val();
    let id_user_job = $("#id_user_job").val();
    let noi_dung = $("#noi_dung").val();
    list_note_ung_vien = [];
    list_ghi_chu = [];
    $("input[name=ung-vien-selected]").each(function() {
        let val = $(this).val();
        list_note_ung_vien.push(val);
    });
    $("textarea[name=ghi_chu_ung_vien_ntd]").each(function() {
        let val = $(this).val();
        list_ghi_chu.push(val);

    });

    var nha_td = { "id_nha_tuyen_dung": id_nha_tuyen_dung, "id_job": id_job, "id_user_job": id_user_job, "noi_dung": noi_dung, "nguoi_tao": user, "nguoi_sua": user }
    var obj = {
        "nha_tuyen_dung": nha_td, "note_ung_vien_share": { "id": list_note_ung_vien, "obj_type": 6 }, "note_ung_vien_gui_nha_tuyen_dung": list_ghi_chu
    }
    callAPI(`${API_URL}/nhatuyendung/add`, obj, "POST", function (res) {
        if (res.success) {
            $.notify("Thêm nhà tuyển dụng thành công", "success");
        } else {
            $.notify(`Thêm không thành công ${res.msg}`, "error");
        }
    });
}

function Search(page) {
    let term = $("[name='term']").val();
    //callAPI(`${API_URL}/nhatuyendung/getnhatuyendungseen?term=&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
    //    console.log(res);
    //})
    let thuoc_tinh = [];
    $("input[name^='da-xem']:checked").each(function (el) {
        try {
            thuoc_tinh.push(parseInt($(this).val()));
        } catch (e) {
            return e;
        }
    });
    thuoc_tinh = thuoc_tinh.join(',');
    callAPI(`${API_URL}/nhatuyendung/search?term=${term}&thuoc_tinh=${thuoc_tinh}&page=${page}&page_size=${PAGE_SIZE}`, null, "GET", function (res) {
        $("#div_data").html(DIV_LOADING);
        if (res.success) {
            $("#div_data").empty();
            $(".totalRecs").html("Tổng số nhà tuyển dụng: " + res.total);
            let html_data = "";

            res.data.forEach(item => {
                html_data += `<tr>`;
                html_data += `<td>${item.id_nha_tuyen_dung}</td>`;
                html_data += `<td>${item.chuc_danh}`;
                if (item.id_auto != "") {
                    html_data += `(${item.id_auto})</td>`;
                }                
                html_data += `<td>${item.noi_dung}</td>`;
                html_data += `<td><a href="/tuyendungungvien/ungvienjob?id=${item.id_nha_tuyen_dung}&token=${item.token}&log=n" target="_blank"  class="btn-big btn btn-dark" title="Xem trước khi gửi" type="button" >Xem trước</a>&nbsp`;
                html_data += `<button class="btn-big btn btn-success" type="button" data-id="${item.id_nha_tuyen_dung}" data-token="${item.token}" title="Lấy đường dẫn gửi cho nhà tuyển dụng" onclick="getLinkNtd(this)">Get Link</button>&nbsp`;
                html_data += `<a class="btn-big btn btn-info" href="/nhatuyendung/lognhatuyendung?id=${item.id}">Lịch sử</a>&nbsp`;
                html_data += `<a class="btn-big btn btn-warning" href="/nhatuyendung/edit?id=${item.id}">Sửa</a>&nbsp`;
                html_data += `<button class="btn btn-danger btn-s-small" id="${item.id}" onclick="deleteRecs(this.id)">Xóa</button></td>`;
                html_data += `</tr>`;
            });
            $("#div_data").html(html_data);
            /*START_PAGING*/
            paging(res.total, 'search', page);
            /*END_PAGING*/
        } else {
            $("#div_data").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}
function deleteRecs(id) {
    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/nhatuyendung/delete?id=${id}`,
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

function getLinkNtd(e) {
    var id = $(e).attr('data-id');
    var token = $(e).attr('data-token');
    var url = window.location.origin + `/tuyendungungvien/ungvienjob?id=${id}&token=${token}`;
    var copy_link = $('<input class="hide-elm">').val(url).appendTo('body').select()
    $(".hide-elm").css('opacity', '0');
    document.execCommand('copy')
    $.notify("Đã copy link gửi cho nhà tuyển dụng.", "success");
}

function SearchNoteUvJob(page) {
    console.log("hihihi")
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
                        html_data += `<td>${item.ghi_chu !== null ? item.ghi_chu : ""}</td>`;
                        html_data += `<td><input type="button" data-name='${item.ho_ten_ung_vien}' data-id="${item.id_note_ung_vien_job}" data-iduv="${item.id_ung_vien}" value="Chọn" onclick="selectUngVien(event, this, this.id)" class='btn btn-big btn-info' title='Chọn note ứng viên chia sẻ cho nhà tuyển dụng' name="noteungvienChecked" id="ung_vien_checked_${item.id_ung_vien}"></td>`;
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
    //
    // Lấy chi tiết job được chọn
}

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
    getTomTatUngVien(id_ung_vien);
    $.notify(`Đã chọn Note ${name}`, "success");
    countResult();

    $(`.remove-selected-${id_uv}`).click(function () {
        $(this).parent().remove();
        $(`#${id_uv}`).attr("disabled", false);
        countResult();
    });
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

function viewLog() {
    callAPI(`${API_URL}/nhatuyendung/getlog?id_obj=${id}`, null, "GET", function (res) {
        if (res.success) {
            let html_note = "";
            console.log(res);
            res.data_note.forEach(function (item) {
                html_note += `<tr>`;
                html_note += `<td>${item.ho_ten_ung_vien}</td>`;
                html_note += `<td>${item.noi_dung}</td>`;
                html_note += `<td>${epochToTime(item.ngay_tao)}</td>`;
                html_note += `</tr>`;
            });
            $("#note_ung_vien").html(html_note);

            let html_data = "";
            res.data_cv.forEach(function (item) {
                html_data += `<tr>`;
                html_data += `<td>${item.noi_dung}</td>`;
                html_data += `</tr>`;
            });
            $("#log_cv_ung_vien").html(html_data);

            let html_log = "";
            res.data_log.forEach((item) => {
                html_log += `<tr>`;
                html_log += `<td>${item.noi_dung}</td>`
                html_log += `</tr>`;
            })
            $("#log_nha_tuyen_dung").html(html_log);
        }
    })
}