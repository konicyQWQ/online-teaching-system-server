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
using Newtonsoft.Json;

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
                throw new Exception("用户名不存在");
            }
            if(user.Password != password)
            {
                logger.LogError("User: " + id + "Wrong Password!");
                throw new Exception("密码错误");
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
                if(user == null)
                {
                    throw new Exception("User Not Found!");
                }
                user.Password = null;
                return user;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateUserInfoAsync(User userInfo)
        {
            var userToUpdate = await dbContext.Users.FindAsync(userInfo.Id);
            if(userToUpdate == null)
            {
                throw new Exception("User Not Found");
            }
            userToUpdate.Name = userInfo.Name;
            userToUpdate.Gender = userInfo.Gender;
            userToUpdate.Phone = userInfo.Phone;
            userToUpdate.Email = userInfo.Email;
            userToUpdate.AvatarId = userInfo.AvatarId;
            userToUpdate.Introduction = userInfo.Introduction;
            userToUpdate.Role = userInfo.Role;
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

        public async Task RemoveUserAsync(string userID)
        {
            try
            {
                var userToRemove = await dbContext.Users.FindAsync(userID);
                dbContext.Users.Remove(userToRemove);
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
                var list = await dbContext.Users.Where(user => user.Role == role && user.Name.Contains(keyword)).Take(limit).ToListAsync();
                var res = list.ToList();
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
        public async Task<UserResList> GetUsersAsync(int start, int limit, string keyword, List<UserRole> roles)
        {
            try
            {
                IQueryable<User> q = dbContext.Users;
                if(keyword != null)
                {
                    q = q.Where(u => u.Name.Contains(keyword) || u.Id.Contains(keyword));
                }
                if(roles.Count > 0)
                {
                    q = q.Where(u => roles.Contains(u.Role));
                }

                var totalCount = q.Count();
                var resList = await q.Skip(start).Take(limit).ToListAsync();
                foreach(var res in resList)
                {
                    res.Password = null;
                    res.Introduction = null;
                }
                return new UserResList()
                {
                    TotalCount = totalCount,
                    ResList = resList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task<List<User>> GetCourseTeachersAsync(int courseID)
        {
            try
            {
                var idList = await dbContext.UserCourse.Where(uc => uc.UserRole == UserRole.Teacher && uc.CourseId == courseID).ToListAsync();
                var teacherList = new List<User>();
                foreach (var id in idList)
                {
                    var t = await dbContext.Users.FindAsync(id.UserId);
                    t.Introduction = null;
                    t.Password = null;
                    teacherList.Add(t);
                }
                return teacherList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Teacher Info!");
            }
        }

        public async Task<UserRole> GetCourseRoleAsync(string userID, int courseID)
        {
            try
            {
                var uc = await dbContext.UserCourse.FindAsync(userID, courseID);
                if(uc == null)
                {
                    return UserRole.Unknown;
                }
                return uc.UserRole;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Role!");
            }
        }

        public async Task AddUserToCourseAsync(string userID, int courseID, UserRole role)
        {
            try
            {
                if(role == UserRole.Admin || role == UserRole.Unknown)
                {
                    throw new Exception("Invalid Course Role!");
                }
                var uc = new UserCourse()
                {
                    CourseId = courseID,
                    UserId = userID,
                    UserRole = role
                };
                await dbContext.UserCourse.AddAsync(uc);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveUserFromCourseAsync(string userID, int courseID)
        {
            try
            {
                var ucToDelete = await dbContext.UserCourse.FindAsync(userID, courseID);
                if(ucToDelete == null)
                {
                    throw new Exception("User is Not in Course!");
                }
                dbContext.UserCourse.Remove(ucToDelete);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserCourseResList> GetUserCoursesAsync(string userID)
        {
            try
            {
                var ucList = await dbContext.UserCourse.Where(uc => uc.UserId == userID).ToListAsync();
                var courseList = new List<CourseWithTeachers>();
                var teachList = new List<CourseWithTeachers>();
                var assistList = new List<CourseWithTeachers>();
                foreach(var uc in ucList)
                {
                    var course = await dbContext.Courses.FindAsync(uc.CourseId);
                    var t = new CourseWithTeachers()
                    {
                        Course = course,
                        Teachers = await this.GetCourseTeachersAsync(course.Id)
                    };
                    if(uc.UserRole == UserRole.Student)
                    {
                        courseList.Add(t);
                    }
                    else if(uc.UserRole == UserRole.Teacher)
                    {
                        teachList.Add(t);
                    }
                    else
                    {
                        assistList.Add(t);
                    }
                }
                return new UserCourseResList()
                {
                    CourseList = courseList,
                    TeachList = teachList,
                    AssistList = assistList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task<List<CourseWithTeachers>> GetTeachListAsync(string userID)
        {
            try
            {
                var ucList = await dbContext.UserCourse.Where(uc => uc.UserId == userID).ToListAsync();
                var teachList = new List<CourseWithTeachers>();
                foreach (var uc in ucList)
                {
                    if (uc.UserRole == UserRole.Teacher)
                    {
                        var course = await dbContext.Courses.FindAsync(uc.CourseId);
                        var t = new CourseWithTeachers()
                        {
                            Course = course,
                            Teachers = await this.GetCourseTeachersAsync(course.Id)
                        };
                        teachList.Add(t);
                    }
                }
                return teachList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task AddTeacherPageAsync(TeacherPage teacherPage)
        {
            try
            {
                await dbContext.TeacherPages.AddAsync(teacherPage);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task UpdateTeacherPageAsync(TeacherPage teacherPage)
        {
            try
            {
                dbContext.TeacherPages.Update(teacherPage);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task UpdateOrAddTeacherPageAsync(TeacherPage teacherPage)
        {
            try
            {
                var teacherToUpdate = await dbContext.TeacherPages.FindAsync(teacherPage.ID);
                if(teacherToUpdate == null)
                {
                    await this.AddTeacherPageAsync(teacherPage);
                }
                else
                {
                    teacherToUpdate.OfficePlace = teacherPage.OfficePlace;
                    teacherToUpdate.PersonalUrl = teacherPage.PersonalUrl;
                    teacherToUpdate.TeachingAge = teacherPage.TeachingAge;
                    teacherToUpdate.PosTitle = teacherPage.PosTitle;
                    teacherToUpdate.Status = teacherPage.Status;
                    await this.UpdateTeacherPageAsync(teacherToUpdate);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task RemoveTeacherPageAsync(string id)
        {
            try
            {
                var tpToReomve = await dbContext.TeacherPages.FindAsync(id);
                dbContext.TeacherPages.Remove(tpToReomve);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task<TeacherPage> GetTeacherPageAsync(string id)
        {
            try
            {
                var tp = await dbContext.TeacherPages.FindAsync(id);
                return tp;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task<TeacherDetail> GetTeacherDetailAsync(string id)
        {
            try
            {
                var userInfo = await this.GetUserInfoAsync(id);
                return new TeacherDetail()
                {
                    TeacherInfo = userInfo,
                    TeacherPage = await this.GetTeacherPageAsync(id),
                    TeachList = await this.GetTeachListAsync(id)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }
    }
}