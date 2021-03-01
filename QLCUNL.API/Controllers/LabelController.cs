using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        [HttpPost]
        [Route("get/all")]
        public IActionResult All([FromBody] BaseGetAll res)
        {
            List<Label> label = QLCUNL.BL.LabelBL.GetAll(res.page_index, res.page_size).ToList();

            return Ok(new DataResponse() { data = label, success = label != null, msg = "" });
        }

        //GET: api/label
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var label = QLCUNL.BL.LabelBL.GetById(id);
            return Ok(new DataResponse() { data = label, success = label != null, msg = "" });
        }

        //POST: api/label
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var label = Newtonsoft.Json.JsonConvert.DeserializeObject<Label>(value.ToString());
                res.success = QLCUNL.BL.LabelBL.Index(label);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/label/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var label = Newtonsoft.Json.JsonConvert.DeserializeObject<Label>(value.ToString());
                label.id_label = id;
                res.success = QLCUNL.BL.LabelBL.Update(label);
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
    }
}