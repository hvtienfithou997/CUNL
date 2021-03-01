using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLCUNL.WebUI.Controllers
{
    public class JobController : BaseController
    {
        private IWebHostEnvironment hostingEnvironment;
        private IConfiguration configuration;
        // GET: /<controller>/
        public JobController(IMemoryCache _cache, IWebHostEnvironment _hostingEnvironment, IConfiguration _configuration) : base(_cache)
        {
            this.hostingEnvironment = _hostingEnvironment;
            this.configuration = _configuration;
            var provider = new PhysicalFileProvider(hostingEnvironment.WebRootPath);

        }
        List<ThuocTinh> GetAllThuocTinh()
        {
            return GetThuocTinh(LoaiThuocTinh.JOB, -1);
        }
        [HttpGet]

        public IActionResult All(string term, string id_cong_ty, string thuoc_tinh, string thuoc_tinh_rieng, int page = 1)
        {
            ViewBag.term = term;
            ViewBag.id_cong_ty = id_cong_ty;
            ViewBag.page = page;
            ViewBag.thuoc_tinh = thuoc_tinh;

            ViewBag.thuoc_tinh_rieng = thuoc_tinh_rieng;

            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinhNew(GetAllThuocTinh());
            ViewBag.thuoc_tinh_checkbox2 = BuildBoxThuocTinhNew2(GetAllThuocTinh());
            return View();
        }
        public IActionResult Add()
        {
            ViewBag.list_tinh_thanh = SelectAllTinhThanh();
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            ViewBag.Settings = Settings;
            return View();
        }
        public IActionResult Edit(string id)
        {
            ViewBag.list_tinh_thanh = SelectAllTinhThanh();
            ViewBag.id = id;
            ViewBag.thuoc_tinh_checkbox = BuildBoxThuocTinh(GetAllThuocTinh());
            ViewBag.Settings = Settings;
            return View();
        }

        [HttpGet]
        [Route("job/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public IActionResult Share(string id)
        {
            ViewBag.id = id;
            ViewBag.obj_type = (int)PhanQuyenObjType.JOB;
            ViewBag.list_team = SelectAllTeam();
            ViewBag.all_user = BuildAllUserCheckBox();
            return View();
        }
        public IActionResult UserJob(string id)
        {
            ViewBag.id = id;
            ViewBag.all_user = BuildAllUserInTeamCheckBox();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadJD(IList<IFormFile> files)
        {
            string file_name = "", file_path = ""; bool is_upload_ok = false; string msg = "";
            if (Request.Form.Files.Count > 0)
            {
                foreach (var source in Request.Form.Files)
                {
                    file_name = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                    file_name = this.EnsureCorrectFilename(file_name);
                    var ext = Path.GetExtension(file_name);
                    if (configuration.GetSection("AllowFileExt").Get<List<string>>().Contains(ext))
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
                        msg = $"Định dạng JD phải là: {string.Join(", ", configuration.GetSection("AllowFileExt").Get<List<string>>())}";
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

        public IActionResult Preview()
        {
            return View();
        }

    }
}
