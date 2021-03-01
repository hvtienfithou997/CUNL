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
    public class UserJobController : BaseController
    {
        // GET: /<controller>/
        public UserJobController(IMemoryCache _cache) : base(_cache)
        {
        }
        List<ThuocTinh> GetAllThuocTinh()
        {
            return GetThuocTinh(LoaiThuocTinh.USER_JOB, -1);
        }
        // GET: /<controller>/
        public IActionResult All(string term, string thuoc_tinh, string thuoc_tinh_rieng, int page = 1)
        {
            ViewBag.term = term;
            ViewBag.page = page;
            ViewBag.thuoc_tinh = thuoc_tinh;
            ViewBag.thuoc_tinh_rieng = thuoc_tinh_rieng;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            return View("All");
        }
        public IActionResult AllUngVien(string id_user_job, string id_job)
        {
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            ViewBag.id_user_job = id_user_job;
            ViewBag.id_job = id_job;
            return View("AllUngVien");
        }
        public IActionResult Add()
        {
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            return View("Add");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.id = id;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            return View("Edit");
        }

        [HttpGet]
        [Route("[controller]/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            ViewBag.id = id;
            return View("Detail");
        }

        public IActionResult Share(string id)
        {
            ViewBag.id = id;
            ViewBag.obj_type = (int)PhanQuyenObjType.USER_JOB;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View("Share");
        }
    }
}
