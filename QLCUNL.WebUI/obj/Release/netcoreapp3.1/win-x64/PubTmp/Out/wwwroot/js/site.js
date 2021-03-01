var API_URL = "https://localhost:44302/api";
var DIV_LOADING = `<div id="div_loader"><div class="loader"></div></div>`;
const PAGE_SIZE = 30;
$.ajaxSetup({
    contentType: 'application/json',
    dataType: "json",
    crossDomain: true,
    xhrFields: {
        withCredentials: true
    },
    beforeSend: function (xhr) {
        xhr.setRequestHeader("Authorization", 'Bearer ' + API_TOKEN);
        $("body").append(DIV_LOADING);
    },
    success: function (data, textStatus, request) {
    },
    error: function (request, textStatus, errorThrown) {
        $("#div_loader").remove();
        if (request.status == 401) {
            let token_exp = request.getResponseHeader('token-expired');
            if (token_exp != null && token_exp == 'true') {
                document.location.href = "/";
            }
        }
    }, complete: function (xhr, status) {
        $("#div_loader").remove();
        let new_token = xhr.getResponseHeader('new-token');
        if (new_token != null && typeof new_token != 'undefined') {
            API_TOKEN = new_token;
        }
    }
});
function callAPI(url, obj, method, callback) {
    if (obj == null) {
        $.ajax({
            type: method,
            url: `${url}`,
            success: function (res, textStatus, request) {
                $("#div_loader").remove();
                if (typeof callback === "function") {
                    callback(res);
                }
            },
            failure: function (response) {
                $("#div_loader").remove();
                $.notify(`Lỗi xảy ra ${response.error}`, "error");
            },
            error: function (response) {
                $("#div_loader").remove();
                $.notify(`Lỗi xảy ra với API, vui lòng xem lại`, "error");
            }
        });
    } else {
        $.ajax({
            type: method,
            contentType: 'application/json',
            dataType: "json",
            url: `${url}`,
            data: JSON.stringify(obj),
            success: function (res) {
                $("#div_loader").remove();
                if (typeof callback === "function") {
                    callback(res);
                }
            },
            failure: function (response) {
                $("#div_loader").remove();
                $.notify(`Lỗi xảy ra ${response.error}`, "error");
            },
            error: function (request, textStatus, errorThrown) {
                $("#div_loader").remove();
                $.notify(`Lỗi xảy ra với API, vui lòng xem lại`, "error");
                if (request.status == 401) {
                    let token_exp = request.getResponseHeader('token-expired');
                    if (token_exp != null && token_exp == 'true') {
                        document.location.href = "/";
                    }
                    console.log(request.statusText);
                }
            }
        });
    }
}
function convert(str) {
    var date = new Date(str),
        mnth = ("0" + (date.getMonth() + 1)).slice(-2),
        day = ("0" + date.getDate()).slice(-2);
    return [date.getFullYear(), mnth, day].join("-");
}

