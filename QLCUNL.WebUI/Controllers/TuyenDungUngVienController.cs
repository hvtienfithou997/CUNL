using Microsoft.AspNetCore.Mvc;

namespace QLCUNL.WebUI.Controllers
{
    public class TuyenDungUngVienController : Controller
    {
        public IActionResult UngVienJob(string id, string id_ung_vien, string token, string log)
        {
            ViewBag.token1 = token;
            ViewBag.id = id;
            ViewBag.id_ung_vien = id_ung_vien;
            ViewBag.log = log;
            return View();
        }

        public IActionResult XemCvUngVien(string url)
        {
            return View();
        }
    }
}