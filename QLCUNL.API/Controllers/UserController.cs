using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json.Linq;
using QLCUNL.API.Models;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize("SYS_ADMIN")]
    public class UserController : APIBase
    {
        // GET: api/User
        [HttpGet]
        [Route("all")]
        public IActionResult GetAll(string term, string id_team)
        {
            string[] fields = new string[] { "user_name", "full_name", "id_team", "email" };
            if (is_sys_admin || is_app_admin)
            {
                fields = new string[] { "user_name", "full_name", "id_team", "email", "last_login", "ip", "type" };
            }
            var data = BL.UserBL.Search(app_id, term, id_team, 1, out _, out _, 9999);
            return Ok(new DataResponse() { data = data, success = data != null, msg = "" });
        }

        [HttpGet]
        [Route("team")]
        public IActionResult GetAllInTeam()
        {
            string[] fields = new string[] { "user_name", "full_name", "id_team", "email" };
            if (is_sys_admin || is_app_admin)
            {
                fields = new string[] { "user_name", "full_name", "id_team", "email", "last_login", "ip", "type" };
            }
            var data = BL.UserBL.GetAllUserNameByAppId(app_id, fields, (is_sys_admin || is_app_admin));
            return Ok(new DataResponse() { data = data, success = data != null, msg = "" });
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, string id_team, int page, int page_size)
        {
            long total_recs = 0; string msg = "";
            string[] fields = new string[] { "user_name", "full_name", "id_team", "email" };
            string[] fields_group = new string[] { "id_team", "team_name" };

            var all_team = new Dictionary<long, GroupUser>();
            foreach (var item in BL.GroupUserBL.GetAll(app_id, fields_group))
            {
                if (!all_team.ContainsKey(item.id_team))
                {
                    all_team.Add(item.id_team, item);
                }
            }
            List<User> data = new List<User>();
            var lst_map = new List<UserMap>();
            if (is_sys_admin || is_app_admin)
            {
                fields = new string[] { "user_name", "full_name", "id_team", "email", "last_login", "ip", "type" };
                data = BL.UserBL.Search(app_id, term, id_team, page, out total_recs, out msg, page_size);

                foreach (var item in data)
                {
                    Models.UserMap mm = new Models.UserMap(item, all_team);
                    lst_map.Add((mm));
                }
            }
            else
            {
                var user_obj = BL.UserBL.GetByUserName(user);
                Models.UserMap mm = new Models.UserMap(user_obj, all_team);
                lst_map.Add((mm));
            }


            return Ok(new DataResponsePaging() { data = lst_map, success = data != null, msg = msg, total = total_recs });
        }

        // GET: api/User/5
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var user = QLCUNL.BL.UserBL.GetByUserName(id);
            
            if (user == null)
                return BadRequest();
            if (!IsInAppId(user)) return BadRequest();
            string user_str = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            dynamic data = new System.Dynamic.ExpandoObject();
            data = Newtonsoft.Json.JsonConvert.DeserializeObject(user_str);
            if (user.default_settings == null)
                user.default_settings = "";
            data.settings = Newtonsoft.Json.JsonConvert.DeserializeObject(user.default_settings);

            data.default_settings = "";
            return Ok(new DataResponse() { data = data, success = user != null, msg = "" });
        }

        // POST: api/User
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            if (is_sys_admin || is_app_admin)
            {
                DataResponse res = new DataResponse();
                try
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(value.ToString());
                    string obj_app_id = obj.app_id;
                    var setting = JToken.Parse(value.ToString());
                    if (setting != null && setting["settings"] != null)
                    {
                        var default_setting = setting["settings"].ToObject<UserSetting>();
                        obj.default_settings = setting["settings"].ToString();
                    }
                    SetMetaData(obj, false);
                    if (!is_sys_admin)
                    {
                        obj.roles.Remove(Role.SYS_ADMIN.ToString());
                    }
                    else
                    {
                        obj.app_id = obj_app_id;
                    }
                    if(!string.IsNullOrEmpty(obj.password))
                    {
                        obj.password = XMedia.XUtil.Encode(obj.password);
                    }
                    res.success = QLCUNL.BL.UserBL.Index(obj, out string msg);
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

        // PUT: api/User/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(value.ToString());
                obj.id_user = id;
                var setting = JToken.Parse(value.ToString());
                if (setting != null && setting["settings"] != null)
                {
                    var default_setting = setting["settings"].ToObject<UserSetting>();
                    obj.default_settings = setting["settings"].ToString();
                }
                if ((is_sys_admin || is_app_admin) || id == user)
                {
                    string obj_app_id = obj.app_id;
                    SetMetaData(obj, true);
                    if (!is_sys_admin)//Không có quyền sys_admin thì ko cho phép thêm role sys_admin và ko cho phép đổi APP_ID
                    {
                        obj.roles.Remove(Role.SYS_ADMIN.ToString());
                        obj.app_id = app_id;
                    }

                    var get_user = QLCUNL.BL.UserBL.GetByUserName(id);
                    if (!IsInAppId(get_user)) return BadRequest();
                    obj.id_user = get_user.id_user;
                    if (!string.IsNullOrEmpty(obj.password))
                    {
                        obj.password = XMedia.XUtil.Encode(obj.password);
                    }
                    res.success = QLCUNL.BL.UserBL.Update(obj);
                    //Nếu là app_admin thì tìm lại tất cả người dùng của app_id này và update lại cấu hình mặc định (tiền cọc, bảo hành...)
                    if (is_app_admin && id == user)
                    {
                        QLCUNL.BL.UserBL.UpdateDefaultSettingByAppId(app_id, obj.default_settings);
                    }
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }
            return Ok(res);
        }

        [HttpPut]
        [Route("useredit")]
        public IActionResult UserEditInfo([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(value.ToString());
                obj.id_user = obj.id_user;
                SetMetaData(obj, true);
                res.success = QLCUNL.BL.UserBL.UserEditInfo(user, obj.full_name, out string msg);
                res.msg = msg;
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }
            return Ok(res);
        }

        [HttpPut]
        [Route("change")]
        public IActionResult ChangePassword([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var user_change_pass = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(value.ToString());
                //user_change_pass.id_user = app_id + "_" + user_change_pass.id_user;
                user_change_pass.id_user = user;
                SetMetaData(user_change_pass, true);
                if (!string.IsNullOrEmpty(user_change_pass.password))
                {
                    user_change_pass.password = XMedia.XUtil.Encode(user_change_pass.password);
                    user_change_pass.old_password = XMedia.XUtil.Encode(user_change_pass.old_password);
                }
                res.success = QLCUNL.BL.UserBL.UpdatePassWord(user_change_pass.id_user, user_change_pass.password, user_change_pass.old_password, out string msg);
                res.msg = msg;
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }
            return Ok(res);
        }

        [HttpPut]
        [Route("reset")]
        public IActionResult ResetPassword([FromBody] object value)
        {
            if ((is_sys_admin || is_app_admin))
            {
                DataResponse res = new DataResponse();
                try
                {
                    var user_reset_pass = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(value.ToString());
                    user_reset_pass.id_user = user_reset_pass.id_user;
                    SetMetaData(user_reset_pass, true);
                    user_reset_pass.password = XMedia.XUtil.Encode(user_reset_pass.password);
                    res.success = QLCUNL.BL.UserBL.ResetPassWord(user_reset_pass.id_user, user_reset_pass.password,
                        out string msg, (is_sys_admin || is_app_admin));
                    res.msg = msg;
                }
                catch (Exception ex)
                {
                    res.msg = ex.StackTrace;
                    res.success = false;
                }

                return Ok(res);
            }

            return BadRequest();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete()]
        [Route("delete")]
        public bool Delete(string id)
        {
            return QLCUNL.BL.UserBL.Delete(id);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] object value)
        {
            var obj = JToken.Parse(value.ToString());
            QLCUNL.Models.User u_info = new User();
            bool is_success = false;
            string msg = ""; string token = "";
            var ip_add = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
            string browser = Request.Headers["User-Agent"];
            if (obj != null)
            {
                string user_name = obj["user"]?.ToString();
                string password = obj["pass"]?.ToString();
                password = XMedia.XUtil.Encode(password);
                u_info = QLCUNL.BL.UserBL.Login(user_name, password, ip_add, browser);
                is_success = u_info != null;

                if (is_success)
                {
                    msg = $"Chào {u_info.full_name}!";
                    token = TokenManager.BuildToken(u_info.app_id, u_info.user_name, u_info.roles, u_info.full_name, u_info.id_team.ToString(), ip_add);
                }
                else
                {
                    msg = "Đăng nhập không thành công";
                }
            }
            else
            {
                msg = "Yêu cầu tham số user và pass";
            }

            return Ok(new
            {
                data = !is_success ? new object() : new
                {
                    setting = u_info.default_settings,
                    u_info.full_name,
                    u_info.user_name,
                    u_info.email,
                    u_info.id_team,
                    u_info.app_id,
                    roles = u_info.roles == null ? new List<string>() : u_info.roles
                },
                success = is_success,
                msg,
                token
            });
        }
        [HttpGet]
        [Route("appsetting")]
        public IActionResult DefaultAppSetting()
        {
            DataResponse res = new DataResponse();
            var default_settings = QLCUNL.BL.UserBL.GetDefaultSettingByAppId(app_id);
            res.success = true;
            res.data = default_settings;
            return Ok(res);
        }
    }
}