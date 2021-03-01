using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using QLCUNL.BL;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QLCUNL.API.Controllers
{
    //[ApiVersion("1.0")]
    [ApiController]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [Authorize(Policy = "SameIpPolicy")]
    public class APIBase : ControllerBase, IActionFilter
    {
        private bool _is_sys_admin;
        private bool _is_super_user;
        private bool _is_app_admin;
        public bool is_sys_admin
        {
            get
            {
                try
                {
                    if (User != null && User.Identity.IsAuthenticated)
                        _is_sys_admin = User.IsInRole(QLCUNL.Models.Role.SYS_ADMIN.ToString());
                }
                catch (Exception)
                {
                }
                return _is_sys_admin;
            }
        }
        public bool is_app_admin
        {
            get
            {
                try
                {
                    if (User != null && User.Identity.IsAuthenticated)
                        _is_app_admin = User.IsInRole(QLCUNL.Models.Role.APP_ADMIN.ToString());
                }
                catch (Exception)
                {
                }
                return _is_app_admin;
            }
        }

        public bool is_super_user
        {
            get
            {
                try
                {
                    if (User != null && User.Identity.IsAuthenticated)
                        _is_super_user = User.IsInRole(QLCUNL.Models.Role.SUPER_USER.ToString());
                }
                catch (Exception)
                {
                }
                return _is_super_user;
            }
        }

        protected void SetMetaData(dynamic obj, bool is_update)
        {
            if (is_update)
            {
                obj.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                obj.nguoi_sua = user;
            }
            else
            {
                obj.app_id = app_id;
                obj.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                obj.nguoi_tao = user;
                obj.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                obj.nguoi_sua = user;
            }
        }
        protected bool IsInAppId(dynamic obj)
        {
            try
            {
                string _app_id = obj.app_id;
                return _app_id == app_id;
            }
            catch (Exception)
            {

                
            }
            return false;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                user = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
                try
                {
                    var claim_app_id = User.FindFirst(x => x.Type == "app_id");
                    if (claim_app_id != null)
                        app_id = claim_app_id.Value;
                }
                catch
                {

                }
                try
                {
                    var claim_team = User.FindFirst(x => x.Type == "team");
                    if (claim_team != null)
                        group = Convert.ToInt32(claim_team.Value);
                }
                catch
                {

                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
        protected string user = "";
        protected string app_id = "";
        protected int page_size = 3;
        protected int group = 1;
        public List<int> ParseThuocTinhRieng(object o)
        {
            List<int> thuoc_tinh_rieng = new List<int>();
            try
            {
                var obj = JToken.Parse(o.ToString());
                if (obj != null && obj["thuoc_tinh_rieng"] != null)
                {
                    thuoc_tinh_rieng = obj["thuoc_tinh_rieng"].ToObject<List<int>>();
                }
            }
            catch (Exception)
            {

            }
            return thuoc_tinh_rieng;
        }
        public void UpsertThuocTinhRieng(object o, string id_obj, LoaiThuocTinh loai_obj)
        {
            List<int> thuoc_tinh_rieng;
            try
            {
                var obj = JToken.Parse(o.ToString());
                if (obj != null && obj["thuoc_tinh_rieng"] != null)
                {
                    thuoc_tinh_rieng = obj["thuoc_tinh_rieng"].ToObject<List<int>>();

                    var data = new ThuocTinhDuLieu();
                    data.id_obj = id_obj;
                    data.loai_obj = loai_obj;
                    data.thuoc_tinh = thuoc_tinh_rieng;
                    data.trang_thai = TrangThai.ACTIVE;
                    SetMetaData(data, false);
                    QLCUNL.BL.ThuocTinhDuLieuBL.Index(data);


                }
            }
            catch (Exception)
            {

            }
        }

    }
}
