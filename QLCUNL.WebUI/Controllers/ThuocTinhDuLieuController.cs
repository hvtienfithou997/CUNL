using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;

namespace QLCUNL.WebUI.Controllers
{
    public class ThuocTinhDuLieuController : BaseController
    {
        
        public ThuocTinhDuLieuController(IMemoryCache _cache) : base(_cache)
        {
        }
        
        public IActionResult All()
        {
            return View("All");
        }
        public IActionResult Add()
        {
            return View("Add");
        }
        public IActionResult Edit()
        {
            return View("Edit");
        }
        [HttpGet]
        [Route("thuoctinhdulieu/detail/{id}")]
        public IActionResult Detail(string id)
        {
            return View("Detail");
        }
    }
}