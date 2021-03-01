using Microsoft.AspNetCore.Mvc;
using QLCUNL.BL;
using QLCUNL.Models;
using System;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize("SYS_ADMIN")]
    public class GroupUserController : APIBase
    {
        // GET: api/GroupUser
        [HttpGet]
        [Route("all")]
        public IActionResult GetAll()
        {
            string[] fields = new string[] { "team_name", "id_team" };

            var data = BL.GroupUserBL.GetAll(app_id, fields);
            return Ok(new DataResponse() { data = data, success = data != null, msg = "" });
        }
        
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, int page)
        {
            if (is_app_admin || is_sys_admin)
            {
                string[] fields = new string[] { "team_name", "id_team" };
                var data = BL.GroupUserBL.Search(app_id, term, page, out long total_recs, out string msg);
                return Ok(new DataResponsePaging() { data = data, success = data != null, msg = msg, total = total_recs });
            }
            return Ok(new DataResponse() { msg = "Bạn không có quyền ở đây" });
        }

        // GET: api/GroupUser/5
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var GroupUser = QLCUNL.BL.GroupUserBL.GetById(id);
            return Ok(new DataResponse() { data = GroupUser, success = GroupUser != null, msg = "" });
        }

        [HttpDelete]
        [Route("delete")]
        public bool Delete(string id)
        {
            bool del = GroupUserBL.Delete(id);
            return del;
        }

        // POST: api/GroupUser
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            if ((is_sys_admin || is_app_admin))
            {
                DataResponse res = new DataResponse();
                try
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupUser>(value.ToString());
                    SetMetaData(obj, false);
                    res.success = QLCUNL.BL.GroupUserBL.Index(obj, out string msg);
                    res.msg = msg;
                }
                catch (Exception ex)
                {
                    res.msg = ex.Message; res.success = false;
                }

                return Ok(res);
            }
            return BadRequest();
        }

        // PUT: api/GroupUser/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            if ((is_sys_admin || is_app_admin))
            {
                DataResponse res = new DataResponse();
                try
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GroupUser>(value.ToString());
                    obj.id = id;
                    SetMetaData(obj, true);
                    res.success = QLCUNL.BL.GroupUserBL.Update(obj);
                }
                catch (Exception ex)
                {
                    res.msg = ex.StackTrace; res.success = false;
                }

                return Ok(res);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("allgroupuser")]
        public IActionResult Search()
        {
            long total_recs = 0;
            string msg = "";

            var data = QLCUNL.BL.GroupUserBL.Search(app_id, 1, out total_recs, out msg, 9999);
            return Ok(new DataResponsePaging() { data = data, total = total_recs, success = data != null, msg = msg });
        }
    }
}