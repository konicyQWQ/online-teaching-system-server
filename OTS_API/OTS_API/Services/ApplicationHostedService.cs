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
using MySql.Data.MySqlClient;
using OTS_API.Utilities;

namespace OTS_API.Services
{
    public class ApplicationHostedService : BackgroundService
    {
        private readonly ILogger<ApplicationHostedService> logger;
        private ExamDBContext dbContext;

        public ApplicationHostedService(ILogger<ApplicationHostedService> logger)
        {
            this.logger = logger;
            this.dbContext = new ExamDBContext();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Hosted Service Starting!");
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckPendingExamsAsync();
                await CheckActiveExamsAsync();
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                logger.LogInformation("Running");
            }
            logger.LogInformation("Hosted Service Stopping!");
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
                        await dbContext.SaveChangesAsync();
                        var list = await dbContext.UserExams.Where(ue => ue.ExamId == exam.ExamId && ue.Mark == null).ToListAsync();
                        foreach (var ue in list)
                        {
                            var ueToUpdate = await dbContext.UserExams.FindAsync(ue.UserId, ue.ExamId);
                            ueToUpdate.Mark = 0;
                            dbContext.UserExams.Update(ueToUpdate);
                            await dbContext.SaveChangesAsync();
                            await CalculateStuScoreAsync(ue);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task CalculateStuScoreAsync(UserExam ue)
        {
            try
            {
                var userAnswers = await dbContext.UserAnswers.Where(ua => ua.ExamId == ue.ExamId && ua.UserId == ue.UserId).ToListAsync();
                ue.Mark = userAnswers.Sum(ua => ua.Mark);
                dbContext.UserExams.Update(ue);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }
}
