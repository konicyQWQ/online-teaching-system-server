﻿using System;
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
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        private readonly TokenService tokenService;
        private readonly PasswordRetrieveService passwordRetrieveService;
        private readonly ILogger<UserService> logger;

        public UserController(UserService authenticateService, TokenService tokenService, PasswordRetrieveService passwordRetrieveService, ILogger<UserService> logger)
        {
            this.userService = authenticateService;
            this.tokenService = tokenService;
            this.passwordRetrieveService = passwordRetrieveService;
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
            try
            {
                var user = await this.userService.AuthenticateAsync(id, password);
                var token = await tokenService.AddTokenAsync(new Token(user.Id, user.Role, 24));

                return new { Role = user.Role, Token = token };
            }
            catch (Exception e)
            {
                return new { Role = UserRole.Unknown, Token = e.Message };
            }
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
            try
            {
                await userService.RegistAsync(user);
                var token = await tokenService.AddTokenAsync(new Token(user.Id, user.Role, 24));

                return new { Res = true, Token = token };
            }
            catch (Exception e)
            {
                return new { Res = false, Token = e.Message };
            }
        }

        /// <summary>
        /// 用户修改信息
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="token">用户token</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("ChangeInfo")]
        public async Task<dynamic> OnChangeInfoAsync([FromForm] User user, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if(t == null)
                {
                    throw new Exception("Invalid Token!");
                }
                await userService.UpdateUserInfoAsync(user);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>用户信息（去密码）,若token失效，返回错误信息</returns>
        [HttpGet]
        [Route("User")]
        public async Task<object> GetUserAsync(string token)
        {
            var t = await tokenService.GetTokenAsync(token);
            if (t == null)
                return new { Error = "Token is Invalid" };//TBD
            var user = await userService.GetUserInfoAsync(t.UserID);
            return user;
        }

        /// <summary>
        /// 批量获取用户列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="limit"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> OnGetAsync(string keyword, int limit, UserRole role)
        {
            try
            {
                if(keyword == null)
                {
                    throw new Exception("keyword cannot be null!");
                }
                var resList = await userService.SearchUserAsync(keyword, limit, role);
                return new { Res = true, Count = resList.Count, resList = resList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
            
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<dynamic> OnResetPasswordAsync([FromForm] string token, [FromForm] string oldPassword, [FromForm] string newPassword)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                await userService.ResetPasswordAsync(t.UserID, oldPassword, newPassword);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("RetrievePassword")]
        public async Task<dynamic> OnRetrievePassword([FromForm] string userID, [FromForm] string email)
        {
            try
            {
                var userInfo = await userService.GetUserInfoAsync(userID);
                var token = await passwordRetrieveService.AddSTokenAsync(userInfo, email);
                return new { Res = true, Roken = token};
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("VerifyEmail")]
        public async Task<dynamic> OnVerifyEmailAsync([FromForm] string token, [FromForm] string code)
        {
            try
            {
                await passwordRetrieveService.VerifyAsync(token, code);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("PasswordReset")]
        public async Task<dynamic> OnPasswordResetAsync([FromForm] string token, [FromForm] string newPassword)
        {
            try
            {
                var userID = await passwordRetrieveService.ResetVerifyAsync(token);
                await userService.ResetPasswordAsync(userID, newPassword);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
