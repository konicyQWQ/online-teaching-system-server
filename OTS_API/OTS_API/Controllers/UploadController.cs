using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTS_API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using OTS_API.Models;
using OTS_API.Utilities;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        public int lengthLimit = int.MaxValue;

        private readonly ILogger<UploadController> logger;
        private readonly FileService fileService;
        private readonly TokenService tokenService;
        private readonly CourseService courseService;
        private readonly HomeworkExamService homeworkService;

        public UploadController(ILogger<UploadController> logger, FileService fileService, TokenService tokenService, CourseService courseService, HomeworkExamService homeworkService)
        {
            this.logger = logger;
            this.fileService = fileService;
            this.tokenService = tokenService;
            this.courseService = courseService;
            this.homeworkService = homeworkService;
        }

        /// <summary>
        /// 上传公开文件
        /// </summary>
        /// <param name="files">文件</param>
        /// <returns>上传结果+id</returns>
        [HttpPost]
        public async Task<dynamic> OnPostUploadAsync([FromForm] List<IFormFile> files)
        {
            try
            {
                var fileInfolist = new List<Models.File>();
                int count = 0;
                long size = 0;
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileInfo = await fileService.SavePublicFileAsync(formFile);
                        fileInfolist.Add(fileInfo);
                        count++;
                        size += formFile.Length;
                    }
                }

                return new { Res = true, Count = count, Size = size, fileList = fileInfolist };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
            
        }

        /// <summary>
        /// 获取公开文件
        /// </summary>
        /// <param name="id">文件id</param>
        /// <returns>文件流</returns>
        [HttpGet]
        public async Task<dynamic> OnGetAsync(int id)
        {
            try
            {
                var fileInfo = await fileService.GetFileAsync(id);

                if (fileInfo.Path.Contains("Private"))
                {
                    throw new Exception("权限不足");
                }

                var fs = new FileStream(fileInfo.Path, FileMode.Open, FileAccess.Read);

                new FileExtensionContentTypeProvider().TryGetContentType(fileInfo.Name, out var contentType);
                return new FileStreamResult(fs, contentType);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpPost]
        [Route("Courseware")]
        public async Task<dynamic> OnAddCoursewareFileAsync([FromForm] int courseID, [FromForm] List<IFormFile> formFiles, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null || uc.UserRole == UserRole.Student)
                    {
                        throw new Exception("权限不足");
                    }
                }
                //Only For Dev
                var courseFileRoot = Config.privateFilePath + "Course" + courseID;
                if (!Directory.Exists(courseFileRoot))
                {
                    Directory.CreateDirectory(courseFileRoot);
                    Directory.CreateDirectory(courseFileRoot + "/Courseware");
                    Directory.CreateDirectory(courseFileRoot + "/Homework");
                }


                var desPath = "Course" + courseID + "/Courseware/";
                var fileInfoList = new List<Models.File>();
                int count = 0;
                long size = 0;
                foreach(var file in formFiles)
                {
                    if(file.Length > 0)
                    {
                        var fileInfo = await fileService.SavePrivateFileAsync(file, desPath);
                        fileInfoList.Add(fileInfo);
                        count++;
                        size += file.Length;
                    }
                }

                return new { Res = true, Count = count, Size = size, FileList = fileInfoList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Courseware")]
        public async Task<dynamic> OnGetCousewareFileAsync(int coursewareID, int fileID, string token, bool mode)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                var courseware = await courseService.GetCoursewareAsync(coursewareID);
                var fileInfo = await fileService.GetFileAsync(fileID);
                bool previewMode = false;
                new FileExtensionContentTypeProvider().TryGetContentType(fileInfo.Name, out var contentType);
                if (t == null)
                {
                    if(courseware.Privilege == Privilege.NotDownloadable)
                    {
                        if (fileInfo.Name.Contains("pdf") || contentType.Contains("video"))
                        {
                            previewMode = true;
                        }
                        else
                        {
                            throw new Exception("权限不足");
                        }
                    }
                }
                
                if (mode)
                {
                    var arr = Encoding.UTF8.GetBytes(fileInfo.Name);
                    var name = string.Empty;
                    foreach (var b in arr)
                    {
                        name += string.Format("%{0:X2}", b);
                    }
                    HttpContext.Response.Headers.Add("Content-Disposition", new Microsoft.Extensions.Primitives.StringValues("attachment; filename = " + name));
                }
                if (previewMode)
                {
                    return PhysicalFile(Path.GetFullPath(fileInfo.Path + "(preview)"), contentType, fileInfo.Name);
                }
                else
                {
                    return PhysicalFile(Path.GetFullPath(fileInfo.Path), contentType, fileInfo.Name);
                }
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        /// <summary>
        /// 学生提交作业文件接口
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="formFiles"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Homework")]
        public async Task<dynamic> OnUploadHomeworkFileAsync([FromForm] int courseID, [FromForm] List<IFormFile> formFiles, [FromForm] string token)
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
                    var uc = await courseService.GetUserCourseAsync(t.UserID, courseID);
                    if (uc == null)
                    {
                        throw new Exception("权限不足");
                    }
                }
                //Only For Dev
                var courseFileRoot = Config.privateFilePath + "Course" + courseID;
                if (!Directory.Exists(courseFileRoot))
                {
                    Directory.CreateDirectory(courseFileRoot);
                    Directory.CreateDirectory(courseFileRoot + "/Courseware");
                    Directory.CreateDirectory(courseFileRoot + "/Homework");
                }


                var desPath = "Course" + courseID + "/Homework/";
                var fileInfoList = new List<Models.File>();
                int count = 0;
                long size = 0;
                foreach (var file in formFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileInfo = await fileService.SavePrivateFileAsync(file, desPath);
                        fileInfoList.Add(fileInfo);
                        count++;
                        size += file.Length;
                    }
                }

                return new { Res = true, Count = count, Size = size, FileList = fileInfoList };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("Homework")]
        public async Task<dynamic> OnGetHomeworkFileAsync(int hwID, int fileID, string token, bool mode)
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
                    if(!await homeworkService.CheckInCourse(t.UserID, hwID)){
                        throw new Exception("权限不足");
                    }
                }

                var fileInfo = await fileService.GetFileAsync(fileID);
                new FileExtensionContentTypeProvider().TryGetContentType(fileInfo.Name, out var contentType);
                if (mode)
                {
                    var arr = Encoding.UTF8.GetBytes(fileInfo.Name);
                    var name = string.Empty;
                    foreach (var b in arr)
                    {
                        name += string.Format("%{0:X2}", b);
                    }
                    HttpContext.Response.Headers.Add("Content-Disposition", new Microsoft.Extensions.Primitives.StringValues("attachment; filename = " + name));
                }
                return PhysicalFile(Path.GetFullPath(fileInfo.Path), contentType, fileInfo.Name);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }

        [HttpGet]
        [Route("StuHomework")]
        public async Task<dynamic> OnGetStuHomeworkFileAsync(string stuID, int hwID, int fileID, string token, bool mode)
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
                    var courseRole = await homeworkService.GetHWRoleAsync(hwID, t.UserID);
                    if(courseRole == UserRole.Student && stuID != t.UserID)
                    {
                        throw new Exception("权限不足");
                    }
                }

                var fileInfo = await fileService.GetFileAsync(fileID);
                new FileExtensionContentTypeProvider().TryGetContentType(fileInfo.Name, out var contentType);
                if (mode)
                {
                    var arr = Encoding.UTF8.GetBytes(fileInfo.Name);
                    var name = string.Empty;
                    foreach (var b in arr)
                    {
                        name += string.Format("%{0:X2}", b);
                    }
                    HttpContext.Response.Headers.Add("Content-Disposition", new Microsoft.Extensions.Primitives.StringValues("attachment; filename = " + name));
                }
                return PhysicalFile(Path.GetFullPath(fileInfo.Path), contentType);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
