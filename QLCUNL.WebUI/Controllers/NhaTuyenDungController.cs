using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using QLCUNL.WebUI.Models;

namespace QLCUNL.WebUI.Controllers
{
    [AllowAnonymous]
    public class NhaTuyenDungController : BaseController
    {
        private readonly ILogger<NhaTuyenDungController> _logger;

        public NhaTuyenDungController(ILogger<NhaTuyenDungController> logger, IMemoryCache _cache) : base(_cache)
        {
            _logger = logger;
        }
        
        public IActionResult UngVienJob(string id, string id_ung_vien, string token)
        {
            ViewBag.token1 = token;
            ViewBag.id = id;
            ViewBag.id_ung_vien = id_ung_vien;            
            return View();
        }
        public IActionResult UngVien(string id, string id_ung_vien, string token)
        {
            ViewBag.id = id;
            ViewBag.id_ung_vien = id_ung_vien;
            ViewBag.token1 = token;
            return View();
        }
     
        public IActionResult ChiTietUngVien(string id_ung_vien)
        {
            ViewBag.id_ung_vien = id_ung_vien;
            return View();
        }
        public IActionResult Add()
        {

            ViewBag.all_job = SelectAllJob();
            return View("Add");
        }
        public IActionResult All()
        {
            return View("All");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.all_job = SelectAllJob();
            ViewBag.id = id;
            return View("Edit");
        }
        public IActionResult LogNhaTuyenDung(string id)
        {
            
            ViewBag.id = id;
            return View("LogNhaTuyenDung");
        }
    }
}
