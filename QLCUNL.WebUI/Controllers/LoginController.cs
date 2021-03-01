using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace QLCUNL.WebUI.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogMeIn([FromBody] object value)
        {
            using (WebClient wc = new WebClient())
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("user", "thuanld");
                nvc.Add("pass", "123");

                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var res = wc.UploadValues("/api/example", "POST", nvc);
                string data = Encoding.ASCII.GetString(res);
                var obj = JToken.Parse(data);
                if (obj != null)
                {

                }

            }
            return View();
        }
    }
}