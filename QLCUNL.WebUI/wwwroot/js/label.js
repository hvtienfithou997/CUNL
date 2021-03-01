$(document).ready(function () {
    getAll();
});

function getAll() {
    let page = 1;
    let page_size = 4;

    let path = API_URL + "/label/get/all";
    $.ajax({
        url: path,
        type: "POST",
        datatype: "json",
        headers: { "Content-Type": "application/json" },
        data: (JSON.stringify({
            term: '',
            page_index: page,
            page_size: page_size
        })),
        success: function (data) {
            if (data.success) {
                let content = "<tr></tr>";
                $.each(data.data,
                    function (key, value) {
                        $('.table-data').append("<tr>");
                        $('.table-data').append(`<td>${value.id_label}</td>`);
                        $('.table-data').append(`<td>${value.ten_label}</td>`);
                        $('.table-data').append(`<td>${value.ngay_tao}</td>`);
                        $('.table-data').append(`<td>${value.nguoi_tao}</td>`);
                        $('.table-data').append(`<td>${value.ngay_sua}</td>`);
                        $('.table-data').append(`<td>${value.nguoi_sua}</td>`);
                        $('.table-data').append(`<td><a href="${value.id_label}">Edit</a> | <a href="${value.id_label}">Detail</a></tr>`);
                        $('.table-data').append("/<tr>");
                    });
            }
        },
        error: function (e) {
            console.log("error");
        }
    });
}


function onSearh() {
    let kewWord = $('.searchTerm').val();

}

    function onSubmit() {
        let ten_label = $('#ten_label').val();

        var obj = { "ten_label": ten_label };

        callAPI(`${API_URL}/Label`,
            obj,
            "POST",
            function(res) {
                if (res.success) {
                    $.notify("Thêm thành công", "success");
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
            });
    };

    function onUpdate() {

        let id_label = $('#id_label').val();
        let ten_label = $('#ten_label').val();

        var obj = { "id_label": id_label, "ten_label": ten_label };
        callAPI(`${API_URL}/Label/${id_label}`,
            obj,
            "PUT",
            function(res) {
                console.log("...");
                if (res.success) {
                    $.notify("Sửa thành công", "success");
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
            });
    };


function bindDetail(id) { callAPI(`${API_URL}/Label/${id}`, null, "GET", function(res) {
                if (res.success) {
                    if (res.data != null && res.data != undefined && res.data != "") {
                        
                        $("#label_detail").empty();
                        $("#label_detail").append(`<li>Id :<span class="font-weight-bold">${res.data.id_label}</span></li>`);
                        $("#label_detail").append(`<li>Label name :<span class="font-weight-bold">${res.data.user_name}</span></li>`);
                    }
                } else {
                    $.notify(`Lỗi xảy ra ${res.msg}`, "error");
                }
    });
}