function epochToTime(str) {
    if (str === 0)
        return "";
    var date = new Date(str * 1000),
        month = ("0" + (date.getMonth() + 1)).slice(-2),
        day = ("0" + date.getDate()).slice(-2);
    return [day, month, date.getFullYear()].join("-");
}
function formatCurency(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + '.' + '$2');
    }
    return val;
}
function toDate(dateStr) {
    var parts = dateStr.split("-");
    return new Date(parts[2], parts[1] - 1, parts[0])
}
function toDateWithHour(dateStr) {
    var parts_hour = dateStr.split(" ")[0].split(":");
    var parts = dateStr.split(" ")[1].split("-");
    return new Date(parts[2], parts[1] - 1, parts[0], parts_hour[0], parts_hour[1])
}
function epochToTimeWithHour(str) {
    if (str === 0)
        return "";
    var date = new Date(str * 1000),
        month = ("0" + (date.getMonth() + 1)).slice(-2),
        day = ("0" + date.getDate()).slice(-2);
    return `${[date.getHours().toString().padStart(2, '0'), date.getMinutes().toString().padStart(2, '0')].join(":")} ${[day, month, date.getFullYear()].join("-")}`;
}
function onShare(id, obj_type, type, rule) {
    try {
        let users = [];
        $("[name='user_shared']:checked").each(function () {
            users.push($(this).val());
        });
        let teams = [];
        $("[name='team_shared']:checked").each(function () {
            teams.push($(this).val());
        });
        if (typeof type === 'undefined') {
            type = 1;
        }
        /*if (typeof rule === 'undefined') {
            rule = 1;
        }*/
        rule = 2;
        let ngay_het = 0;

        if ($("#ngay_het").val() !== "") {
            ngay_het = toDate($("#ngay_het").val());
        }
        let quyen = [];
        $("input[name='quyen']:checked").each(function () {
            quyen.push($(this).val());
        });
        var obj = {
            'user': users, 'type': type, 'rule': rule, 'obj_type': obj_type, 'teams': teams, 'id': id, 'ngay_het': ngay_het, 'quyen': quyen
        };
        callAPI(`${API_URL}/phanquyen/share`, obj, "POST", function (res) {
            if (res.success) {
                $.notify("Chia sẻ thông tin thành công", "success");
            } else {
                $.notify(`Lỗi xảy ra khi chia sẻ thông tin: ${res.msg}`, "error");
            }
        });
    } catch (e) {
        console.log(`onShare ${e}`);
    }
}
function paging(total, func_name, page) {
    if (total === 0) {
        $('.paging').html(''); return;
    }
    let total_page = Math.ceil(total / PAGE_SIZE);
    let ext_space = false;
    let html_paging = `<div class="paginationjs-pages"><ul>`;
    if (page > 1)
        html_paging += `<li class="paging" onclick="${func_name}(${page - 1})"><a href="#">«</a></li>`;
    else
        html_paging += `<li class="paging"><a>«</a></li>`;
    for (var i = 1; i <= total_page; i++) {
        if (i > 2 && i < total_page - 1) {
            if (!ext_space) {
                ext_space = true;
                html_paging += `...`;
            }
        } else {
            if (page === i) {
                html_paging += `<li class="current"><span class="font-weight-bold">${i}</span></li>`;
            } else {
                html_paging += `<li class="paging" onclick="${func_name}(${i})"><a href="#" data-href="${i}">${i}</a></li>`;
            }
        }
    }
    if (page < total_page)
        html_paging += `<li class="paging" onclick="${func_name}(${page + 1})"><a href="#">»</a></li>`;
    else
        html_paging += `<li class="paging"><a>»</a></li>`;
    html_paging += `</ul></div>`;
    $('.paging').html(html_paging);
}
function reverseNumber(input) {
    return [].map.call(input, function (x) {
        return x;
    }).reverse().join('');
}

function plainNumber(number) {
    return number.split('.').join('');
}

function splitInDots(input) {
    try {
        var value = input.value,
            plain = plainNumber(value),
            reversed = reverseNumber(plain),
            reversedWithDots = reversed.match(/.{1,3}/g).join('.'),
            normal = reverseNumber(reversedWithDots);
        input.value = normal;
    } catch (e) {
        return e;
    }
};

function replaceDot(str) {
    try {
        if (str != null)
            str = str.replace(/\./g, '');
        return str;
    } catch (e) {
        return e;
    }
}
function groupItem(obj) {
    var thuoc_tinh = "";
    obj.forEach((item) => {
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
            item.activities.forEach((child) => {
                thuoc_tinh += "<ul class='check-box row'>";
                thuoc_tinh += `<li class='col-md-12'><input type='checkbox' name='thuoc_tinh_danh_dau' value='${
                    child.gia_tri}'> <span class="font-weight-bold">${child.ten}</span></li>`;
                thuoc_tinh += "</ul>";
            });
        }
    });
}

function copyValue(selector) {
    var copyText = selector;
    try {
        if (selector.value != "") {
            copyText.select();

            copyText.setSelectionRange(0, 99999);
            document.execCommand("copy");
            $.notify(`Đã copy ` + copyText.value, "success");
        } else {
            $.notify(`Không có gì để copy `, "error");
        }
    } catch (e) {
        return e;
    }
};


$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
});
function copyText(el) {
    if (window.getSelection && document.createRange) {
        var sel = window.getSelection();
        var range = document.createRange();
        range.selectNodeContents(el);
        sel.removeAllRanges();
        sel.addRange(range);
        
    } else if (document.selection && document.body.createTextRange) {
        var textRange = document.body.createTextRange();
        textRange.moveToElementText(el);
        textRange.select();
    }
    document.execCommand('copy');
    $.notify("Đã copy", "success");
}

function checkDayInput(selector) {
    let time = selector.val();
    if (time !== "") {
        let time_check = toDate(time);
        if (isNaN(time_check)) {
            $.notify("Định dạng ngày tháng không hợp lệ", "error");
            selector.parent().addClass('has-error');
            return;
        }
        else {
            selector.parent().removeClass('has-error');
        }
    }
}

// detech url
function urlify(text) {
    var urlRegex = /(https?:\/\/[^\s]+)/g;
    return text.replace(urlRegex, function (url) {
        return '<p><a target="_blank" href="' + url + '">' + url + '</a><p>';
    })
}