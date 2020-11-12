using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OTS_API.DatabaseContext;
using OTS_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Services
{
    public class HomeworkService
    {
        private readonly ILogger<HomeworkService> logger;
        private readonly OTSDbContext dbContext;

        public HomeworkService(ILogger<HomeworkService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task AddHomeworkAsync(Homework homework)
        {
            try
            {
                await dbContext.Homework.AddAsync(homework);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddFileToHomeworkAsync(int fileID, int homeworkID)
        {
            try
            {
                var hwFile = new HomeworkFile()
                {
                    FileID = fileID,
                    HwID = homeworkID
                };
                await dbContext.HomeworkFile.AddAsync(hwFile);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<Homework> GetHomeworkAsync(int hwID)
        {
            try
            {
                var hw = await dbContext.Homework.FindAsync(hwID);
                if(hw == null)
                {
                    throw new Exception("Unable to Find Homework(id: " + hwID + ")!");
                }
                return hw;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Models.File>> GetHomeworkFileAsync(int hwID)
        {
            try
            {
                var hfList = await dbContext.HomeworkFile.Where(hf => hf.HwID == hwID).ToListAsync();
                var fileList = new List<Models.File>();
                foreach(var hf in hfList)
                {
                    fileList.Add(await dbContext.Files.FindAsync(hf.FileID));
                }
                return fileList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<HomeworkWithFiles> GetHomeworkWithFilesAsync(int hwID)
        {
            try
            {
                var hw = await this.GetHomeworkAsync(hwID);
                var files = await this.GetHomeworkFileAsync(hwID);
                return new HomeworkWithFiles()
                {
                    Homework = hw,
                    Files = files
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        
    }
}
