$(function () {
    callAPI(`${API_URL}/baocao/dashboard`, null, "GET", function (res) {
        if (res.success) {
            $("[name='job_cua_toi']").text(res.job_cua_toi);
            $("[name='ung_vien_theo_job']").text(res.ung_vien_theo_job);
            $("[name='ung_vien']").text(res.ung_vien);
            $("[name='job_chung']").text(res.job_chung);
        }
    });
});