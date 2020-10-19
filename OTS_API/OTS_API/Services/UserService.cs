using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;
using MySql.Data.MySqlClient;
using OTS_API.Models;

namespace OTS_API.Services
{
    public class UserService : DBService
    {
        private readonly ILogger<UserService> logger;

        public UserService(ILogger<UserService> logger, ILogger<DBService> logger1)
            : base(logger1)
        {
            this.logger = logger;
        }

        /// <summary> 
        /// 验证用户名与密码，若用户不存在或密码错误，请返回Unknown(3)
        /// </summary> 
        /// <param name="name">用户名</param> 
        /// <param name="password">密码，已加密（MD5）</param> 
        /// <returns>用户角色</returns> 
        public Task<User> AuthenticateAsync(string name, string password)
        {
            return Task.Run(() =>
            {
                return new User();
            });
        }

        public Task<bool> RegistAsync(User user)
        {
            return Task.Run(() =>
            {
                return true;
            });
        }

        public Task<List<User>> GetAllUserAsync()
        {
            return Task.Run(() =>
            {
                return new List<User>();
            });
        }
    }
}