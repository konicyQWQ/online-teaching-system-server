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

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> logger;
        private readonly FileService fileService;
        private readonly TokenService tokenService;
        private readonly CourseService courseService;

        public UploadController(ILogger<UploadController> logger, FileService fileService, TokenService tokenService, CourseService courseService)
        {
            this.logger = logger;
            this.fileService = fileService;
            this.tokenService = tokenService;
            this.courseService = courseService;
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

        [HttpPost]
        [Route("UploadPrivate")]
        public async Task<dynamic> OnUploadPrivateAsync([FromForm] List<IFormFile> files, [FromForm] string desRoot, [FromForm] string token)
        {
            try
            {
                var t = await tokenService.GetTokenAsync(token);
                if(t == null)
                {
                    throw new Exception("Insufficient Authority!");
                }
                var fileInfoList = new List<OTS_API.Models.File>();
                int count = 0;
                long size = 0;
                foreach(var file in files)
                {
                    if(file.Length > 0)
                    {
                        var fileInfo = await fileService.SavePrivateFileAsync(file, desRoot);
                        fileInfoList.Add(fileInfo);
                        count++;
                        size += file.Length;
                    }
                }
                return new { Res = true, Count = count, Size = size, fileList = fileInfoList };
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
                    throw new Exception("Insufficient Authority!");
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

        [HttpGet]
        [Route("Courseware")]
        public async Task<dynamic> OnGetCousewareFileAsync(int courseId, int fileId, string token)
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

                var fileInfo = await fileService.GetFileAsync(fileId);
                new FileExtensionContentTypeProvider().TryGetContentType(fileInfo.Name, out var contentType);
                return PhysicalFile(Path.GetFullPath(fileInfo.Path), contentType);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
