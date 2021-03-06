﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using OTS_API.Models;
using OTS_API.Services;
using OTS_API.Utilities;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeworkController : ControllerBase
    {
        private readonly TokenService tokenService;
        private readonly SystemService eventService;
        private readonly HomeworkExamService homeworkService;

        public HomeworkController(TokenService tokenService, SystemService eventService, HomeworkExamService homeworkService)
        {
            this.tokenService = tokenService;
            this.eventService = eventService;
            this.homeworkService = homeworkService;
        }

        [HttpGet]
        public async Task<dynamic> OnGetHomeworkAsync(int hwID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                }
                if(role == UserRole.Student)
                {
                    var hwDetail = await homeworkService.GetHomeworkDetailAsync(hwID, t.UserID);
                    return new { Res = true, HWDetail = hwDetail };
                }
                else
                {
                    var hwDetail = await homeworkService.GetHomeworkDetailTAsync(hwID);
                    return new { Res = true, HWDetail = hwDetail };
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<dynamic> OnGetCourseHomeworkAsync(int courseID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if(t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if(role != UserRole.Admin)
                {
                    role = await homeworkService.GetCourseRoleAsync(courseID, t.UserID);
                }
                if(role == UserRole.Student)
                {
                    var hwList = await homeworkService.GetCourseHomeworkOverviewAsync(t.UserID, courseID);
                    return new { Res = true, HWList = hwList };
                }
                else
                {
                    var hwList = await homeworkService.GetCourseHomeworkOverviewTAsync(courseID);
                    return new { Res = true, HWList = hwList };
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Export")]
        public async Task<dynamic> OnExportHomeworkAsync(int hwID, bool mode, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }

                var homeworkInfo = await homeworkService.GetHomeworkAsync(hwID);
                if(homeworkInfo.Status != HWStatus.Finished)
                {
                    throw new Exception("作业尚未结束提交！");
                }
                var tempFile = Path.GetTempFileName();
                using (var sw = new StreamWriter(new FileStream(tempFile, FileMode.OpenOrCreate), Encoding.GetEncoding("gbk")))
                {
                    await homeworkService.WriteHWInfoAsync(sw, homeworkInfo);
                }
                    
                var fileName = homeworkInfo.Title + " 作业成绩.csv";
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
                if (mode)
                {
                    var arr = Encoding.UTF8.GetBytes(fileName);
                    var name = string.Empty;
                    foreach (var b in arr)
                    {
                        name += string.Format("%{0:X2}", b);
                    }
                    HttpContext.Response.Headers.Add("Content-Disposition", new Microsoft.Extensions.Primitives.StringValues("attachment; filename = " + name));
                }
                return PhysicalFile(Path.GetFullPath(tempFile), contentType, fileName);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("ExportAll")]
        public async Task<dynamic> OnExportCourseHomeworkAsync(int courseID, bool mode, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetCourseRoleAsync(courseID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }
                var courseInfo = await homeworkService.GetCourseAsync(courseID);
                var tempFile = Path.GetTempFileName();
                using (var sw = new StreamWriter(new FileStream(tempFile, FileMode.OpenOrCreate), Encoding.GetEncoding("gbk")))
                {
                    await homeworkService.WriteCourseHWInfoAsync(sw, courseInfo);
                }
                var fileName = courseInfo.Name + " 课程作业成绩.csv";
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
                if (mode)
                {
                    var arr = Encoding.UTF8.GetBytes(fileName);
                    var name = string.Empty;
                    foreach (var b in arr)
                    {
                        name += string.Format("%{0:X2}", b);
                    }
                    HttpContext.Response.Headers.Add("Content-Disposition", new Microsoft.Extensions.Primitives.StringValues("attachment; filename = " + name));
                }
                return PhysicalFile(Path.GetFullPath(tempFile), contentType, fileName);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        public async Task<dynamic> OnAddHomeworkAsync([FromForm] Homework homework, [FromForm] List<int> files, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetCourseRoleAsync(homework.CourseId, t.UserID);
                }
                if(role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }
                await homeworkService.AddHomeworkAsync(homework);
                foreach (var file in files)
                {
                    await homeworkService.AddFileToHomeworkAsync(file, homework.HwId);
                }
                await eventService.AddHWCreatedEventAsync(homework.HwId, homework.Title, t.UserID, homework.CourseId);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<dynamic> OnUpdateHomeworkAsync([FromForm] Homework homework, [FromForm] List<int> files, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetCourseRoleAsync(homework.CourseId, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }
                await homeworkService.RemoveHomeworkFilesAsync(homework.HwId);
                await homeworkService.UpdateHomeworkAsync(homework);
                foreach(var file in files)
                {
                    await homeworkService.AddFileToHomeworkAsync(file, homework.HwId);
                }
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("SetScore")]
        public async Task<dynamic> OnSetStuHWScoreAsync([FromForm] string stuID, [FromForm] int hwID, [FromForm] int score, [FromForm] string comment, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }
                await homeworkService.SetStuHomeworkScoreAsync(stuID, hwID, score, comment);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }


        [HttpPost]
        [Route("Submit")]
        public async Task<dynamic> OnSubmitHomeworkAsync([FromForm] UserHomework homework, [FromForm] List<int> files, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role == UserRole.Admin)
                {
                    throw new Exception("无效操作");
                }
                role = await homeworkService.GetHWRoleAsync(homework.HwId, t.UserID);
                if (role != UserRole.Student)
                {
                    throw new Exception("无效操作");
                }
                homework.UserId = t.UserID;
                homework.Mark = null;
                homework.Comment = null;

                var hw = await homeworkService.GetHomeworkAsync(homework.HwId);
                if(hw.Type == HWType.Group)
                {
                    if(await homeworkService.GetGroupIdentityAsync(t.UserID, hw.CourseId) != GroupIdentity.Leader)
                    {
                        throw new Exception("小组作业只能组长提交!");
                    }
                }
                if(hw.Status != HWStatus.Active && hw.Status != HWStatus.NearDDL)
                {
                    throw new Exception("作业尚未开始提交或已经结束！");
                }

                if(await homeworkService.HasSubmittedAsync(homework.HwId, homework.UserId))
                {
                    await homeworkService.RemoveStuHomeworkFilesAsync(homework.HwId, homework.UserId);
                    await homeworkService.UpdateStuHomeworkAsync(homework);
                    foreach(var file in files)
                    {
                        await homeworkService.AddFileToStuHomeworkAsync(file, homework.UserId, homework.HwId);
                    }
                }
                else
                {
                    await homeworkService.AddStuHomeworkAsync(homework);
                    foreach (var file in files)
                    {
                        await homeworkService.AddFileToStuHomeworkAsync(file, t.UserID, homework.HwId);
                    }
                }
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Remove")]
        public async Task<dynamic> OnRemoveHomeworkAsync([FromForm] int hwID, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }
                await homeworkService.RemoveHomeworkAsync(hwID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
