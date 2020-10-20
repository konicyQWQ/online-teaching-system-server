using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTS_API.Models;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("User")]
        public async Task<dynamic> GetUserAsync(string id)
        {
            //头像，姓名
            return null;
        }
    }
}
