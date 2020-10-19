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
        private readonly UserService userService;
        private readonly TokenService tokenService;
        private readonly ILogger<UserService> logger;

        public AuthenticationController(UserService authenticateService, TokenService tokenService, ILogger<UserService> logger)
        {
            this.userService = authenticateService;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        //传入用户名密码（已加密），返回登录结果（身份）、token
        //
        [HttpPost]
        public async Task<dynamic> OnPostAsync([FromForm]string name, [FromForm]string password)
        {
            var token = "ERROR";
            var user = await this.userService.AuthenticateAsync(name, password);
            if(user.Role != UserRole.Unknown)
            {
                token = await tokenService.SetToken(user.ID);
            }

            return new { Role = user.Role, Token = token };
        }

        [HttpPost]
        [Route("Regist")]
        public async Task<dynamic> OnRegistAysnc([FromForm] User user)
        {
            var res = await userService.RegistAsync(user);
            var token = "Error";
            if (res)
            {
                token = await tokenService.SetToken(user.ID);
            }
            return new { Res = res, Token = token };
        }

        [HttpGet]
        public async Task<List<User>> OnGetAsync()
        {
            return await userService.GetAllUserAsync();
        }
    }
}
