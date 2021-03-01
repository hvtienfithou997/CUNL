function search(id_uv_search) {
    let term = $('#term').val();
    
    let thuoc_tinh = []; let thuoc_tinh_rieng = [];
    let id_ung_vien = "";
    $("input[name^='thuoc_tinh']:checked").each(function (el) {
        try {
            if ($(this).attr('data-type') === '0')
                thuoc_tinh.push(parseInt($(this).val()));
            else
                thuoc_tinh_rieng.push(parseInt($(this).val()));
        } catch{
        }
    });

    if (id_uv_search !== '' && id_uv_search !== null && id_uv_search !== undefined) {
        id_ung_vien = id_uv_search;
    }
    if (typeof term === 'undefined') {
        term = '';
    }
    if (typeof id_ung_vien === 'undefined') {
        id_ung_vien = '';
    }
    callAPI(`${API_URL}/NoteUngVien/search?term=${term}&id_ung_vien=${id_ung_vien}&thuoc_tinh=${thuoc_tinh}&thuoc_tinh_rieng=${thuoc_tinh_rieng}&page=1`, null, "GET", function (res) {
        $("#div_data").html(DIV_LOADING);
        if (res.success) {
            $("#div_data").empty();
            if (res.data != null && res.data.length > 0) {
                console.log(res.data);
                let html_data = "";
                res.data.forEach(item => {
                    html_data += `<tr>`;
                    html_data += `<td><a href="/ungvien/detail/${item.id_ung_vien}">${item.ho_ten_ung_vien}</a></td>`;
                    html_data += `<td>${item.ghi_chu}</td>`;
                    html_data += `<td>${epochToTime(item.ngay_tao)}</td>`;
                    html_data += `<td>${item.nguoi_tao}</td>`;
                    html_data += `<td><a class="btn btn-warning" href="edit?id=${item.id_note_ung_vien
                        }">Sửa</a><a class="btn btn-success" href="detail/${item.id_note_ung_vien
                        }">Xem</a><a class="btn btn-info" href="share?id=${item.id_note_ung_vien
                        }">Chia sẻ</a><button class="btn btn-dark modall" id="${item.id_note_ung_vien}" 
                            data-toggle="modal" data-target="#myModal" onclick="onShowThuocTinh(this.id)">Đánh dấu</button>`;
                    html_data += `<button class="btn btn-danger" id="${item.id_note_ung_vien}" onclick="deleteRec(this.id)">Xóa</button></td>`;
                    html_data += `</tr>`;
                });
                $("#div_data").html(html_data);
            }
        } else {
            $("#div_data").empty();
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });

}

function onShowThuocTinh(id) {

    callAPI(`${API_URL}/thuoctinh/canhan?loai=2`, null, "GET", function (res) {
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
                if (item.nhom == 0) {
                    thuoc_tinh += `Nhóm ${item.nhom}`;
                    item.activities.forEach((child) => {
                        thuoc_tinh += "<ul class='check-box row'>";
                        thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau' value='${
                            child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                        thuoc_tinh += "</ul>";
                    });
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
function deleteRec(id) {

    var mess = confirm("Bạn có muốn xóa bản ghi này không?");
    if (mess) {
        callAPI(`${API_URL}/noteungvien/delete?id=${id}`,
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