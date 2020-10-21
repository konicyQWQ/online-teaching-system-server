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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> RegistAsync(User user)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var cmd = this.sqlConnection.CreateCommand();
                    
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "insert into user values(@id, @password, @name, @gender, @grade, @phone, @email, @role, @avatar_id)";
                    
                    cmd.Parameters.Add("@id", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@gender", MySqlDbType.Int16);
                    cmd.Parameters.Add("@grade", MySqlDbType.Int16);
                    cmd.Parameters.Add("@phone", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@role", MySqlDbType.Int16);
                    cmd.Parameters.Add("@avatar_id", MySqlDbType.Int32);

                    cmd.Parameters["@id"].Value = user.ID;
                    cmd.Parameters["@password"].Value = user.Password;
                    cmd.Parameters["@name"].Value = user.Name;
                    cmd.Parameters["@gender"].Value = (int)user.Gender;
                    cmd.Parameters["@grade"].Value = user.Grade;
                    cmd.Parameters["@phone"].Value = user.Phone;
                    cmd.Parameters["@email"].Value = user.Email;
                    cmd.Parameters["@role"].Value = (int)user.Role;
                    cmd.Parameters["avatar_id"].Value = user.AvatarID;

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