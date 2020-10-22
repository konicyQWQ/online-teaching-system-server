using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;

namespace OTS_API.Services
{
    public class CourseService : DBService
    {
        private readonly ILogger<CourseService> logger;

        public CourseService(ILogger<CourseService> logger ,ILogger<DBService> logger1) : base(logger1)
        {
            this.logger = logger;
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
    }
}
