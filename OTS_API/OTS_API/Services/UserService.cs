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
        /// <param name="id">用户名</param> 
        /// <param name="password">密码，已加密（MD5）</param> 
        /// <returns>用户角色</returns> 
        public Task<User> AuthenticateAsync(string id, string password)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var cmd = new MySqlCommand("select * from user where id = \"" + id + "\"", this.sqlConnection);
                    using var reader = await cmd.ExecuteReaderAsync();
                    if(!await reader.ReadAsync())
                    {
                        throw new Exception("User Not Found!");
                    }
                    if (!password.Equals(reader.GetString(1)))
                    {
                        throw new Exception("Incorrect Password");
                    }
                    return new User {
                        ID = reader.GetString(0),
                        Name = reader.GetString(2),
                        Role = (UserRole)reader.GetInt16(7)
                    };
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return new User
                    {
                        Role = UserRole.Unknown,
                        ID = e.Message
                    };
                }
            });
        }

        public Task<bool> RegistAsync(User user)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var cmd = new MySqlCommand("insert into user values(\"" + user.ID + "\",\"" + user.Password + "\",\"" + user.Name + "\"," + (int)user.Gender + "," + user.Grade + ",\"" + user.Phone + "\",\"" + user.Email + "\"," + (int)user.Role + ")", this.sqlConnection);
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return false;
                }
            });
        }

        public Task<User> GetUserInfoAsync(string id)
        {
            return Task.Run(() =>
            {
                return new User();
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