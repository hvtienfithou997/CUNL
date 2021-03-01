using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLCUNL.WebUI.Controllers
{
    public class GroupUserController : BaseController
    {
        // GET: /<controller>/
        public GroupUserController(IMemoryCache _cache) : base(_cache) { }
        public IActionResult All()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View("Add");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.id = id;
            return View("Edit");
        }
        [HttpGet]
        [Route("[controller]/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.id = id;
            return View("Detail");
        }
    }
}
