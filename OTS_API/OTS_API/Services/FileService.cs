﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using OTS_API.Utilities;
using Microsoft.VisualBasic;

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

        public async Task<Models.File> SavePublicFileAsync(IFormFile file)
        {          
            try
            {
                var fileInfo = new Models.File()
                {
                    Id = 0,
                    Name = file.FileName,
                    Path = Config.publicFilePath + DateTime.Now.ToString("yyyyMMddHHmmss") + CodeGenerator.GetCode(10),
                    Size = file.Length
                };

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

        public async Task<Models.File> SavePrivateFileAsync(IFormFile file, string desPath)
        {
            try
            {
                var fileInfo = new Models.File()
                {
                    Id = 0,
                    Name = file.FileName,
                    Path = Config.privateFilePath + desPath + DateTime.Now.ToString("yyyyMMddHHmmss") + CodeGenerator.GetCode(10),
                    Size = file.Length
                };

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

        public async Task<Models.File> GetFileAsync(int id)
        {
            try
            {
                var file = await dbContext.Files.FindAsync(id);
                if (file == null)
                {
                    throw new Exception("File Not Found!");
                }
                return file;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
            
        }
    }
}
