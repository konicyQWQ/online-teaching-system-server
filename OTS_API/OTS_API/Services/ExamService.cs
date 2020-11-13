using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;

namespace OTS_API.Services
{
    public class ExamService
    {
        private readonly ILogger<ExamService> logger;
        private readonly OTSDbContext dbContext;

        public ExamService(ILogger<ExamService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task AddExamAsync(Exam exam, List<Question> questions)
        {
            try
            {
                await dbContext.Exams.AddAsync(exam);
                await dbContext.SaveChangesAsync();
                foreach(var question in questions)
                {
                    question.ExamId = exam.ExamId;
                    await dbContext.Questions.AddAsync(question);
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }


    }
}
