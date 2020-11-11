using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data;
using MySql.Data.MySqlClient;
using OTS_API.Models;
using OTS_API.Utilities;
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
            var user = await dbContext.Users.FindAsync(id);
            if(user == null)
            {
                logger.LogError("User: " + id + " Not Found!");
                throw new Exception("User Not Found!");
            }
            if(user.Password != password)
            {
                logger.LogError("User: " + id + "Wrong Password!");
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
            try
            {
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("学号已存在");
            }
            
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息（去密码）</returns>
        public async Task<User> GetUserInfoAsync(string id)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(id);
                user.Password = null;
                return user;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return null;
            }
        }

        public async Task UpdateUserInfoAsync(User userInfo)
        {
            var userToUpdate = await dbContext.Users.FindAsync(userInfo.Id);
            if(userToUpdate == null)
            {
                throw new Exception("User Not Found");
            }
            userToUpdate.Gender = userInfo.Gender;
            userToUpdate.Phone = userInfo.Phone;
            userToUpdate.Email = userInfo.Email;
            userToUpdate.AvatarId = userInfo.AvatarId;
            userToUpdate.Introduction = userInfo.Introduction;
            try
            {
                dbContext.Users.Update(userToUpdate);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Falied!");
            }
        }

        public async Task<List<User>> SearchUserAsync(string keyword, int limit, UserRole role)
        {
            try
            {
                var list = await dbContext.Users.Where(user => user.Role == role && user.Name.Contains(keyword)).ToListAsync();
                var res = list.Take(limit).ToList();
                foreach (var t in res)
                {
                    t.Introduction = null;
                    t.Password = null;
                }
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task ResetPasswordAsync(string userID, string oldPassword, string newPassword)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userID);
                if(user.Password != oldPassword)
                {
                    throw new Exception("密码错误!");
                }
                user.Password = newPassword;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        public async Task ResetPasswordAsync(string userID, string newPassword)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(userID);
                user.Password = newPassword;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
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