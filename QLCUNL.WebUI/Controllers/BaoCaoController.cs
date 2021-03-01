using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QLCUNL.Models;

namespace QLCUNL.WebUI.Controllers
{
    public class BaoCaoController : BaseController
    {
        public BaoCaoController(IMemoryCache _cache) : base(_cache) { }
        
        public IActionResult All()
        {
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetThuocTinh(LoaiThuocTinh.NOTE_UNG_VIEN_JOB, -1));


            return View("Index");
        }
        
    }
}