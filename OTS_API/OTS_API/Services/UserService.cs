using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;
using MySql.Data.MySqlClient;
using OTS_API.Models;
using OTS_API.Common;
using OTS_API.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace OTS_API.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> logger;
        private readonly OTSDbContext dbContext;

        public UserService(ILogger<UserService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        /// <summary> 
        /// 验证用户名与密码，若用户不存在或密码错误，请返回Unknown(3)
        /// </summary> 
        /// <param name="id">用户名</param> 
        /// <param name="password">密码，已加密（MD5）</param> 
        /// <returns>用户角色</returns> 
        public async Task<User> AuthenticateAsync(string id, string password)
        {
            var user = await dbContext.User.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                throw new Exception("User Not Found!");
            }
            if(user.Password != password)
            {
                throw new Exception("Wrong Password!");
            }
            return user;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>注册结果</returns>
        public async Task RegistAsync(User user)
        {
            await dbContext.User.AddAsync(user);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息（去密码）</returns>
        public Task<User> GetUserInfoAsync(string id)
        {
            return Task.Run(() =>
            {
                return new User();
            });
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns>用户信息列表（去密码）</returns>
        public Task<List<User>> GetAllUserAsync()
        {
            return Task.Run(() =>
            {
                return new List<User>();
            });
        }
    }
}