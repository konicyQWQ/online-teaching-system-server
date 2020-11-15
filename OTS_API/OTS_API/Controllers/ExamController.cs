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
    public class ExamController : ControllerBase
    {
        private readonly HomeworkExamService examService;
        private readonly TokenService tokenService;

        public ExamController(HomeworkExamService examService, TokenService tokenService)
        {
            this.examService = examService;
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
                    throw new Exception("Token is Invalid!");
                }
                if(t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetCourseRoleAsync(exam.CourseId, t.UserID);
                    if(courseRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await examService.AddExamAsync(exam, questions);
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
                    throw new Exception("Token is Invalid!");
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
        [Route("GetAll")]
        public async Task<dynamic> OnGetExamListAsync(int courseID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
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
                    throw new Exception("Token is Invalid!");
                }
                if (t.Role != UserRole.Admin)
                {
                    var courseRole = await examService.GetCourseRoleAsync(exam.CourseId, t.UserID);
                    if (courseRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
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
        [Route("Start")]
        public async Task<dynamic> OnStartExamAsync([FromForm] int examID, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if(await examService.GetExamRoleAsync(examID, t.UserID) != UserRole.Student)
                {
                    throw new Exception("Invalid Action!");
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
                    throw new Exception("Token is Invalid!");
                }
                if(answers == null || answers.Count <= 0)
                {
                    throw new Exception("Invalid Params!");
                }
                if (await examService.GetExamRoleAsync(answers[0].ExamId, t.UserID) != UserRole.Student)
                {
                    throw new Exception("Invalid Action!");
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
                    throw new Exception("Token is Invalid!");
                }
                if (await examService.GetExamRoleAsync(examID, t.UserID) != UserRole.Student)
                {
                    throw new Exception("Invalid Action!");
                }
                await examService.StuFinishExamAsync(t.UserID, examID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
