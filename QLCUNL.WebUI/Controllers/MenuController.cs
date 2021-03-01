using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLCUNL.WebUI.Controllers
{
    public class MenuController : BaseController
    {

        public MenuController(IMemoryCache _cache) : base(_cache)
        {
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
        public IActionResult Share(string id)
        {
            ViewBag.id = id;
            ViewBag.obj_type = (int)PhanQuyenObjType.MENU;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View();
        }
        public IActionResult All()
        {
            return View();
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
