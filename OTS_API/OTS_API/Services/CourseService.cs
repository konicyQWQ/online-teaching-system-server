﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.IO;
using OTS_API.Utilities;

namespace OTS_API.Services
{
    public class CourseService
    {
        private readonly ILogger<CourseService> logger;
        private readonly OTSDbContext dbContext;

        public CourseService(ILogger<CourseService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>课程信息</returns>
        public async Task<Course> GetCourseAsync(int id)
        {
            try
            {
                var course = await dbContext.Courses.FindAsync(id);
                return course;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<CourseResList> GetCoursesAsync(int start, int limit)
        {
            try
            {
                var courseList = await dbContext.Courses.ToListAsync();
                var totalCount = courseList.Count;
                if(limit > totalCount - start)
                {
                    limit = totalCount - start;
                }
                courseList = courseList.GetRange(start, limit);
                var resList = new List<CourseWithTeachers>();
                foreach(var course in courseList)
                {
                    var teacherList = await this.GetCourseTeachersAsync(course.Id);
                    var res = new CourseWithTeachers()
                    {
                        Course = course,
                        Teachers = teacherList
                    };
                    resList.Add(res);
                }
                return new CourseResList()
                {
                    TotalCount = totalCount,
                    ResList = resList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Info");
            }
        }

        public async Task<CourseResList> GetCoursesAsync(string keyword, int start, int limit)
        {
            try
            {
                var courseList = await dbContext.Courses.Where(c => c.Name.Contains(keyword)).ToListAsync();
                var totalCount = courseList.Count;
                if (limit > totalCount - start)
                {
                    limit = totalCount - start;
                }
                courseList = courseList.GetRange(start, limit);
                var resList = new List<CourseWithTeachers>();
                foreach (var course in courseList)
                {
                    var teacherList = await this.GetCourseTeachersAsync(course.Id);
                    var res = new CourseWithTeachers()
                    {
                        Course = course,
                        Teachers = teacherList
                    };
                    resList.Add(res);
                }
                return new CourseResList()
                {
                    TotalCount = totalCount,
                    ResList = resList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Info");
            }
        }

        public async Task<List<User>> GetCourseTeachersAsync(int courseID)
        {
            try
            {
                var idList = await dbContext.UserCourse.Where(uc => uc.UserRole == UserRole.Teacher && uc.CourseId == courseID).ToListAsync();
                var teacherList = new List<User>();
                foreach(var id in idList)
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

        public async Task RemoveCourseTeachersAsync(int courseID)
        {
            try
            {
                dbContext.UserCourse.RemoveRange(await dbContext.UserCourse.Where(uc => uc.CourseId == courseID).ToArrayAsync());
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateCourseAysnc(Course course)
        {
            try
            {
                dbContext.Courses.Update(course);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Update Course Info!");
            }
        }

        public Task<bool> DeleteCourseAsync(string id)
        {
            return Task.Run(() =>
            {
                return true;
            });
        }

        public async Task<int> AddCourseAysnc(Course course)
        {
            try
            {
                await dbContext.Courses.AddAsync(course);
                await dbContext.SaveChangesAsync();
                var courseFileRoot = Config.privateFilePath + "Course" + course.Id;
                Directory.CreateDirectory(courseFileRoot);
                Directory.CreateDirectory(courseFileRoot + "/Courseware");
                Directory.CreateDirectory(courseFileRoot + "/Homework");
                return course.Id;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddTeacherToCouseAsync(int courseId, string teacherId)
        {
            try
            {
                var uc = new UserCourse()
                {
                    CourseId = courseId,
                    UserId = teacherId,
                    UserRole = UserRole.Teacher
                };
                await dbContext.UserCourse.AddAsync(uc);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Add Teacher(id: " + teacherId + ") Failed!");
            }
        }

        public async Task<UserCourse> GetUserCourseAsync(string userID, int courseID)
        {
            try
            {
                var res = await dbContext.UserCourse.FindAsync(userID, courseID);
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task AddBulletinAsync(Bulletin bulletin)
        {
            try
            {
                await dbContext.Bulletin.AddAsync(bulletin);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateBulletinAsync(Bulletin bulletin)
        {
            try
            {
                dbContext.Bulletin.Update(bulletin);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task DeleteBulletinAsync(int bulletinID)
        {
            try
            {
                var bulletinToDelete = await dbContext.Bulletin.FindAsync(bulletinID);
                if(bulletinToDelete == null)
                {
                    throw new Exception("Cannot Find Bulletin(id: " + bulletinID + ")!");
                }
                dbContext.Bulletin.Remove(bulletinToDelete);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<Bulletin> GetBulletinAsync(int bulletinID)
        {
            try
            {
                var res = await dbContext.Bulletin.FindAsync(bulletinID);
                if (res == null)
                {
                    throw new Exception("Cannot Find Bulletin(id: " + bulletinID + ")!");
                }
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Bulletin>> GetCourseBulletinsAsync(int courseID)
        {
            try
            {
                var list = await dbContext.Bulletin.Where(b => b.CourseId == courseID).ToListAsync();
                return list;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddCoursewareAsync(Courseware courseware)
        {
            try
            {
                await dbContext.Coursewares.AddAsync(courseware);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<Courseware> GetCoursewareAsync(int id)
        {
            try
            {
                var res = await dbContext.Coursewares.FindAsync(id);
                if(res == null)
                {
                    throw new Exception("Unable to Find Courseware(id: " + id + ")!");
                }
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateCousewareAsync(Courseware courseware)
        {
            try
            {
                dbContext.Coursewares.Update(courseware);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task DeleteCoursewareAsync(int id)
        {
            try
            {
                var cwToDelete = await dbContext.Coursewares.FindAsync(id);
                if(cwToDelete == null)
                {
                    throw new Exception("Unable to Find Courseware(id: " + id + ")!");
                }
                dbContext.Coursewares.Remove(cwToDelete);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddFileToCourseware(int coursewareID, int fileID)
        {
            try
            {
                var coursewareFile = new CoursewareFile()
                {
                    CoursewareId = coursewareID,
                    FileId = fileID
                };
                await dbContext.CoursewareFile.AddAsync(coursewareFile);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Models.File>> GetCoursewareFilesAsync(int coursewareID)
        {
            try
            {
                var cfList = await dbContext.CoursewareFile.Where(cf => cf.CoursewareId == coursewareID).ToListAsync();
                var fileList = new List<Models.File>();
                foreach(var cf in cfList)
                {
                    var fileInfo = await dbContext.Files.FindAsync(cf.FileId);
                    fileList.Add(fileInfo);
                }
                return fileList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Courseware FileList!");
            }
        }

        public async Task RemoveCoursewareFilesAsync(int coursewareID)
        {
            try
            {
                var arrayToRemove = await dbContext.CoursewareFile.Where(cf => cf.CoursewareId == coursewareID).ToArrayAsync();
                dbContext.CoursewareFile.RemoveRange(arrayToRemove);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable Reset Courseware FileList!");
            }
        }

        public async Task<List<CoursewareWithFiles>> GetCoursewareWithFilesAsync(int courseID)
        {
            try
            {
                var coursewareList = await dbContext.Coursewares.Where(c => c.CourseId == courseID).ToListAsync();
                var cwfList = new List<CoursewareWithFiles>();
                foreach(var courseware in coursewareList)
                {
                    var cwf = new CoursewareWithFiles()
                    {
                        Courseware = courseware,
                        Files = await GetCoursewareFilesAsync(courseware.Id)
                    };
                    cwfList.Add(cwf);
                }
                return cwfList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Courseware FileList!");
            }
        }
    }
}
