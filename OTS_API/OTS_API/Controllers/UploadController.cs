using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTS_API.Services;

namespace OTS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> logger;
        private readonly FileService fileService;

        public UploadController(ILogger<UploadController> logger, FileService fileService)
        {
            this.logger = logger;
            this.fileService = fileService;
        }

        [HttpPost]
        public async Task<dynamic> OnPostUploadAsync([FromForm] List<IFormFile> files)
        {
            try
            {
                var fileInfolist = new List<Models.File>();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileInfo = await fileService.SaveFileAsync(formFile);
                        fileInfolist.Add(fileInfo);
                    }
                }

                return new { Res = true, Count = files.Count, Size = files.Sum(f => f.Length), fileList = fileInfolist };
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
            
        }

        [HttpGet]
        public async Task<dynamic> OnGetAsync(int id)
        {
            try
            {
                var fileInfo = await fileService.GetFileAsync(id);

                return PhysicalFile(fileInfo.Path, Path.GetExtension(fileInfo.Name), fileInfo.Name);
            }
            catch (Exception e)
            {
                return new { Res = false, Error = e.Message };
            }
        }
    }
}
