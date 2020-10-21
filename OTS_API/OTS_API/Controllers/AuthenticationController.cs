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
    /// <summary>
    /// 用户验证、带token请求等
    /// </summary>
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

        /// <summary>
        /// 用户身份验证接口
        /// </summary>
        /// <param name="id">学号</param>
        /// <param name="password">密码，已加密（MD5）</param>
        /// <returns>用户角色，token；若用户不存在或密码错误，角色返回Unknown</returns>
        [HttpPost]
        public async Task<dynamic> OnPostAsync([FromForm]string id, [FromForm]string password)
        {
            var token = "ERROR";
            var user = await this.userService.AuthenticateAsync(id, password);
            if(user.Role != UserRole.Unknown)
            {
                token = await tokenService.AddTokenAsync(new Token(user.ID, user.Role, 24));
            }
            else
            {
                token = user.ID;
            }

            return new { Role = user.Role, Token = token };
        }

        /// <summary>
        /// 用户注册接口
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>注册结果，token；若注册失败（用户名已存在），res返回false</returns>
        [HttpPost]
        [Route("Regist")]
        public async Task<dynamic> OnRegistAysnc([FromForm] User user)
        {
            var res = await userService.RegistAsync(user);
            var token = "Error";
            if (res)
            {
                token = await tokenService.AddTokenAsync(new Token(user.ID, user.Role, 24));
            }
            else
            {
                token = "学号已存在";
            }
            return new { Res = res, Token = token };
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>用户信息（去密码）,若token失效，返回？</returns>
        [HttpGet]
        [Route("User")]
        public async Task<User> GetUserAsync(string token)
        {
            var t = await tokenService.GetTokenAsync(token);
            if (t == null)
                return null;//TBD
            var user = await userService.GetUserInfoAsync(t.UserID);
            return user;
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        [HttpGet]
        public async Task<List<User>> OnGetAsync()
        {
            return await userService.GetAllUserAsync();
        }
    }
}
