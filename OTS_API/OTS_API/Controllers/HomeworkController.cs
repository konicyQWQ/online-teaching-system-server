using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTS_API.Models;
using OTS_API.Services;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeworkController : ControllerBase
    {
        private readonly TokenService tokenService;
        private readonly HomeworkService homeworkService;

        public HomeworkController(TokenService tokenService, HomeworkService homeworkService)
        {
            this.tokenService = tokenService;
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
                    throw new Exception("Toke is Invalid!");
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
                    throw new Exception("Toke is Invalid!");
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

        [HttpPost]
        public async Task<dynamic> OnAddHomeworkAsync([FromForm] Homework homework, [FromForm] List<int> files, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Toke is Invalid!");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetCourseRoleAsync(homework.CourseId, t.UserID);
                }
                if(role == UserRole.Student)
                {
                    throw new Exception("Insufficient Authority!");
                }
                await homeworkService.AddHomeworkAsync(homework);
                foreach (var file in files)
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
        [Route("Update")]
        public async Task<dynamic> OnUpdateHomeworkAsync([FromForm] Homework homework, [FromForm] List<int> files, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Toke is Invalid!");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetCourseRoleAsync(homework.CourseId, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("Insufficient Authority!");
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
        public async Task<dynamic> OnSetStuHWScoreAsync([FromForm] string stuID, [FromForm] int hwID, [FromForm] int score, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Toke is Invalid!");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("Insufficient Authority!");
                }
                await homeworkService.SetStuHomeworkScoreAsync(stuID, hwID, score);
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
                    throw new Exception("Toke is Invalid!");
                }
                var role = t.Role;
                if (role == UserRole.Admin)
                {
                    throw new Exception("Invalid Action!");
                }
                role = await homeworkService.GetHWRoleAsync(homework.HwId, t.UserID);
                if (role != UserRole.Student)
                {
                    throw new Exception("Invalid Action!");
                }
                if(homework.UserId != t.UserID)
                {
                    throw new Exception("Invalid Action!");
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
                    throw new Exception("Toke is Invalid!");
                }
                var role = t.Role;
                if (role != UserRole.Admin)
                {
                    role = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("Insufficient Authority!");
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
