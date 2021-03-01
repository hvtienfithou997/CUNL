function search() {

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
    let ngay_nhan_job_from = 0, ngay_nhan_job_to = 0;
    if ($("#ngay_nhan_job_from").val() !== "") {
        ngay_nhan_job_from = toDate($("#ngay_nhan_job_from").val()).getTime() / 1000;
        if (isNaN(ngay_nhan_job_from)) {
            ngay_nhan_job_from = 0;
        }
    }
    if ($("#ngay_nhan_job_to").val() !== "") {
        ngay_nhan_job_to = toDate($("#ngay_nhan_job_to").val()).getTime() / 1000;
        if (isNaN(ngay_nhan_job_to)) {
            ngay_nhan_job_to = 0;
        }
    }
    callAPI(`${API_URL}/baocao/view?ngay_nhan_job_from=${ngay_nhan_job_from}&ngay_nhan_job_to=${ngay_nhan_job_to}&thuoc_tinh=${thuoc_tinh}&gop_job=false`, null, "GET", function (res) {
        if (res.success) {
            let tong_so_luong_yeu_cau = 0; let tong_so_tien_da_coc = 0; let tong_so_ung_vien_da_tuyen = 0;
            let tong_so_ung_vien_can_tuyen_tiep = 0; let tong_so_tien_can_phai_thanh_toan = 0; let tong_so_tien_con_phai_thanh_toan = 0;
            let html = ``;
            res.data.forEach(item => {
                html += `<tr>`;
                html += `<td>${item.stt}</td>`;
                html += `<td>${item.ngay_nhan_job}</td>`;
                html += `<td>${item.nguoi_tuyen_dung}</td>`;
                html += `<td>${item.dia_diem}</td>`;
                html += `<td>${item.vi_tri}</td>`;
                html += `<td>${item.so_luong_yeu_cau}</td>`;
                html += `<td>${item.don_gia}</td>`;
                html += `<td>${formatCurency(item.tien_coc)}</td>`;
                html += `<td>${item.so_ung_vien_da_tuyen_duoc}</td>`;
                html += `<td></td>`;
                html += `<td>${item.ngay_cac_ung_vien_di_lam}</td>`;
                html += `<td>${item.so_ung_vien_can_tuyen_tiep}</td>`;
                html += `<td>${formatCurency(item.so_tien_can_thanh_toan_theo_ung_vien)}</td>`;
                html += `<td>${formatCurency(item.so_tien_con_phai_thanh_toan)}</td>`;
                html += `<td>${item.tinh_trang_thanh_toan}</td>`;
                html += `<td>${item.han_thanh_toan}</td>`;
                html += `<td>${item.tinh_trang_hop_dong}</td>`;
                html += `</tr>`;
                tong_so_luong_yeu_cau += item.so_luong_yeu_cau;
                tong_so_tien_da_coc += item.tien_coc;
                tong_so_ung_vien_da_tuyen += item.so_ung_vien_da_tuyen_duoc;
                tong_so_tien_can_phai_thanh_toan += item.so_tien_can_thanh_toan_theo_ung_vien;
                tong_so_tien_con_phai_thanh_toan += item.so_tien_con_phai_thanh_toan;
            });
            html += `<tr style="background-color:#c9daf8;font-weight:600">`;
            html += `<td></td>`;
            html += `<td></td>`;
            html += `<td><strong>TỔNG CỘNG</strong></td>`;
            html += `<td></td>`;
            html += `<td></td>`;
            html += `<td>${tong_so_luong_yeu_cau}</td>`;
            html += `<td></td>`;
            html += `<td>${formatCurency(tong_so_tien_da_coc)}</td>`;
            html += `<td>${tong_so_ung_vien_da_tuyen}</td>`;
            html += `<td></td>`;
            html += `<td></td>`;
            html += `<td>${tong_so_ung_vien_can_tuyen_tiep}</td>`;
            html += `<td>${formatCurency(tong_so_tien_can_phai_thanh_toan)}</td>`;
            html += `<td></td>`;
            html += `<td></td>`;
            html += `<td></td>`;
            html += `<td></td>`;
            html += `</tr>`;

            $("#div_data").html(html);
        }
    })
}