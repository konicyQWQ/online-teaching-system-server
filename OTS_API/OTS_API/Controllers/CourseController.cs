using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTS_API.Models;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        [HttpGet]
        public Task<Course> OnGetAsync([FromForm] string id)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Bulletin")]
        public Task<List<Bulletin>> GetBulletinAsync([FromForm] string id)
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Courseware")]
        public Task<List<Courseware>> GetCoursewareAsync([FromForm] string id)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Homework")]
        public Task<List<Homework>> GetHomeworkAsync([FromForm] string id)
        {
            return null;
        }
    }
}
