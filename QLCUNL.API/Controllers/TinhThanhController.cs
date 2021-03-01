using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TinhThanhController : APIBase
    {
        [HttpGet]
        [Route("view")]
        [AllowAnonymous]
        public IActionResult Get(string id)
        {

            QLCUNL.BL.TinhThanhBL.UpdateSTT("24", -63);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("58", -62);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("27", -61);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("15", -60);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("9", -59);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("56", -58);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("6", -57);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("54", -56);
            QLCUNL.BL.TinhThanhBL.UpdateSTT("62", -55);

            var tinhthanh = QLCUNL.BL.TinhThanhBL.GetById(id);

            return Ok(new DataResponse() { data = tinhthanh, success = tinhthanh != null, msg = "" });
        }

        // POST: api/tinhthanh
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var tinhthanh = Newtonsoft.Json.JsonConvert.DeserializeObject<TinhThanh>(value.ToString());
                res.success = QLCUNL.BL.TinhThanhBL.Index(tinhthanh);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/tinhthanh/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var tinhthanh = Newtonsoft.Json.JsonConvert.DeserializeObject<TinhThanh>(value.ToString());
                tinhthanh.id_tinh = id;
                res.success = QLCUNL.BL.TinhThanhBL.Update(tinhthanh);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
        }

        [HttpGet]
        [Route("alltinhthanh")]
        public IActionResult GetAllTinhThanh()
        {

            List<TinhThanh> data = new List<TinhThanh>();
            long total_recs = 0;
            string msg = "";

            data = QLCUNL.BL.TinhThanhBL.Search(1, out total_recs, out msg, 9999);
            data = data.OrderBy(x => x.stt).ToList();
            return Ok(new DataResponsePaging() { data = data, total = total_recs, success = data != null, msg = msg });
        }
    }
}
