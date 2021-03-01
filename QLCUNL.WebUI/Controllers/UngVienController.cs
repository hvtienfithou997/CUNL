using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using QLCUNL.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLCUNL.WebUI.Controllers
{
    public class UngVienController : BaseController
    {
        private IWebHostEnvironment hostingEnvironment;
        private IConfiguration configuration;

        public UngVienController(IMemoryCache _cache, IWebHostEnvironment _hostingEnvironment, IConfiguration _configuration) : base(_cache)
        {
            this.hostingEnvironment = _hostingEnvironment;
            this.configuration = _configuration;
            var provider = new PhysicalFileProvider(hostingEnvironment.WebRootPath);
        }

        private List<ThuocTinh> GetAllThuocTinh()
        {
            return GetThuocTinh(LoaiThuocTinh.UNG_VIEN, -1);
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

        public IActionResult All(string term, double ngay_di_lam_from, double ngay_di_lam_to, double ngay_tao_from, double ngay_tao_to, double luong_mong_muon_from, double luong_mong_muon_to, string thuoc_tinh, string thuoc_tinh_rieng)
        {
            ViewBag.term = term;
            ViewBag.ngay_di_lam_from = ngay_di_lam_from;
            ViewBag.ngay_di_lam_to = ngay_di_lam_to;
            ViewBag.ngay_tao_from = ngay_tao_from;
            ViewBag.ngay_tao_to = ngay_tao_to;
            ViewBag.luong_mong_muon_from = luong_mong_muon_from;
            ViewBag.luong_mong_muon_to = luong_mong_muon_to;
            ViewBag.thuoc_tinh = thuoc_tinh;
            ViewBag.thuoc_tinh_rieng = thuoc_tinh_rieng;

            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            ViewBag.tim_ung_vien_team_khac = Settings != null ? Settings.tim_ung_vien_team_khac : false;
            return View();
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
            ViewBag.obj_type = (int)PhanQuyenObjType.UNG_VIEN;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View("Share");
        }

        [HttpPost]
        public async Task<IActionResult> UploadCV(IList<IFormFile> files)
        {
            string file_name = "", file_path = ""; bool is_upload_ok = false; string msg = "";
            if (Request.Form.Files.Count > 0)
            {
                foreach (var source in Request.Form.Files)
                {
                    file_name = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                    file_name = this.EnsureCorrectFilename(file_name);
                    var ext = Path.GetExtension(file_name);
                    if (configuration != null && configuration.GetSection("AllowFileExt").Get<List<string>>().Contains(ext))
                    {
                        if (!string.IsNullOrEmpty(Request.Form["name"]))
                        {
                            file_name = XMedia.XUtil.GenSKU(Request.Form["name"]);
                        }
                        else
                        {
                            file_name = XMedia.XUtil.GenSKU(file_name.Replace(ext, ""));
                        }
                        file_name = $"{file_name}_{Path.GetRandomFileName().Replace(".", "")}{ext}";

                        file_path = this.GetPathAndFilename(file_name);
                        using (FileStream output = System.IO.File.Create(file_path))
                        {
                            await source.CopyToAsync(output);
                            is_upload_ok = true;
                            file_path = file_path.Replace(hostingEnvironment.WebRootPath, "").Replace("\\", "/");
                        }
                    }
                    else
                    {
                        if (configuration != null)
                            msg = $"Định dạng CV phải là: {string.Join(", ", configuration.GetSection("AllowFileExt").Get<List<string>>())}";
                    }
                }
            }

            return Ok(new { success = is_upload_ok, file_name, file_path, msg });
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            string app_id = App_Id;
            if (string.IsNullOrEmpty(app_id))
            {
                app_id = "temp";
            }

            string path = $"{this.hostingEnvironment.WebRootPath}\\uploads\\{app_id}\\{Team}\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }
    }
}