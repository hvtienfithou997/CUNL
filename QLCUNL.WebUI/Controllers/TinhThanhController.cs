using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLCUNL.WebUI.Controllers
{
    public class TinhThanhController : BaseController
    {
        public TinhThanhController(IMemoryCache _cache) : base(_cache)
        {

        }
        // GET: /<controller>/
        public IActionResult Add()
        {
            return View("Add");
        }
        public IActionResult Edit()
        {
            return View("Edit");
        }

        [HttpGet]
        [Route("tinhthanh/detail/{id}")]
        public IActionResult Detail(string id)
        {
            return View("Detail");
        }
    }
}
