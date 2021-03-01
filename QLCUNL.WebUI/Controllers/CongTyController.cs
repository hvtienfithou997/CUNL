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
    public class CongTyController : BaseController
    {
        public CongTyController(IMemoryCache _cache) : base(_cache)
        {
        }
        List<ThuocTinh> GetAllThuocTinhCongTy()
        {
            return GetThuocTinh(LoaiThuocTinh.CONG_TY, -1);
        }

        public IActionResult Add()
        {
            
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinhCongTy());
            return View("Add");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.id = id;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinhCongTy());
            return View("Edit");
        }
        public IActionResult Share(string id)
        {
            ViewBag.id = id;
            ViewBag.obj_type = (int)PhanQuyenObjType.CONG_TY;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View("Share");
        }
        public IActionResult All(string term, string thuoc_tinh, string thuoc_tinh_rieng, int page = 1)
        {
            ViewBag.term = term;
            ViewBag.page = page;
            ViewBag.thuoc_tinh = thuoc_tinh;
            ViewBag.thuoc_tinh_rieng = thuoc_tinh_rieng;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinhCongTy());
            return View();
        }
        [HttpGet]
        [Route("[controller]/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.id = id;
            return View("Detail");
        }

        public IActionResult QuickCreateCompany()
        {
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew2(GetAllThuocTinhCongTy());
            return View("QuickCreateCompany");
        }
        public IActionResult SendMail()
        {
            //ViewBag.list_mail = lst;
            ViewBag.ip = XMedia.XUtil.GetToken();
            //ViewBag.Decode = XMedia.XUtil.DecodeToken(ViewBag.ip);
            return View("sendmail");
        }
        public IActionResult TotalMail()
        {
            ViewBag.ip = XMedia.XUtil.GetToken();
            //ViewBag.Decode = XMedia.XUtil.DecodeToken(ViewBag.ip);
            return View("totalmail");
        }
    }
}
