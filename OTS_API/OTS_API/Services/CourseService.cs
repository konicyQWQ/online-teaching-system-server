using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<CourseWithTeachers>> GetCoursesAsync(int start, int limit)
        {
            try
            {
                var courseList = await dbContext.Courses.ToListAsync();
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
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Info");
            }
        }

        public async Task<List<CourseWithTeachers>> GetCoursesAsync(string keyword, int start, int limit)
        {
            try
            {
                var courseList = await dbContext.Courses.Where(c => c.Name.Contains(keyword)).ToListAsync();
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
                return resList;
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
                    var t = await dbContext.Users.FindAsync(id);
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
    }
}
