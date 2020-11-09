﻿using System;
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
        /// 获取单个课程信息
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>课程信息</returns>
        [HttpGet]
        public async Task<dynamic> OnGetAsync(int id)
        {
            try
            {
                var course = await courseService.GetCourseAsync(id);
                var teachers = await courseService.GetCourseTeachersAsync(id);
                return new { Res = true, Course = course, Teachers = teachers };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        /// <summary>
        /// 批量获取课程信息
        /// </summary>
        /// <param name="keyword">关键词，若为空则返回所有</param>
        /// <param name="start">开始的index</param>
        /// <param name="limit">返回的最大数量</param>
        /// <returns>返回检索到的课程列表，附带教师信息</returns>
        [HttpGet]
        [Route("get")]
        public async Task<dynamic> OnGetAsync(string keyword, int start, int limit)
        {
            try
            {
                if(keyword == null)
                {
                    var res = await courseService.GetCoursesAsync(start, limit);
                    return new { Res = true, TotalCount = res.TotalCount, Count = res.ResList.Count, res.ResList };
                }
                else
                {
                    var res = await courseService.GetCoursesAsync(keyword, start, limit);
                    return new { Res = true, TotalCount = res.TotalCount, Count = res.ResList.Count, res.ResList };
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        /// <summary>
        /// 添加课程
        /// </summary>
        /// <param name="course">课程信息</param>
        /// <param name="teachers">教师的id列表（可有多个教师）</param>
        /// <param name="token">管理员token</param>
        /// <returns>课程id</returns>
        [HttpPost]
        public async Task<dynamic> OnPostAsync([FromForm] Course course, [FromForm] List<string> teachers, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if(t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if(t.Role != UserRole.Admin)
                {
                    throw new Exception("Insufficient Authority!");
                }
                var id = await courseService.AddCourseAysnc(course);
                foreach(var teacher in teachers)
                {
                    await courseService.AddTeacherToCouseAsync(id, teacher);
                }
                return new { Res = true, CourseID = id };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        /// <summary>
        /// 更新课程信息
        /// </summary>
        /// <param name="course">课程信息</param>
        /// <param name="teachers">教师id列表</param>
        /// <param name="token">管理员token</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [Route("Update")]
        public async Task<dynamic> OnUpdateAsync([FromForm] Course course, [FromForm] List<string> teachers, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if (t.Role != UserRole.Admin)
                {
                    throw new Exception("Insufficient Authority!");
                }

                await courseService.RemoveCourseTeachersAsync(course.Id);
                foreach(var teacherId in teachers)
                {
                    await courseService.AddTeacherToCouseAsync(course.Id, teacherId);
                }
                await courseService.UpdateCourseAysnc(course);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

    }
}
