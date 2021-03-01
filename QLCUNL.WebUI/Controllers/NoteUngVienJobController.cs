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
    /// <summary>
    /// COMMENT
    /// </summary>
    public class NoteUngVienJobController : BaseController
    {
        public NoteUngVienJobController(IMemoryCache _cache) : base(_cache)
        {
        }
        List<ThuocTinh> GetAllThuocTinh()
        {
            return GetThuocTinh(LoaiThuocTinh.NOTE_UNG_VIEN_JOB, -1);
        }
        
        // GET: /<controller>/
        public IActionResult All(string id_job,string id_ung_vien)
        {
            ViewBag.id_job = id_job;
            ViewBag.id_ung_vien = id_ung_vien;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            return View("All");
        }
        public IActionResult UngVienTheoJob(string id_job, string id_ung_vien)
        {
            ViewBag.id_job = id_job;
            ViewBag.id_ung_vien = id_ung_vien;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            return View();
        }
        public IActionResult ViewUngVienTheoJob(string id_job, string id_ung_vien)
        {

            ViewBag.id_job = id_job;
            ViewBag.id_ung_vien = id_ung_vien;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            return View();
        }
        public IActionResult Add()
        {
            ViewBag.list_ung_vien = SelectAllUngVien();
            ViewBag.list_job = SelectAllJob();
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            return View("Add");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.list_ung_vien = SelectAllUngVien();
            ViewBag.list_job = SelectAllJob();
            ViewBag.id = id;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            return View("Edit");
        }

        [HttpGet]
        [Route("[controller]/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.id = id;
            return View("Detail");
        }

        public IActionResult Share(string id)
        {
            ViewBag.id = id;
            ViewBag.obj_type = (int)PhanQuyenObjType.NOTE_UNG_VIEN_JOB;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View("Share");
        }
    }
}