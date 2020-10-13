using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTS_API.Models;
using OTS_API.Services;

namespace OTS_API.Controllers
{
    /*
     * 登录验证
     */
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowCors")]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticateService authenticateService;
        private readonly ILogger<AuthenticateService> logger;

        public AuthenticationController(AuthenticateService authenticateService, ILogger<AuthenticateService> logger)
        {
            this.authenticateService = authenticateService;
            this.logger = logger;
        }

        //传入用户名密码（已加密），返回登录结果（身份）、token
        //
        [HttpPost]
        public async Task<UserRole> OnPostAsync([FromForm]string name, [FromForm]string password)
        {
            this.logger.LogInformation("Request Recived");
            return await this.authenticateService.AuthenticateAsync(name, password);
        }

        [HttpPost]
        [Route("Regist")]
        public bool OnRegistAysnc([FromForm] User user)
        {
            if (user.Name == "Jack")
                return true;
            return false;
        }

        [HttpGet]
        public List<User> OnGet()
        {
            return this.authenticateService.Users;
        }
    }
}
