$(document).ready(function() {
    search();
});

function search() {
    let status = $('#status').val() != '' ? $('#status').val() : "";
    callAPI(`${API_URL}/congty/getallmail?status=${status}`, null, "GET", function (res) {
        if (res.success) {
            if (res.success && res.data != null) {
                let html = "";
                $('.total-recs').html('Tổng số bản ghi:' + res.total);
                var count = 1;
                res.data.forEach(item => {
                    html += `<tr>`;
                    html += `<td>${count}</td>`;
                    html += `<td>${item.Email}</td>`;
                    switch (item.Status) {
                        case "1":
                            item.Status = "Mới";
                            break;
                        case "2":
                            item.Status = "Chờ";
                            break;
                        case "3":
                            item.Status = "Đã gửi";
                            break;
                        case "4":
                            item.Status = "Lỗi";
                            break;
                        case "5":
                            item.Status = "Hủy";
                            break;
                    }
                    console.log(item.Status);
                    html += `<td class="font-weight-bold">${item.Status}</td>`;
                    html += `<td>${epochToTime(item.ngay_tao)}</td>`;
                    html += `</tr>`;
                    count++;
                });
                $("#div_data").html(html);
            }
            paging(res.total, 'search', 1);
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function updateStatusMail() {
    callAPI(`${API_URL}/congty/updatestatusmail`, null, "GET", function (res) {
        if (res.success) {
            var data = new FormData();
            data.append("token", token);
            data.append("obj", JSON.stringify(res.data));
            $.ajax({
                url: "https://mail.x2convert.com/ajax/CheckStatus.ashx",
                type: 'post',
                data: data,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.Status) {
                        checkStatusSend(response.Data);
                    } else {
                        console.log(response.Message);
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

function checkStatusSend(data) {
    callAPI(`${API_URL}/CongTy/updatelog`, data, "PUT", function (res) {
        if (res.success) {
            $.notify("Cập nhật thành công", "success");
            search();
        } else {
            $.notify(`Lỗi xảy ra ${res.msg}`, "error");
        }
    });
}

