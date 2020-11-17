using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Agreement.JPake;
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
                    if (t.Role == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    var uc = await courseService.GetUserCourseAsync(t.UserID, course.Id);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                if(t.Role == UserRole.Admin)
                {
                    await courseService.RemoveCourseTeachersAsync(course.Id);
                    foreach (var teacherId in teachers)
                    {
                        await courseService.AddTeacherToCouseAsync(course.Id, teacherId);
                    }
                }
                await courseService.UpdateCourseAysnc(course);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("GetRole")]
        public async Task<dynamic> OnGetCourseRole(int courseID, string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if(t.Role != UserRole.Admin)
                {
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    return new { Res = true, Role = uc.UserRole };
                }
                else
                {
                    return new { Res = true, Role = t.Role };
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Bulletin")]
        public async Task<dynamic> OnAddBulletinAsync([FromForm] Bulletin bulletin, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                if(t.Role != UserRole.Admin)
                {
                    if(t.Role == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    var uc = await courseService.GetUserCourseAsync(t.UserID, bulletin.CourseId);
                    if(uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                await courseService.AddBulletinAsync(bulletin);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Bulletin/Update")]
        public async Task<dynamic> OnUpdateBulletinAsync([FromForm] Bulletin bulletin, [FromForm] string token)
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
                    if (t.Role == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    var uc = await courseService.GetUserCourseAsync(t.UserID, bulletin.CourseId);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                await courseService.UpdateBulletinAsync(bulletin);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Bulletin/Delete")]
        public async Task<dynamic> OnDeleteBulletinAsync([FromForm] int bulletinID, [FromForm] string token)
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
                    if (t.Role == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    var bulletin = await courseService.GetBulletinAsync(bulletinID);
                    var uc = await courseService.GetUserCourseAsync(t.UserID, bulletin.CourseId);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                await courseService.DeleteBulletinAsync(bulletinID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Bulletin")]
        public async Task<dynamic> OnGetBulletinAsync(int courseID)
        {
            try
            {
                var list = await courseService.GetCourseBulletinsAsync(courseID);
                return new { Res = true, List = list };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Courseware")]
        public async Task<dynamic> OnAddCoursewareAsync([FromForm] Courseware courseware,[FromForm] List<int> fileList, [FromForm] string token)
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
                    if (t.Role == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseware.CourseId);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                await courseService.AddCoursewareAsync(courseware);
                foreach(var fileID in fileList)
                {
                    await courseService.AddFileToCourseware(courseware.Id, fileID);
                }
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="token"></param>
        /// <returns>所有的课件、每个课件的fileList、包含file{id, name, size}</returns>
        [HttpGet]
        [Route("Courseware/GetAll")]
        public async Task<dynamic> OnGetAllCoursewareAsync(int courseId)
        {
            try
            {
                var allInfoList = await courseService.GetCoursewareWithFilesAsync(courseId);
                return new { Res = true, CoursewareList = allInfoList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Courseware")]
        public async Task<dynamic> OnGetCoursewareAsync(int courseId, int coursewareId, string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseId);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                var courseware = await courseService.GetCoursewareAsync(coursewareId);
                return new { Res = true, Courseware = courseware };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Courseware/Update")]
        public async Task<dynamic> OnUpdateCoursewareAsync([FromForm] Courseware courseware,[FromForm] List<int> fileList, [FromForm] string token)
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
                    if (t.Role == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseware.CourseId);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                await courseService.RemoveCoursewareFilesAsync(courseware.Id);
                foreach(var fileID in fileList)
                {
                    await courseService.AddFileToCourseware(courseware.Id, fileID);
                }
                await courseService.UpdateCousewareAsync(courseware);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Courseware/Delete")]
        public async Task<dynamic> OnDeleteCoursewareAsync([FromForm] int courseId, [FromForm] int coursewareId, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseId);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                await courseService.DeleteCoursewareAsync(coursewareId);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<dynamic> OnGetCourseUsersAsync(int courseID, string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if(uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }
                var resList = await courseService.GetCourseUsersAsync(courseID);
                return new { Res = true, UserList = resList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Group")]
        public async Task<dynamic> OnCreateGroupsAsync([FromForm] int courseID, [FromForm] int groupCount, [FromForm] int maxCount, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null || uc.UserRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.AddCourseGroupAsync(courseID, groupCount, maxCount);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Group")]
        public async Task<dynamic> OnGetGroupsAsync(int courseID, string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                var groupList = await courseService.GetGroupInfoListAsync(courseID);
                return new { Res = true, GroupList = groupList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Group/Remove")]
        public async Task<dynamic> OnRemoveGroupsAsync([FromForm] int courseID, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null || uc.UserRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.RemoveCourseGroupAsync(courseID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Group/Join")]
        public async Task<dynamic> OnJoinGroupAsync([FromForm] int courseID, [FromForm] int groupID, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null || uc.UserRole != UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.AddStuToGroup(t.UserID, courseID, groupID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Group/Exit")]
        public async Task<dynamic> OnExitGroupAsync([FromForm] int courseID, [FromForm] int groupID, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null || uc.UserRole != UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.RemoveStuFromGroup(t.UserID, courseID, groupID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Discussion")]
        public async Task<dynamic> OnCreateDiscussionAsync([FromForm] Discussion discussion, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, discussion.CourseId);
                    if (uc == null || uc.UserRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.AddDiscussionAsync(discussion);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Discussion")]
        public async Task<dynamic> OnGetDiscussionListAsync(int courseID)
        {
            try
            {
                var disList = await courseService.GetDiscussionWithCInfoList(courseID);
                return new { Res = true, DiscussionList = disList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Discussion/Detail")]
        public async Task<dynamic> OnGetDiscussionDetailAsync(int disID)
        {
            try
            {
                var disDetail = await courseService.GetDiscussionDetailAsync(disID);
                return new { Res = true, DiscussionDetail = disDetail };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Discussion/Submit")]
        public async Task<dynamic> OnSubmitDiscussionAsync([FromForm] UserDiscussion userDiscussion, [FromForm] string token)
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
                    var uc = await courseService.GetDiscussionUCAsync(userDiscussion.DiscussionId, t.UserID);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.UserSubmitDiscussionAsync(userDiscussion);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Discussion/Withdraw")]
        public async Task<dynamic> OnWithDrawDiscussionAsync([FromForm] int disID, [FromForm] int level, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if (t == null)
                {
                    throw new Exception("Token is Invalid!");
                }
                var role = t.Role;
                if (t.Role != UserRole.Admin)
                {
                    var uc = await courseService.GetDiscussionUCAsync(disID, t.UserID);
                    if (uc == null)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                    role = uc.UserRole;
                }
                if(role == UserRole.Student)
                {
                    var udToRemove = await courseService.GetUserDiscussionAsync(disID, level);
                    if(udToRemove.UserId != t.UserID)
                    {
                        throw new Exception("权限不足！");
                    }
                }
                await courseService.RemoveUserDiscussionAsync(disID, level);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Discussion/Update")]
        public async Task<dynamic> OnUpdateDiscussionAsync([FromForm] Discussion discussion, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, discussion.CourseId);
                    if (uc == null || uc.UserRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.UpdateDiscussionAsync(discussion);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Discussion/Remove")]
        public async Task<dynamic> OnRemoveDIscussionAsync([FromForm] int disID, [FromForm] string token)
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
                    var uc = await courseService.GetDiscussionUCAsync(disID, t.UserID);
                    if (uc == null || uc.UserRole == UserRole.Student)
                    {
                        throw new Exception("Insufficient Authority!");
                    }
                }

                await courseService.RemoveDiscussionAsync(disID);
                return new { Res = true };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
