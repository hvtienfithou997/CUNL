using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API.Controllers
{
    interface IAPIBase
    {
        IActionResult Get(string id);
        bool Delete(string id);        
        IActionResult Shared(string id);
    }
}
