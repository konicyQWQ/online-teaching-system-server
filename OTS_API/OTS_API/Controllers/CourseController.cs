using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTS_API.Models;
using OTS_API.Services;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService courseService;
        private readonly TokenService tokenService;
        private readonly ILogger<CourseController> logger;

        public CourseController(CourseService courseService, TokenService tokenService, ILogger<CourseController> logger)
        {
            this.courseService = courseService;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>课程信息</returns>
        [HttpGet]
        public Task<Course> OnGetAsync(string id)
        {
            return null;
        }

        /// <summary>
        /// 获取课程相关的公告
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>公告列表</returns>
        [HttpGet]
        [Route("Bulletin")]
        public Task<List<Bulletin>> GetBulletinAsync(string id)
        {
            return Task.Run(() =>
            {
                return new List<Bulletin>()
            {
                new Bulletin()
                {
                    CourseID = "ABCDE",
                    Content = "Content",
                    Time = DateTime.Now,
                    Title = "Title",
                }
            };
            });
            
        }

        /// <summary>
        /// 获取课件列表
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>课件列表</returns>
        [HttpGet]
        [Route("Courseware")]
        public Task<List<Courseware>> GetCoursewareAsync(string id)
        {
            return null;
        }

        /// <summary>
        /// 获取课程作业列表
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>作业列表</returns>
        [HttpGet]
        [Route("Homework")]
        public Task<List<Homework>> GetHomeworkAsync(string id)
        {
            return null;
        }
    }
}
