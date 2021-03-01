using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLCUNL.WebUI.Controllers
{
    public class UserController : BaseController
    {
        // GET: /<controller>/
        public UserController(IMemoryCache _cache) : base(_cache) { }
        public IActionResult All()
        {
            ViewBag.all_team = SelectAllTeam();
            return View();
        }
        public IActionResult Add()
        {
            ViewBag.list_team = SelectAllTeam();
            ViewBag.default_setting = AppSetting();
            return View("Add");
        }
        public IActionResult Edit(string id)
        {
            ViewBag.list_team = SelectAllTeam();
            ViewBag.id = id;
            ViewBag.default_setting = AppSetting();
            return View("Edit");
        }
        [HttpGet]
        [Route("user/detail/{id}")]
        public IActionResult Detail(string id)
        {
            ViewBag.id = id;
            return View("Detail");
        }

        public IActionResult UserChange(string id)
        {
            ViewBag.id = id;
            return View("UserChange");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.ReturnUrl = HttpContext.Request.Query["ReturnUrl"].ToString();
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromForm] object value)
        {
            string msg = "";
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                user = HttpContext.Request.Form["username"].ToString(),
                pass = HttpContext.Request.Form["password"].ToString()
            });
            try
            {
                var res = Utils.APIUtils.CallAPI("user/login", json, string.Empty, out bool success, out msg, "POST");


                var obj = JToken.Parse(res);
                if (obj != null)
                {
                    if (obj["success"].ToObject<bool>())
                    {
                        cache.Remove(my_menu);
                        var user = obj["data"].ToObject<QLCUNL.Models.User>();

                        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        identity.AddClaim(new Claim(ClaimTypes.Name, user.user_name));
                        identity.AddClaim(new Claim(ClaimTypes.GivenName, !string.IsNullOrEmpty(user.full_name) ? user.full_name : ""));
                        identity.AddClaim(new Claim(ClaimTypes.Email, !string.IsNullOrEmpty(user.email) ? user.email : ""));
                        identity.AddClaim(new Claim("token", obj["token"].ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.GroupSid, obj["data"]["id_team"].ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Sid, obj["data"]["app_id"].ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.UserData, obj["data"]["setting"].ToString()));
                        if (obj["data"]["roles"] != null)
                            foreach (var role in obj["data"]["roles"].ToObject<List<string>>())
                            {
                                identity.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                            }
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddYears(1),
                            AllowRefresh = true
                        });
                        string ret = HttpContext.Request.Form["ReturnUrl"].ToString();
                        if (!string.IsNullOrEmpty(ret))
                            return Redirect(ret);
                        else
                            return Redirect("/");
                    }
                    else
                    {
                        ViewBag.error = obj["msg"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = $"{ex.Message} detail: {msg}";
            }

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            cache.Remove(my_menu);
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
