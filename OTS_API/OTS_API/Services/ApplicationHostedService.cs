using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OTS_API.DatabaseContext;
using OTS_API.Models;
using OTS_API.Services;

namespace OTS_API.Services
{
    public class ApplicationHostedService : BackgroundService
    {
        private readonly ILogger<ApplicationHostedService> logger;
        private readonly OTSDbContext dbContext;
        private readonly HomeworkExamService examService;

        public ApplicationHostedService(ILogger<ApplicationHostedService> logger, OTSDbContext dbContext, HomeworkExamService examService)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.examService = examService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Hosted Service Started!");
                while (!stoppingToken.IsCancellationRequested)
                {
                    await this.CheckPendingExamsAsync();
                    await this.CheckActiveExamsAsync();
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                logger.LogError("Hosted Service Has Ended!");
            }
        }

        private async Task CheckPendingExamsAsync()
        {
            try
            {
                var examList = await dbContext.Exams.Where(e => e.Status == ExamStatus.Pending).ToListAsync();
                foreach(var exam in examList)
                {
                    if(DateTime.Now >= exam.StartTime)
                    {
                        exam.Status = ExamStatus.Active;
                        dbContext.Exams.Update(exam);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task CheckActiveExamsAsync()
        {
            try
            {
                var examList = await dbContext.Exams.Where(e => e.Status == ExamStatus.Active).ToListAsync();
                foreach(var exam in examList)
                {
                    if(DateTime.Now >= exam.StartTime.AddMinutes(exam.Duration))
                    {
                        exam.Status = ExamStatus.Finished;
                        dbContext.Exams.Update(exam);
                        await examService.ForceFinishExamAsync(exam.ExamId);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
