using System;
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

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly HomeworkExamService examService;
        private readonly SystemService eventService;
        private readonly TokenService tokenService;

        public ExamController(HomeworkExamService examService, SystemService eventService, TokenService tokenService)
        {
            this.examService = examService;
            this.eventService = eventService;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<dynamic> OnAddExamAsync([FromForm] Exam exam, [FromForm] List<Question> questions, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if(t == null)
                {
                    throw new Exception("请先登录");
                }
                if(t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetCourseRoleAsync(exam.CourseId, t.UserID);
                    if(courseRole == UserRole.Student)
                    {
                        throw new Exception("权限不足");
                    }
                }

                await examService.AddExamAsync(exam, questions);
                await eventService.AddExamCreatedEventAsync(exam.ExamId, exam.Title, t.UserID, exam.CourseId);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        public async Task<dynamic> OnGetExamAsync(int examID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if(role != UserRole.Admin)
                {
                    role = await examService.GetExamRoleAsync(examID, t.UserID);
                }
                if(role == UserRole.Student)
                {
                    return new { Res = true, ExamDetail = await examService.GetStuExamDetailAsync(t.UserID, examID) };
                }
                else
                {
                    return new { Res = true, ExamDetail = await examService.GetExamDetailAsync(examID) };
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("GetStu")]
        public async Task<dynamic> OnGetStuExamAsync(int examID, string stuID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if (t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetExamRoleAsync(examID, t.UserID);
                    if (courseRole == UserRole.Student)
                    {
                        throw new Exception("权限不足");
                    }
                }
                var stuExam = await examService.GetStuExamDetialTAsync(stuID, examID);
                return new { Res = true, StuExam = stuExam };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<dynamic> OnGetExamListAsync(int courseID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                var role = t.Role;
                if(role != UserRole.Admin)
                {
                    role = await examService.GetCourseRoleAsync(courseID, t.UserID);
                }
                if(role == UserRole.Student)
                {
                    return new { Res = true, ExamList = await examService.GetStuCourseExamOverviewAsync(t.UserID, courseID) };
                }
                else
                {
                    return new { Res = true, ExamList = await examService.GetCourseExamOverviewAsync(courseID) };
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<dynamic> OnUpdateExamAsync([FromForm] Exam exam, [FromForm] List<Question> questions, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if (t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetCourseRoleAsync(exam.CourseId, t.UserID);
                    if (courseRole == UserRole.Student)
                    {
                        throw new Exception("权限不足");
                    }
                }
                await examService.UpdateExamAsync(exam, questions);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Remove")]
        public async Task<dynamic> OnRemoveExamAsync([FromForm] int examID, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if (t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetExamRoleAsync(examID, t.UserID);
                    if (courseRole == UserRole.Student)
                    {
                        throw new Exception("权限不足");
                    }
                }
                await examService.RemoveExamAsync(examID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("SetScore")]
        public async Task<dynamic> OnSetScoreAsync([FromForm] string stuID, [FromForm] int examID, [FromForm] int questionID, [FromForm] int score, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if (t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetExamRoleAsync(examID, t.UserID);
                    if (courseRole == UserRole.Student)
                    {
                        throw new Exception("权限不足");
                    }
                }
                await examService.SetExamScore(stuID, examID, questionID, score);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Start")]
        public async Task<dynamic> OnStartExamAsync([FromForm] int examID, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if(await examService.GetExamRoleAsync(examID, t.UserID) != UserRole.Student)
                {
                    throw new Exception("无效操作");
                }
                await examService.StuStartExamAsync(examID, t.UserID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<dynamic> OnSubmitAsync([FromForm] List<UserAnswer> answers, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if(answers == null || answers.Count <= 0)
                {
                    throw new Exception("参数错误");
                }
                if (await examService.GetExamRoleAsync(answers[0].ExamId, t.UserID) != UserRole.Student)
                {
                    throw new Exception("无效操作");
                }
                await examService.StuSubmitAnswersAsync(answers);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Finish")]
        public async Task<dynamic> OnFinishExamAsync([FromForm] int examID, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("请先登录");
                }
                if (await examService.GetExamRoleAsync(examID, t.UserID) != UserRole.Student)
                {
                    throw new Exception("无效操作");
                }
                await examService.StuFinishExamAsync(t.UserID, examID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Export")]
        public async Task<dynamic> OnExportExamAsync(int examID, bool mode, string token)
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
                    role = await examService.GetExamRoleAsync(examID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }

                var examInfo = await examService.GetExamAsync(examID);
                if(examInfo.Status != ExamStatus.Finished)
                {
                    throw new Exception("无效操作");
                }
                var tempFile = Path.GetTempFileName();
                using (var sw = new StreamWriter(new FileStream(tempFile, FileMode.OpenOrCreate), Encoding.GetEncoding("gbk")))
                {
                    await examService.WriteExamInfoAsync(sw, examInfo);
                }

                var fileName = examInfo.Title + " 测试成绩.csv";
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
        public async Task<dynamic> OnExportAllExamsAsync(int courseID, bool mode, string token)
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
                    role = await examService.GetCourseRoleAsync(courseID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }

                var courseInfo = await examService.GetCourseAsync(courseID);
                var tempFile = Path.GetTempFileName();
                using (var sw = new StreamWriter(new FileStream(tempFile, FileMode.OpenOrCreate), Encoding.GetEncoding("gbk")))
                {
                    await examService.WriteCourseExamInfoAsync(sw, courseInfo);
                }

                var fileName = courseInfo.Name + " 课程测试成绩.csv";
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
        [Route("ExportBoth")]
        public async Task<dynamic> OnExportBothAsync(int courseID, bool mode, string token)
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
                    role = await examService.GetCourseRoleAsync(courseID, t.UserID);
                }
                if (role == UserRole.Student)
                {
                    throw new Exception("权限不足");
                }

                var courseInfo = await examService.GetCourseAsync(courseID);
                var tempFile = Path.GetTempFileName();
                using (var sw = new StreamWriter(new FileStream(tempFile, FileMode.OpenOrCreate), Encoding.GetEncoding("gbk")))
                {
                    await examService.WriteCourseHWExamInfoAsync(sw, courseInfo);
                }

                var fileName = courseInfo.Name + " 课程作业测试成绩.csv";
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
    }
}
