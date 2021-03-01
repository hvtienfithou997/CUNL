using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace QLCUNL.WebUI.Controllers
{
    public class LabelController : BaseController
    {
        public LabelController(IMemoryCache _cache) : base(_cache) { }
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
        [Route("label/detail/{id}")]
        public IActionResult Detail(string id)
        {
            return View("Detail");
        }
    }
}