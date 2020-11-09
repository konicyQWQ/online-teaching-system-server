using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;

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
        public Task<Course> GetCourseAsync(string id)
        {
            return Task.Run(() =>
            {
                return new Course();
            });
        }

        public Task<bool> UpdateCourseAysnc(string id)
        {
            return Task.Run(() =>
            {
                return true;
            });
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
