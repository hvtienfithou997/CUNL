using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace QLCUNL.WebUI.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private string _my_menu = "";
        protected IMemoryCache cache;

        public BaseController(IMemoryCache _cache)
        {
            cache = _cache;
        }

        private string _token;
        private int _team;
        private string _app_id;

        public string Token
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var claim_token = User.Claims.FirstOrDefault(x => x.Type == "token");
                    if (claim_token != null)
                    {
                        _token = claim_token.Value;
                    }
                }
                return _token;
            }
            set => _token = value;
        }

        public UserSetting Settings
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var claim_user_data = User.FindFirst(ClaimTypes.UserData);
                    if (claim_user_data != null)
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<UserSetting>(claim_user_data.Value);
                    }
                }
                return new UserSetting();
            }
        }

        public string my_menu
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    _my_menu = $"{User.Identity.Name}_menu";
                }
                return _my_menu;
            }
        }

        public int Team
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var claim_team = User.FindFirst(ClaimTypes.GroupSid);
                    if (claim_team != null)
                    {
                        _team = Convert.ToInt32(claim_team.Value);
                    }
                }
                return _team;
            }
            set => _team = value;
        }

        public string App_Id
        {
            get
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var claim = User.FindFirst(ClaimTypes.Sid);
                    if (claim != null)
                    {
                        _app_id = claim.Value;
                    }
                }
                return _app_id;
            }
        }

        private List<Menu> GetMenu()
        {
            List<Menu> lst_menu = new List<Menu>();
            if (User != null && User.Identity.IsAuthenticated)
            {
                if (!cache.TryGetValue<List<Menu>>(my_menu, out lst_menu))
                {
                    try
                    {
                        var res = Utils.APIUtils.CallAPI($"menu/search?term=&page=1&page_size=100", "", Token, out _, out _);

                        var obj = JToken.Parse(res);
                        lst_menu = obj["data"].ToObject<List<Menu>>();
                    }
                    catch (Exception)
                    {
                        lst_menu = new List<Menu>();
                    }
                    cache.Set<List<Menu>>(my_menu, lst_menu, lst_menu.Count == 0 ? DateTimeOffset.Now.AddMinutes(5) : DateTimeOffset.Now.AddDays(10));
                }
            }
            return lst_menu;
        }

        public override ViewResult View()
        {
            ViewBag.menu = GetMenu();
            ViewBag.token = Token; ViewBag.team = _team;
            return base.View();
        }

        public override ViewResult View(string viewName)
        {
            ViewBag.menu = GetMenu();
            ViewBag.token = Token; ViewBag.team = _team;
            return base.View(viewName);
        }

        public override ViewResult View(object model)
        {
            ViewBag.menu = GetMenu();
            ViewBag.token = Token; ViewBag.team = _team;
            return base.View(model);
        }

        public override ViewResult View(string viewName, object model)
        {
            ViewBag.menu = GetMenu();
            ViewBag.token = Token; ViewBag.team = _team;
            return base.View(viewName, model);
        }

        protected List<UngVien> SelectAllUngVien()
        {
            string res = "";
            List<UngVien> lst_uv = new List<UngVien>() { };
            try
            {
                res = Utils.APIUtils.CallAPI($"ungvien/allungvien", "", Token, out bool success, out string msg);
                var obj = JToken.Parse(res);
                foreach (var ungvien in obj["data"].ToObject<List<UngVien>>())
                {
                    lst_uv.Add(ungvien);
                }
            }
            catch (Exception) { }
            return lst_uv;
        }    
   
        protected List<CongTy> SelectAllCongTy()
        {
            string res = "";
            List<CongTy> lst_uv = new List<CongTy>() { };
            BaseGetAll req = new BaseGetAll();
            req.term = "";
            req.page_index = 1;
            req.page_size = 9999;
            string req_all_ct = JsonConvert.SerializeObject(req);
            try
            {
                res = Utils.APIUtils.CallAPI($"congty/all", req_all_ct, Token, out bool success, out string msg, "POST");
                var obj = JToken.Parse(res);
                foreach (var congty in obj["data"].ToObject<List<CongTy>>())
                {
                    lst_uv.Add(congty);
                }
            }
            catch (Exception) { }
            return lst_uv;
        }

        protected List<UngVien> SelectAllUngVienByUserJob(string id_user_job)
        {
            string res = "";
            List<UngVien> lst_uv = new List<UngVien>() { };
            try
            {
                res = Utils.APIUtils.CallAPI($"ungvien/allungvienbyuser?id_user_job={id_user_job}", "", Token, out bool success, out string msg);
                var obj = JToken.Parse(res);
                foreach (var ungvien in obj["data"].ToObject<List<UngVien>>())
                {
                    lst_uv.Add(ungvien);
                }
            }
            catch (Exception) { }
            return lst_uv;
        }

        /// <summary>
        /// Hàm đang sai. -> Cần tìm trong bảng UserJob sau đó lấy được danh sách các job đang phải làm -> Trả ra danh sách này để người dùng chọn. Nếu tìm all_job ko có điều kiện thì khi
        /// sửa NoteUngVienJob sẽ được phép chọn tất cả JOB khác mà người này ko có quyền
        /// </summary>
        /// <returns></returns>
        protected List<Job> SelectAllJob()
        {
            string res = "";
            List<Job> lst_job = new List<Job>() { };

            string res_user_job = "";
            try
            {
                res_user_job = Utils.APIUtils.CallAPI($"job/search?id_job=&value_filter=1&id_ung_vien=&ngay_nhan_job_from=0&ngay_nhan_job_to=0&thuoc_tinh=&thuoc_tinh_rieng=&page=1&page_size=9999", "", Token, out bool success, out string msg);
                var obj = JToken.Parse(res_user_job);
                foreach (var user_job in obj["data"].ToObject<List<dynamic>>())
                {
                    lst_job.Add(new Job()
                    {
                        id_job = user_job.id_job,
                        chuc_danh = user_job.chuc_danh,
                        id_auto = user_job.id_auto
                    });
                }
            }
            catch (Exception) { }
            return lst_job;
        }

        protected List<TinhThanh> SelectAllTinhThanh()
        {
            string res = "";
            List<TinhThanh> lst_tt = new List<TinhThanh>() { };
            try
            {
                res = Utils.APIUtils.CallAPI($"tinhthanh/alltinhthanh", "", Token, out bool success, out string msg);
                var obj = JToken.Parse(res);
                foreach (var tinhthanh in obj["data"].ToObject<List<TinhThanh>>())
                {
                    lst_tt.Add(tinhthanh);
                }
            }
            catch (Exception) { }
            return lst_tt;
        }

        protected List<GroupUser> SelectAllTeam()
        {
            string res = "";
            List<GroupUser> lst_tt = new List<GroupUser>() { };
            try
            {
                res = Utils.APIUtils.CallAPI($"groupuser/allgroupuser", "", Token, out bool success, out string msg);
                var obj = JToken.Parse(res);
                foreach (var team in obj["data"].ToObject<List<GroupUser>>())
                {
                    lst_tt.Add(team);
                }
            }
            catch (Exception) { }
            return lst_tt;
        }

        protected string BuildAllUserCheckBox()
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                var res = Utils.APIUtils.CallAPI($"user/all", "", Token, out bool success, out string msg);

                if (success && !string.IsNullOrEmpty(res))
                {
                    var obj = JToken.Parse(res);
                    stb.Append("<div class='form-group'><ul class='check-box row'>");
                    foreach (var user in obj["data"].ToObject<List<User>>())
                    {
                        stb.AppendFormat("<li class='col-md-4'><input type='checkbox' class='user' name='user_shared' value='{0}'> {0} ({1})</li>", user.user_name, user.full_name);
                    }
                    stb.Append("</ul></div>");
                }
            }
            catch (Exception)
            {
            }
            return stb.ToString();
        }

        protected string BuildAllUserInTeamCheckBox()
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                var res = Utils.APIUtils.CallAPI($"user/team", "", Token, out bool success, out string msg);

                if (success && !string.IsNullOrEmpty(res))
                {
                    var obj = JToken.Parse(res);
                    stb.Append("<div class='form-group'><ul class='check-box row'>");
                    foreach (var user in obj["data"].ToObject<List<User>>())
                    {
                        stb.AppendFormat("<li class='col-md-4'><input type='checkbox' class='user' name='user_shared' value='{0}'> {0} ({1})</li>", user.user_name, user.full_name);
                    }
                    stb.Append("</ul></div>");
                }
            }
            catch (Exception)
            {
            }
            return stb.ToString();
        }

        protected List<ThuocTinh> GetThuocTinh(LoaiThuocTinh loai, int type)
        {
            try
            {
                string end_point = $"thuoctinh/loai?id={(int)loai}";
                if (type == -1)
                {
                    end_point = $"{end_point}&type={type}";
                }
                var res = Utils.APIUtils.CallAPI(end_point, "", Token, out bool success, out string msg);
                if (success && !string.IsNullOrEmpty(res))
                {
                    var obj = JToken.Parse(res);
                    return obj["data"].ToObject<List<ThuocTinh>>();
                }
            }
            catch (Exception)
            {

            }
            return new List<ThuocTinh>();
        }

        /// <summary>
        /// 21-04-2020
        /// </summary>
        /// <param name="_thuoc_tinh"></param>
        /// <returns></returns>
        protected string BuildBoxThuocTinh(List<ThuocTinh> _thuoc_tinh)
        {
            StringBuilder stb = new StringBuilder();

            var grp_by_nhom = _thuoc_tinh.GroupBy(x => x.nhom);

            foreach (var grp in grp_by_nhom.OrderBy(x => x.Key))
            {
                if (grp.Key == 0)//Nhóm này cho phép chọn multi
                {
                    stb.AppendFormat("<div class='form-group'><fieldset class='field-thuoc-tinh'><legend><h5>Nhóm {0}</h5></legend><ul class='check-box row'>", grp.Key);
                    foreach (var item in grp)
                    {
                        //stb.AppendFormat("<li class='col-md-4'><input type='checkbox' name='thuoc_tinh' data-type='{2}' value='{0}'> {1} </li>", item.gia_tri, item.ten, (int)item.type);
                        stb.AppendFormat("<li class='col-md-4'><div class='custom-control custom-checkbox'><input type='checkbox' name='thuoc_tinh' data-type='{2}' value='{0}' class='custom-control-input' id='ocustomControlInline{3}_{2}_{4}'><label class='custom-control-label' for='ocustomControlInline{3}_{2}_{4}'>{1}</label></div></li>", item.gia_tri, item.ten, (int)item.type, item.gia_tri, item.loai);
                    }
                    stb.Append("</ul></fieldset></div>");
                }
                else
                {
                    stb.AppendFormat("<div class='form-group'><fieldset class='field-thuoc-tinh'><legend><h5>Nhóm {0}</h5></legend><ul class='check-box row'>", grp.Key);
                    foreach (var item in grp)
                    {
                        //stb.AppendFormat("<li class='col-md-4'><input type='radio' name='thuoc_tinh_nhom_{3}' data-type='{2}' value='{0}'> {1} </li>", item.gia_tri, item.ten, (int)item.type, grp.Key);
                        stb.AppendFormat("<li class='col-md-4'><div class='radio'><input id='radio-{4}_{5}_{6}' name='thuoc_tinh_nhom_{3}' value='{0}' data-type='{2}' type='radio'><label for='radio-{4}_{5}_{6}' class='radio-label'>{1}</label></div> </li>", item.gia_tri, item.ten, (int)item.type, grp.Key, item.gia_tri, (int)item.type, item.loai);
                    }
                    stb.Append("</ul></fieldset></div>");
                }
            }
            return stb.ToString();
        }

        protected string BuildBoxThuocTinhNew(List<ThuocTinh> _thuoc_tinh)
        {
            StringBuilder stb = new StringBuilder();

            var grp_by_nhom = _thuoc_tinh.GroupBy(x => x.nhom);

            foreach (var grp in grp_by_nhom.OrderBy(x => x.Key))
            {
                if (grp.Key == 0)//Nhóm này cho phép chọn multi
                {
                    stb.AppendFormat("<div class='form-group'><ul class='check-box'>");
                    stb.AppendFormat("<li>Nhóm {0}:</li>", grp.Key);
                    foreach (var item in grp)
                    {

                        stb.AppendFormat("<li><div class='custom-control custom-checkbox'><input type='checkbox' name='thuoc_tinh' data-type='{2}' value='{0}' class='custom-control-input' id='ncustomControlInline{3}_{2}_{4}'><label class='custom-control-label' for='ncustomControlInline{3}_{2}_{4}'>{1}</label></div></li>", item.gia_tri, item.ten, (int)item.type, item.gia_tri, item.loai);
                    }
                    stb.Append("</ul></div>");
                }
                else
                {
                    stb.AppendFormat("<ul class='check-box'>");
                    stb.AppendFormat("<li>Nhóm {0}:</li>", grp.Key);
                    foreach (var item in grp)
                    {
                        stb.AppendFormat("<li><div class='custom-control custom-checkbox'><input type='checkbox' name='thuoc_tinh_nhom_{3}' value='{0}' id='check-box-{4}_{5}_{6}' data-type='{2}' class='custom-control-input'><label for='check-box-{4}_{5}_{6}' class='custom-control-label'>{1}</label></div> </li>", item.gia_tri, item.ten, (int)item.type, grp.Key, item.gia_tri, (int)item.type, item.loai);
                    }
                    stb.Append("</ul>");
                }
            }
            return stb.ToString();
        }
        protected string BuildBoxThuocTinhNew2(List<ThuocTinh> _thuoc_tinh)
        {
            StringBuilder stb = new StringBuilder();

            var grp_by_nhom = _thuoc_tinh.GroupBy(x => x.nhom);

            foreach (var grp in grp_by_nhom.OrderBy(x => x.Key))
            {
                if (grp.Key == 0)//Nhóm này cho phép chọn multi
                {
                    stb.AppendFormat("<div class='form-group'><ul class='check-box'>");
                    stb.AppendFormat("<li><b>Nhóm {0}:</b></li>", grp.Key);
                    foreach (var item in grp)
                    {

                        stb.AppendFormat("<li><div class='custom-control custom-checkbox'><input type='checkbox' name='thuoc_tinh' data-type='{2}' value='{0}' class='custom-control-input' id='ncustomControlInline2{3}_{2}_{4}'><label class='custom-control-label' for='ncustomControlInline2{3}_{2}_{4}'>{1}</label></div></li>", item.gia_tri, item.ten, (int)item.type, item.gia_tri, item.loai);
                    }
                    stb.Append("</ul></div>");
                }
                else
                {
                    stb.AppendFormat("<ul class='check-box'>");
                    stb.AppendFormat("<li><b>Nhóm {0}:</b></li>", grp.Key);
                    foreach (var item in grp)
                    {
                        stb.AppendFormat("<li><div class='custom-control custom-checkbox'><input type='checkbox' name='thuoc_tinh_nhom_{3}' value='{0}' id='check-box2-{4}_{5}_{6}' data-type='{2}' class='custom-control-input'><label for='check-box2-{4}_{5}_{6}' class='custom-control-label'>{1}</label></div> </li>", item.gia_tri, item.ten, (int)item.type, grp.Key, item.gia_tri, (int)item.type, item.loai);
                    }
                    stb.Append("</ul>");
                }
            }
            return stb.ToString();
        }
        public static string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }

        public static string AllOfEnum()
        {
            List<string> name = new List<string>();
            name.Add(GetDisplayName(ThuocTinhType.SHARED));
            name.Add(GetDisplayName(ThuocTinhType.PRIVATE));
            return name.ToString();
        }
        public UserSetting AppSetting()
        {
            //appsetting
            try
            {
                string end_point = $"user/appsetting";

                var res = Utils.APIUtils.CallAPI(end_point, "", Token, out bool success, out string msg);
                if (success && !string.IsNullOrEmpty(res))
                {
                    var obj = JToken.Parse(res);
                    return obj["data"].ToObject<UserSetting>();
                }
            }
            catch (Exception)
            {
            }
            return new UserSetting();
        }

    }
}