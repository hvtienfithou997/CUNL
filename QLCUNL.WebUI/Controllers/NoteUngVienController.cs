using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;
namespace QLCUNL.WebUI.Controllers
{
    public class NoteUngVienController : BaseController
    {
        public NoteUngVienController(IMemoryCache _cache) : base(_cache)
        {
            
        }
        List<ThuocTinh> GetAllThuocTinh()
        {
            return GetThuocTinh(LoaiThuocTinh.NOTE_UNG_VIEN, -1);
        }
        

        public IActionResult All(string id)
        {
            ViewBag.id_uv_search = id;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            return View();
        }
        public IActionResult Add()
        {
            List<UngVien> list_ung_vien = SelectAllUngVien();
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            ViewBag.list_ung_vien = list_ung_vien;
            return View();
        }
        public IActionResult Edit(string id)
        {
            List<UngVien> list_ung_vien = SelectAllUngVien();
            ViewBag.id = id;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            ViewBag.list_ung_vien = list_ung_vien;
            return View();
        }

        [HttpGet]
        [Route("[controller]/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public IActionResult Share(string id)
        {
            ViewBag.id = id;
            ViewBag.obj_type = (int)PhanQuyenObjType.NOTE_UNG_VIEN;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View();
        }
    }
}