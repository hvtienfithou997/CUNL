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
    public class ThuocTinhController : BaseController
    {
        public ThuocTinhController(IMemoryCache _cache) : base(_cache) { }
        public IActionResult All()
        {
            ViewBag.Shared = GetDisplayName(ThuocTinhType.SHARED);
            ViewBag.Private = GetDisplayName(ThuocTinhType.PRIVATE);

            return View("All");
        }
        public IActionResult CaNhan()
        {
            return View();
        }
        List<User> AllUserInApp()
        {
            List<User> lst = new List<User>();
            try
            {
                var res = Utils.APIUtils.CallAPI($"user/all", "", Token, out bool success, out string msg);

                if (success && !string.IsNullOrEmpty(res))
                {
                    var obj = JToken.Parse(res);
                    lst = obj["data"].ToObject<List<User>>();
                }
            }
            catch (Exception)
            {
            }
            return lst;
        }
        public IActionResult Add()
        {
            ViewBag.loai = HttpContext.Request.Query["loai"];
            ViewBag.all_user = AllUserInApp();
            return View("Add");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.id = id;
            ViewBag.all_user = AllUserInApp();
            return View("Edit");
        }
        [HttpGet]
        [Route("thuoctinh/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.Shared = GetDisplayName(ThuocTinhType.SHARED);
            ViewBag.Private = GetDisplayName(ThuocTinhType.PRIVATE);
            ViewBag.id = id;
            return View("Detail");
        }
    }
}