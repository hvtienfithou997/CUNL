using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QLCUNL.API.Models;
using QLCUNL.BL;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : APIBase, IAPIBase
    {

        // GET : api/Menu/all
        [HttpPost]
        [Route("all")]
        public IActionResult All([FromBody] BaseGetAll req)
        {
            if ((is_sys_admin || is_app_admin))
            {
                List<Menu> menus = QLCUNL.BL.MenuBL.GetAll(app_id, req.page_index, req.page_size).ToList();
                return Ok(new DataResponse() { data = menus, success = menus != null, msg = req.page_size.ToString() });
            }
            return BadRequest();
        }

        // GET: ?id=abc123
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            if ((is_sys_admin || is_app_admin))
            {
                var menu = QLCUNL.BL.MenuBL.GetById(id);
                return Ok(new DataResponse() { data = menu, success = menu != null, msg = "" });
            }
            return BadRequest();
        }

        // POST: api/Menu
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            
            if ((is_sys_admin || is_app_admin))
            {
                DataResponse res = new DataResponse();
                try
                {
                    var menu = Newtonsoft.Json.JsonConvert.DeserializeObject<Menu>(value.ToString());
                    SetMetaData(menu, false);
                    res.success = QLCUNL.BL.MenuBL.Index(menu);
                }
                catch (Exception ex)
                {
                    res.msg = ex.Message; res.success = false;
                }
                return Ok(res);
            }
            return BadRequest();
        }

        // PUT: api/Menu/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            if ((is_sys_admin || is_app_admin))
            {
                DataResponse res = new DataResponse();
                try
                {
                    var menu = Newtonsoft.Json.JsonConvert.DeserializeObject<Menu>(value.ToString());
                    menu.id_menu = id;
                    SetMetaData(menu, true);
                    res.success = QLCUNL.BL.MenuBL.Update(menu);
                }
                catch (Exception ex)
                {
                    res.msg = ex.StackTrace; res.success = false;
                }

                return Ok(res);
            }
            return BadRequest();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        [Route("delete")]
        public bool Delete(string id)
        {
            if ((is_sys_admin || is_app_admin))
            {
                bool del = MenuBL.Delete(id);
                return del;
            }
            return false;
        }
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, int page, int page_size)
        {
            
            List<Menu> data = new List<Menu>();
            
            long total_recs = 0;
            string msg = "";

            data = QLCUNL.BL.MenuBL.Search(app_id, user, group, term, page, out total_recs, out msg, page_size, (is_sys_admin || (is_sys_admin || is_app_admin)));

            return Ok(new DataResponsePaging() { data = data, total = total_recs, success = data != null, msg = msg });
        }
        [HttpGet]
        [Route("shared")]
        public IActionResult Shared(string id)
        {
            if ((is_sys_admin || is_app_admin))
            {
                var data_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty, PhanQuyenObjType.MENU, id,
                Enum.GetValues(typeof(Quyen)).Cast<Quyen>().ToList(), 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
                return Ok(new DataResponse() { data = data_shared.Select(x => new PhanQuyenMapSimple(x)), success = data_shared != null, msg = "" });
            }
            return BadRequest();
        }
    }
}
