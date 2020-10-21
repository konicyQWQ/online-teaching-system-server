using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTS_API.Models;
using OTS_API.Services;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly UserService userService;
        private readonly ILogger<InfoController> logger;

        public InfoController(UserService userService, ILogger<InfoController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("User")]
        public async Task<dynamic> GetUserAsync(string id)
        {
            var user = await userService.GetUserInfoAsync(id);
            //头像，姓名
            return new { Name = user.Name, AvatarID = user.AvatarID };
        }
    }
}
