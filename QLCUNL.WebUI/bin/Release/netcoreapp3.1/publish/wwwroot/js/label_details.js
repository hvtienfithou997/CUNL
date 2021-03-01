$(function () {
    let url = document.location.href;
    let id = url.substring(url.lastIndexOf('/') + 1);
    bindDetail(id);
})