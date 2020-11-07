using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using OTS_API.Common;

namespace OTS_API.Services
{
    public class FileService
    {
        private readonly ILogger<FileService> logger;
        private readonly OTSDbContext dbContext;

        public FileService(ILogger<FileService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<Models.File> SaveFileAsync(IFormFile file)
        {
            var fileInfo = new Models.File()
            {
                Id = 0,
                Name = file.FileName,
                Path = Config.filePathRoot + DateTime.Now.ToShortTimeString() + CodeGenerator.GetCode(10)
            };

            try
            {
                using (var stream = System.IO.File.Create(fileInfo.Path))
                {
                    await file.CopyToAsync(stream);
                }
                await dbContext.Files.AddAsync(fileInfo);
                await dbContext.SaveChangesAsync();

                return fileInfo;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Server Failed to Save File");
            }
        }
    }
}
