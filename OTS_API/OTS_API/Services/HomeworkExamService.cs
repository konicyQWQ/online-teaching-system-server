using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OTS_API.DatabaseContext;
using OTS_API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Services
{
    public class HomeworkExamService
    {
        private readonly ILogger<HomeworkExamService> logger;
        private readonly OTSDbContext dbContext;

        public HomeworkExamService(ILogger<HomeworkExamService> logger, OTSDbContext dbContext)
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

        public async Task AddStuHomeworkAsync(UserHomework homework)
        {
            try
            {
                var hw = await this.GetHomeworkAsync(homework.HwId);
                if (!hw.IsOpen())
                {
                    throw new Exception("Cannot Submit Closed Homework!");
                }
                await dbContext.UserHomework.AddAsync(homework);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddFileToStuHomeworkAsync(int fileID, string userID, int hwID)
        {
            try
            {
                var uhwFile = new UserHomeworkFile()
                {
                    FileID = fileID,
                    UserID = userID,
                    HwID = hwID
                };
                await dbContext.UserHomeworkFile.AddAsync(uhwFile);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<bool> HasSubmittedAsync(int hwID, string userID)
        {
            try
            {
                var uhw = await dbContext.UserHomework.FindAsync(userID, hwID);
                return uhw != null;
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

        public async Task<Course> GetCourseAsync(int courseID)
        {
            try
            {
                var hw = await dbContext.Courses.FindAsync(courseID);
                if(hw == null)
                {
                    throw new Exception("Unable to Find Course(id: " + courseID + ")!");
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

        /// <summary>
        /// Get Stu's Homework, return UH Info
        /// </summary>
        /// <param name="stuID">Stu ID</param>
        /// <param name="hwID">HW ID</param>
        /// <returns>null if the Student didn't submit his homework</returns>
        public async Task<UserHomework> GetStuHomeworkAsync(string stuID, int hwID)
        {
            try
            {
                var stuHW = await dbContext.UserHomework.FindAsync(stuID, hwID);
                return stuHW;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<int> GetStuHomeworkScoreAsync(string stuID, int hwID)
        {
            try
            {
                var hw = await this.GetStuHomeworkAsync(stuID, hwID);
                if(hw == null)
                {
                    return 0;
                }
                if(hw.Mark == null)
                {
                    return 0;
                }
                return hw.Mark.Value;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Models.File>> GetStuHomeworkFilesAsync(string stuID, int hwID)
        {
            try
            {
                var uhfList = await dbContext.UserHomeworkFile.Where(uhf => uhf.HwID == hwID && uhf.UserID == stuID).ToListAsync();
                var fileList = new List<Models.File>();
                foreach(var uhf in uhfList)
                {
                    fileList.Add(await dbContext.Files.FindAsync(uhf.FileID));
                }
                return fileList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserHomeworkWithFiles> GetStuHomeworkWithFilesAsync(string stuID, int hwID)
        {
            try
            {
                var uh = await this.GetStuHomeworkAsync(stuID, hwID);
                var fileList = await this.GetStuHomeworkFilesAsync(stuID, hwID);
                var userInfo = await dbContext.Users.FindAsync(stuID);
                userInfo.Password = null;
                userInfo.Introduction = null;
                return new UserHomeworkWithFiles()
                {
                    UserInfo = userInfo,
                    UserHomework = uh,
                    Files = fileList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserHomeworkWithFiles> GetStuHomeworkWithFilesTAsync(string stuID, int hwID)
        {
            try
            {
                var uh = await this.GetStuHomeworkAsync(stuID, hwID);
                var fileList = await this.GetStuHomeworkFilesAsync(stuID, hwID);
                var userInfo = await dbContext.Users.FindAsync(stuID);
                if(userInfo == null)
                {
                    throw new Exception("User Not Found!");
                }
                userInfo.Password = null;
                userInfo.Introduction = null;
                return new UserHomeworkWithFiles()
                {
                    UserInfo = userInfo,
                    UserHomework = uh,
                    Files = fileList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<HomeworkStatistics> GetHomeworkStatisticsAsync(int hwID, List<UserCourse> ucList)
        {
            try
            {
                int submitCount = 0;
                int scoredCount = 0;
                foreach(var uc in ucList)
                {
                    var stuHW = await this.GetStuHomeworkAsync(uc.UserId, hwID);
                    if(stuHW != null)
                    {
                        submitCount++;
                        if(stuHW.Mark != null)
                        {
                            scoredCount++;
                        }
                    }
                }
                return new HomeworkStatistics()
                {
                    TotalCount = ucList.Count,
                    SubmitCount = submitCount,
                    ScoredCount = scoredCount
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Statistics!");
            }
        }

        public async Task<HomeworkStatistics> GetHomeworkStatisticsAsync(int hwID, List<User> stuList)
        {
            try
            {
                int submitCount = 0;
                int scoredCount = 0;
                foreach (var uc in stuList)
                {
                    var stuHW = await this.GetStuHomeworkAsync(uc.Id, hwID);
                    if (stuHW != null)
                    {
                        submitCount++;
                        if (stuHW.Mark != null)
                        {
                            scoredCount++;
                        }
                    }
                }
                return new HomeworkStatistics()
                {
                    TotalCount = stuList.Count,
                    SubmitCount = submitCount,
                    ScoredCount = scoredCount
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Statistics!");
            }
        }

        public async Task<List<UserCourse>> GetCourseStuListAsync(int courseID)
        {
            try
            {
                var ucList = await dbContext.UserCourse.Where(uc => uc.UserRole == UserRole.Student && uc.CourseId == courseID).ToListAsync();
                return ucList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<User>> GetCourseStuInfoListAsync(int courseID)
        {
            try
            {
                var ucList = await this.GetCourseStuListAsync(courseID);
                var userList = new List<User>();
                foreach(var uc in ucList)
                {
                    var user = await dbContext.Users.FindAsync(uc.UserId);
                    if(user != null)
                    {
                        userList.Add(user);
                    }
                }
                return userList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<User>> GetHWStuInfoListAsync(int hwID)
        {
            try
            {
                var hw = await this.GetHomeworkAsync(hwID);
                if(hw == null)
                {
                    throw new Exception("Unable to Find Homework(id: " + hwID + ")!");
                }
                return await this.GetCourseStuInfoListAsync(hw.CourseId);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<User>> GetHWStuInfoListAsync(Homework homework)
        {
            try
            {
                return await this.GetCourseStuInfoListAsync(homework.CourseId);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<UserHomeworkWithFiles>> GetCourseStuHomeworkWithFilesTAsync(int hwID)
        {
            try
            {
                var hw = await this.GetHomeworkAsync(hwID);
                var ucList = await this.GetCourseStuListAsync(hw.CourseId);
                var resList = new List<UserHomeworkWithFiles>();
                foreach(var uc in ucList)
                {
                    resList.Add(await this.GetStuHomeworkWithFilesTAsync(uc.UserId, hwID));
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<UserHomeworkWithFiles>> GetCourseStuHomeworkWithFilesTAsync(Homework hw, List<UserCourse> ucList)
        {
            try
            {
                var resList = new List<UserHomeworkWithFiles>();
                foreach (var uc in ucList)
                {
                    resList.Add(await this.GetStuHomeworkWithFilesTAsync(uc.UserId, hw.HwId));
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Homework>> GetCourseHomeworkAsync(int courseID)
        {
            try
            {
                var resList = await dbContext.Homework.Where(hw => hw.CourseId == courseID).ToListAsync();
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<StuHomeworkOverview>> GetCourseHomeworkOverviewAsync(string stuID, int courseID)
        {
            try
            {
                var hwList = await this.GetCourseHomeworkAsync(courseID);
                var resList = new List<StuHomeworkOverview>();
                foreach (var hw in hwList)
                {
                    var uh = await this.GetStuHomeworkAsync(stuID, hw.HwId);
                    hw.Content = null;
                    resList.Add(new StuHomeworkOverview()
                    {
                        Homework = hw,
                        UserHomework = uh
                    });
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<HomeworkOverview>> GetCourseHomeworkOverviewTAsync(int courseID)
        {
            try
            {
                var hwList = await this.GetCourseHomeworkAsync(courseID);
                var ucList = await this.GetCourseStuListAsync(courseID);
                var resList = new List<HomeworkOverview>();
                foreach(var hw in hwList)
                {
                    var hwStat = await this.GetHomeworkStatisticsAsync(hw.HwId, ucList);
                    hw.Content = null;
                    resList.Add(new HomeworkOverview()
                    {
                        Homework = hw,
                        Statistics = hwStat
                    });
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserHomeworkDetail> GetHomeworkDetailAsync(int hwID, string stuID)
        {
            try
            {
                var hw = await this.GetHomeworkWithFilesAsync(hwID);
                var stuHW = await this.GetStuHomeworkWithFilesAsync(stuID, hwID);
                return new UserHomeworkDetail()
                {
                    Homework = hw,
                    UserHomework = stuHW
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<HomeworkDetail> GetHomeworkDetailTAsync(int hwID)
        {
            try
            {
                var hwWithFiles = await this.GetHomeworkWithFilesAsync(hwID);
                var ucList = await this.GetCourseStuListAsync(hwWithFiles.Homework.CourseId);
                var hwStatistics = await this.GetHomeworkStatisticsAsync(hwID, ucList);
                var stuHWList = await this.GetCourseStuHomeworkWithFilesTAsync(hwWithFiles.Homework, ucList);
                return new HomeworkDetail()
                {
                    Homework = hwWithFiles,
                    Statistics = hwStatistics,
                    StuHomeworkList = stuHWList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserRole> GetCourseRoleAsync(int courseID, string userID)
        {
            try
            {
                var uc = await dbContext.UserCourse.FindAsync(userID, courseID);
                if(uc == null)
                {
                    throw new Exception("User-Course Not Valid!");
                }
                return uc.UserRole;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        public async Task<UserRole> GetHWRoleAsync(int hwID, string userID)
        {
            try
            {
                var hw = await this.GetHomeworkAsync(hwID);
                if(hw == null)
                {
                    throw new Exception("Cannot Find Homework(id: " + hwID + ")!");
                }
                return await this.GetCourseRoleAsync(hw.CourseId, userID);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        public async Task UpdateHomeworkAsync(Homework homework)
        {
            try
            {
                dbContext.Homework.Update(homework);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateStuHomeworkAsync(UserHomework userHomework)
        {
            try
            {
                var hwToUpdate = await this.GetStuHomeworkAsync(userHomework.UserId, userHomework.HwId);
                if(hwToUpdate == null)
                {
                    throw new Exception("Unable to Find UHW(" + userHomework.UserId + ", " + userHomework.HwId + ")");
                }
                hwToUpdate.Description = userHomework.Description;
                dbContext.UserHomework.Update(hwToUpdate);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task SetStuHomeworkScoreAsync(string userID, int hwID, int mark, string comment)
        {
            try
            {
                var hwToUpdate = await this.GetStuHomeworkAsync(userID, hwID);
                if (hwToUpdate == null)
                {
                    throw new Exception("Unable to Find UHW(" + userID + ", " + hwID + ")");
                }
                hwToUpdate.Mark = mark;
                hwToUpdate.Comment = comment;
                dbContext.UserHomework.Update(hwToUpdate);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveHomeworkFilesAsync(int hwID)
        {
            try
            {
                var hwFileList = await dbContext.HomeworkFile.Where(hwf => hwf.HwID == hwID).ToListAsync();
                dbContext.HomeworkFile.RemoveRange(hwFileList);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveStuHomeworkFilesAsync(int hwID, string userID)
        {
            try
            {
                var uhwFileList = await dbContext.UserHomeworkFile.Where(uhf => uhf.HwID == hwID && uhf.UserID == userID).ToListAsync();
                dbContext.UserHomeworkFile.RemoveRange(uhwFileList);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveHomeworkAsync(int hwID)
        {
            try
            {
                var hwToDelete = await this.GetHomeworkAsync(hwID);
                dbContext.Homework.Remove(hwToDelete);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task WriteHWInfoAsync(StreamWriter sw, Homework homework)
        {
            try
            {
                var stuList = await this.GetCourseStuInfoListAsync(homework.CourseId);
                var course = await dbContext.Courses.FindAsync(homework.CourseId);
                var hwStat = await this.GetHomeworkStatisticsAsync(homework.HwId, stuList);
                await sw.WriteLineAsync("课程名称：," + course.Name + ",人数：," + hwStat.TotalCount);
                await sw.WriteLineAsync("作业名称：," + homework.Title);
                await sw.WriteLineAsync("开始时间：," + homework.StartTime.ToUniversalTime());
                await sw.WriteLineAsync("结束时间：," + homework.EndTime.ToUniversalTime());
                await sw.WriteLineAsync("总分：," + homework.TotalMark + ",占比：," + homework.Percentage);
                await sw.WriteLineAsync("完成情况：,已交：," + hwStat.ScoredCount + ",未交：," + (hwStat.TotalCount - hwStat.SubmitCount) + ",已评分：," + hwStat.ScoredCount);
                await sw.WriteLineAsync("学号,姓名,学院,得分");
                foreach (var stu in stuList)
                {
                    await sw.WriteLineAsync(string.Join(",", stu.Id, stu.Name, stu.Department, await this.GetStuHomeworkScoreAsync(stu.Id, homework.HwId)));
                }
                await sw.FlushAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task WirteCourseHWInfoAsync(StreamWriter sw, Course course)
        {
            try
            {
                var stuList = await this.GetCourseStuInfoListAsync(course.Id);
                var hwList = await this.GetCourseHomeworkAsync(course.Id);
                await sw.WriteLineAsync("课程名称：," + course.Name + ",人数：," + stuList.Count);
                var buffList = new List<string>()
                {
                    "学号", "姓名", "学院"
                };
                foreach(var hw in hwList)
                {
                    buffList.Add(hw.Title);
                }
                buffList.Add("总分");
                await sw.WriteLineAsync(string.Join(',', buffList));
                foreach(var stu in stuList)
                {
                    buffList.Clear();
                    buffList.Add(stu.Id);
                    buffList.Add(stu.Name);
                    buffList.Add(stu.Department);
                    double totalScore = 0;
                    foreach(var hw in hwList)
                    {
                        var score = await this.GetStuHomeworkScoreAsync(stu.Id, hw.HwId);
                        totalScore += (double)score * (hw.Percentage / 100.0);
                        buffList.Add(score.ToString());
                    }
                    buffList.Add(totalScore.ToString());
                    await sw.WriteLineAsync(string.Join(',', buffList));
                }
                await sw.FlushAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<bool> CheckInCourse(string userID, int hwID)
        {
            try
            {
                var homework = await this.GetHomeworkAsync(hwID);
                var uc = await dbContext.UserCourse.FindAsync(userID, homework.CourseId);
                return uc != null;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 添加考试
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        public async Task AddExamAsync(Exam exam, List<Question> questions)
        {
            try
            {
                exam.Status = ExamStatus.Pending;
                await dbContext.Exams.AddAsync(exam);
                await dbContext.SaveChangesAsync();
                foreach (var question in questions)
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

        public async Task RemoveExamQuestionsAsync(int examID)
        {
            try
            {
                var removeList = await dbContext.Questions.Where(q => q.ExamId == examID).ToListAsync();
                dbContext.Questions.RemoveRange(removeList);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 更新考试
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        public async Task UpdateExamAsync(Exam exam, List<Question> questions)
        {
            try
            {
                if(exam.Status == ExamStatus.Pending)
                {
                    await this.RemoveExamQuestionsAsync(exam.ExamId);
                    dbContext.Update(exam);
                    foreach (var q in questions)
                    {
                        q.ExamId = exam.ExamId;
                        await dbContext.Questions.AddAsync(q);
                    }
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Invalid Action!");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 删除考试
        /// </summary>
        /// <param name="examID"></param>
        /// <returns></returns>
        public async Task RemoveExamAsync(int examID)
        {
            try
            {
                var examToRemove = await this.GetExamAsync(examID);
                if(examToRemove.Status == ExamStatus.Pending)
                {
                    dbContext.Exams.Remove(examToRemove);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Invalid Action!");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 学生开始考试
        /// </summary>
        /// <param name="examID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task StuStartExamAsync(int examID, string userID)
        {
            try
            {
                var exam = await this.GetExamAsync(examID);
                if(exam.Status == ExamStatus.Active && !await this.HasStuStartedExamAsync(userID, examID))
                {
                    var ue = new UserExam()
                    {
                        UserId = userID,
                        ExamId = examID
                    };
                    await dbContext.UserExam.AddAsync(ue);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Invalid Action!");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveStuAnswersAsync(string userID, int examID)
        {
            try
            {
                var answersToRemove = await this.GetStuAnswerListAsync(userID, examID);
                dbContext.UserAnswer.RemoveRange(answersToRemove);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 学生提交作答
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public async Task StuSubmitAnswersAsync(List<UserAnswer> answers)
        {
            try
            {
                if(answers == null || answers.Count <= 0)
                {
                    throw new Exception("Answers are Not Valid!");
                }
                var userID = answers[0].UserId;
                var examID = answers[0].ExamId;
                var exam = await this.GetExamAsync(examID);
                if(exam.Status == ExamStatus.Active && await this.HasStuStartedExamAsync(userID, examID) && !await HasStuFinishedExamAsync(userID, examID))
                {
                    await this.RemoveStuAnswersAsync(userID, examID);
                    foreach (var an in answers)
                    {
                        an.UserId = userID;
                        an.ExamId = examID;
                        await dbContext.UserAnswer.AddAsync(an);
                    }
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Invalid Action!");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 学生交卷
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="examID"></param>
        /// <returns></returns>
        public async Task StuFinishExamAsync(string userID, int examID)
        {
            try
            {
                var ueToUpdate = await this.GetStuExamAsync(userID, examID);
                ueToUpdate.Mark = 0;
                dbContext.UserExam.Update(ueToUpdate);
                await dbContext.SaveChangesAsync();
                _ = CalculateStuScoreAsync(userID, examID);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task ForceFinishExamAsync(int examID)
        {
            try
            {
                var list = await dbContext.UserExam.Where(ue => ue.ExamId == examID && ue.Mark == null).ToListAsync();
                foreach(var ue in list)
                {
                    await this.StuFinishExamAsync(ue.UserId, ue.ExamId);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 老师设置分数
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="examID"></param>
        /// <param name="questionID"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task SetExamScore(string userID, int examID, int questionID, int score)
        {
            try
            {
                if(await this.HasStuFinishedExamAsync(userID, examID))
                {
                    var uaToUpdate = await this.GetStuAnswerAsync(userID, examID, questionID);
                    uaToUpdate.Mark = score;
                    dbContext.UserAnswer.Update(uaToUpdate);
                    await dbContext.SaveChangesAsync();
                    _ = this.CalculateStuScoreAsync(userID, examID);
                }
                else
                {
                    throw new Exception("Invalid Action!");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 客观题自动评分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="examID"></param>
        /// <returns></returns>
        public async Task AutoSetScoreAsync(string userID, int examID)
        {
            try
            {
                var questions = await this.GetExamQuestionsAsync(examID);
                var userAnswers = await this.GetStuAnswerListAsync(userID, examID);
                foreach(var answer in userAnswers)
                {
                    var q = questions.Find(q => q.QuestionId == answer.QuestionId);
                    switch (q.Type)
                    {
                        case QuestionType.True_False:
                        case QuestionType.Single_Choice:
                        case QuestionType.Multi_Choice:
                            if (!string.IsNullOrEmpty(answer.Answer) && answer.Answer == q.RightAnswer)
                            {
                                answer.Mark = q.Mark;
                            }
                            else
                            {
                                answer.Mark = 0;
                            }
                            break;
                        case QuestionType.Fill_In_Blanks:
                            var rightAnswers = await this.ProcessAnswerAsync(q.RightAnswer);
                            var ua = await this.ProcessStuAnswerAsync(answer.Answer);
                            int rightCount = 0;
                            for(int i = 0; i < ua.Count; i++)
                            {
                                if (!string.IsNullOrEmpty(ua[i]) && rightAnswers[i].Contains(ua[i]))
                                {
                                    rightCount++;
                                }
                            }
                            answer.Mark = (rightCount * q.Mark) / ua.Count;
                            break;
                        default:
                            break;
                    }
                }
                dbContext.UserAnswer.UpdateRange(userAnswers);
                await dbContext.SaveChangesAsync();
                _ = this.CalculateStuScoreAsync(userID, examID);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 计算总分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="examID"></param>
        /// <returns></returns>
        public async Task CalculateStuScoreAsync(string userID, int examID)
        {
            try
            {
                var userAnswers = await this.GetStuAnswerListAsync(userID, examID);
                var ue = await this.GetStuExamAsync(userID, examID);
                ue.Mark = userAnswers.Sum(ua => ua.Mark);
                dbContext.UserExam.Update(ue);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// find Exam by id, throws exception if not found
        /// </summary>
        /// <param name="examID">id</param>
        /// <returns></returns>
        public async Task<Exam> GetExamAsync(int examID)
        {
            try
            {
                var exam = await dbContext.Exams.FindAsync(examID);
                if(exam == null)
                {
                    throw new Exception("Unable to Find Exam(id: " + examID + ")!");
                }
                return exam;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        /// <summary>
        /// return UserExam by stuID and examID, returns null if not found
        /// </summary>
        /// <param name="userID">stuID</param>
        /// <param name="examID">examID</param>
        /// <returns>UserExam</returns>
        public async Task<UserExam> GetStuExamAsync(string userID, int examID)
        {
            try
            {
                var ue = await dbContext.UserExam.FindAsync(userID, examID);
                return ue;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        public async Task<List<UserExam>> GetStuExamListAsync(int examID, List<UserCourse> ucList)
        {
            try
            {
                var resList = new List<UserExam>();
                foreach(var uc in ucList)
                {
                    var ue = await this.GetStuExamAsync(uc.UserId, examID);
                    if(ue != null)
                    {
                        resList.Add(ue);
                    }
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<bool> HasStuStartedExamAsync(string userID, int examID)
        {
            try
            {
                var ue = await this.GetStuExamAsync(userID, examID);
                return ue != null;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        public Task<bool> HasStuStartedExamAsync(UserExam ue)
        {
            return Task.Run(() =>
            {
                return ue != null;
            });
        }

        public async Task<bool> HasStuFinishedExamAsync(string userID, int examID)
        {
            try
            {
                var ue = await this.GetStuExamAsync(userID, examID);
                if(ue != null)
                {
                    return ue.Mark != null;
                }
                return false;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw e;
            }
        }

        public Task<bool> HasStuFinishedExamAsync(UserExam ue)
        {
            return Task.Run(() =>
            {
                if (ue != null)
                {
                    return ue.Mark != null;
                }
                return false;
            });
        }

        public async Task<ExamStatistics> GetExamStatisticsAsync(Exam exam)
        {
            try
            {
                var courseStuList = await this.GetCourseStuListAsync(exam.CourseId);
                return await this.GetExamStatisticsAsync(exam, courseStuList);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamStatistics> GetExamStatisticsAsync(Exam exam, List<UserCourse> courseStuList)
        {
            try
            {
                var totalCount = courseStuList.Count;
                int startCount = 0;
                int finishedCount = 0;
                int totalScore = 0;
                double avgScore = 0;
                foreach (var uc in courseStuList)
                {
                    var ue = await this.GetStuExamAsync(uc.UserId, exam.ExamId);
                    if (ue != null)
                    {
                        startCount++;
                        if (ue.Mark != null)
                        {
                            finishedCount++;
                            totalScore += ue.Mark.Value;
                        }
                    }
                }
                if (finishedCount != 0)
                {
                    avgScore = (double)totalScore / finishedCount;
                }
                return new ExamStatistics()
                {
                    TotalCount = totalCount,
                    StartCount = startCount,
                    FinishedCount = finishedCount,
                    AverageMark = avgScore
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public Task<ExamStatistics> GetExamStatisticsAsync(Exam exam, List<UserExam> ueList, int totalCount)
        {
            return Task.Run(() =>
            {
                try
                {
                    int startCount = 0;
                    int finishedCount = ueList.Count;
                    int totalScore = 0;
                    double avgScore = 0;
                    foreach (var ue in ueList)
                    {
                        if (ue.Mark != null)
                        {
                            finishedCount++;
                            totalScore += ue.Mark.Value;
                        }
                    }
                    if (finishedCount != 0)
                    {
                        avgScore = (double)totalScore / finishedCount;
                    }
                    return new ExamStatistics()
                    {
                        TotalCount = totalCount,
                        StartCount = startCount,
                        FinishedCount = finishedCount,
                        AverageMark = avgScore
                    };
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    throw new Exception("Action Failed!");
                }
            });
        }

        public async Task<List<Question>> GetExamQuestionsAsync(int examID)
        {
            try
            {
                var questions = await dbContext.Questions.Where(q => q.ExamId == examID).ToListAsync();
                return questions;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<QuestionDetail>> GetExamQuestionDetailsAsync(int examID, List<UserCourse> stuList, int finishedCount)
        {
            try
            {
                var questions = await this.GetExamQuestionsAsync(examID);
                var resList = new List<QuestionDetail>();
                foreach(var q in questions)
                {
                    resList.Add(new QuestionDetail()
                    {
                        Question = q,
                        Statistics = await this.GetQuestionStatisticsAsync(q, stuList, finishedCount)
                    });
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<QuestionDetail>> GetExamQuestionDetailsAsync(int examID, List<UserCourse> stuList, int finishedCount, List<Question> questions)
        {
            try
            {
                var resList = new List<QuestionDetail>();
                foreach (var q in questions)
                {
                    resList.Add(new QuestionDetail()
                    {
                        Question = q,
                        Statistics = await this.GetQuestionStatisticsAsync(q, stuList, finishedCount)
                    });
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamWithQuestions> GetExamWithQuestionsAsync(Exam exam)
        {
            try
            {
                return new ExamWithQuestions()
                {
                    Exam = exam,
                    Questions = await this.GetExamQuestionsAsync(exam.ExamId)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamWithQuestions> GetExamWithQuestionsAsync(int examID)
        {
            try
            {
                var exam = await this.GetExamAsync(examID);
                return await this.GetExamWithQuestionsAsync(exam);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamWithQuestions> GetExamWithQuestionsAsync(Exam exam, List<Question> questions, bool withAnswer, string userID)
        {
            try
            {
                List<Question> qList = null;
                if (await this.HasStuStartedExamAsync(userID, exam.ExamId))
                {
                    qList = new List<Question>(questions);
                    if (!withAnswer)
                    {
                        foreach (var q in qList)
                        {
                            q.RightAnswer = null;
                        }
                    }
                }

                return new ExamWithQuestions()
                {
                    Exam = exam,
                    Questions = qList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamWithQuestionDetails> GetExamWithQuestionDetailsAsync(Exam exam, List<UserCourse> stuList, int finishedCount)
        {
            try
            {
                return new ExamWithQuestionDetails()
                {
                    Exam = exam,
                    Questions = await this.GetExamQuestionDetailsAsync(exam.ExamId, stuList, finishedCount)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamWithQuestionDetails> GetExamWithQuestionDetailsAsync(Exam exam, List<UserCourse> stuList, int finishedCount, List<Question> questions)
        {
            try
            {
                return new ExamWithQuestionDetails()
                {
                    Exam = exam,
                    Questions = await this.GetExamQuestionDetailsAsync(exam.ExamId, stuList, finishedCount, questions)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamWithQuestionDetails> GetExamWithQuestionDetailsAsync(int examID, List<UserCourse> stuList, int finishedCount)
        {
            try
            {
                var exam = await this.GetExamAsync(examID);
                return await this.GetExamWithQuestionDetailsAsync(exam, stuList, finishedCount);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserAnswer> GetStuAnswerAsync(string stuID, int examID, int qID)
        {
            try
            {
                var ua = await dbContext.UserAnswer.FindAsync(stuID, examID, qID);
                return ua;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<UserAnswer>> GetStuAnswerListAsync(string stuID, int examID)
        {
            try
            {
                var qList = await this.GetExamQuestionsAsync(examID);
                return await this.GetStuAnswerListAsync(stuID, qList);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<UserAnswer>> GetStuAnswerListAsync(string stuID, List<Question> qList)
        {
            try
            {
                var resList = new List<UserAnswer>();
                foreach(var q in qList)
                {
                    var an = await this.GetStuAnswerAsync(stuID, q.ExamId, q.QuestionId);
                    if(an != null)
                    {
                        resList.Add(an);
                    }
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<QuestionStatistics> GetQuestionStatisticsAsync(Question question, List<UserCourse> stuList, int finishedCount)
        {
            try
            {
                int correctCount = 0;
                int totalScore = 0;
                int[] optionCount = null;
                switch (question.Type)
                {
                    case QuestionType.True_False:
                        optionCount = new int[2];
                        break;
                    case QuestionType.Single_Choice:
                    case QuestionType.Multi_Choice:
                        optionCount = new int[8];
                        break;
                    default:
                        break;
                }
                foreach (var uc in stuList)
                {
                    var ua = await this.GetStuAnswerAsync(uc.UserId, question.ExamId, question.QuestionId);
                    if(ua != null)
                    {
                        if(ua.Mark != null)
                        {
                            totalScore += ua.Mark.Value;
                            if(ua.Mark.Value == question.Mark)
                            {
                                correctCount++;
                            }
                        }
                        //process user answer
                        switch (question.Type)
                        {
                            case QuestionType.True_False:
                                if (ua.Answer.Equals("T"))
                                {
                                    optionCount[0]++;
                                }
                                else
                                {
                                    optionCount[1]++;
                                }
                                break;
                            case QuestionType.Single_Choice:
                            case QuestionType.Multi_Choice:
                                foreach(var c in ua.Answer)
                                {
                                    optionCount[c - 'A']++;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                double avgScore = 0;
                if(finishedCount > 0)
                {
                    avgScore = (double)totalScore / finishedCount;
                }
                return new QuestionStatistics()
                {
                    CorrectCount = correctCount,
                    AverageScore = avgScore,
                    OptionCount = optionCount
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<ExamOverview> GetExamOverviewAsync(Exam exam, List<UserCourse> stuList)
        {
            try
            {
                var stat = await this.GetExamStatisticsAsync(exam, stuList);
                return new ExamOverview()
                {
                    Exam = exam,
                    Statistics = stat
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Acion Failed!");
            }
        }

        public async Task<UserExamOverView> GetStuExamOverviewAsync(string stuID, int examID)
        {
            try
            {
                var u = await dbContext.Users.FindAsync(stuID);
                if (u == null)
                {
                    throw new Exception("User Not Found!");
                }
                return new UserExamOverView()
                {
                    UserInfo = u,
                    Exam = await this.GetExamAsync(examID),
                    UserExam = await this.GetStuExamAsync(stuID, examID)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserExamOverView> GetStuExamOverviewAsync(string stuID, Exam exam)
        {
            try
            {
                var u = await dbContext.Users.FindAsync(stuID);
                if (u == null)
                {
                    throw new Exception("User Not Found!");
                }
                return new UserExamOverView()
                {
                    UserInfo = u,
                    Exam = exam,
                    UserExam = await this.GetStuExamAsync(stuID, exam.ExamId)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserExamOverView> GetStuExamOverviewAsync(User u, Exam exam)
        {
            try
            {
                if (u == null)
                {
                    throw new Exception("User Not Found!");
                }
                return new UserExamOverView()
                {
                    UserInfo = u,
                    Exam = exam,
                    UserExam = await this.GetStuExamAsync(u.Id, exam.ExamId)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<UserExamOverView>> GetStuExamOverviewListAsync(Exam exam, List<UserCourse> ucList)
        {
            try
            {
                var resList = new List<UserExamOverView>();
                foreach(var uc in ucList)
                {
                    var user = await dbContext.Users.FindAsync(uc.UserId);
                    if(user != null)
                    {
                        resList.Add(new UserExamOverView()
                        {
                            UserInfo = user,
                            Exam = exam,
                            UserExam = await this.GetStuExamAsync(user.Id, exam.ExamId)
                        });
                    }
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserExamWithAnswers> GetStuExamWithAnswersAsync(User user, Exam exam, List<Question> questions)
        {
            try
            {
                return new UserExamWithAnswers()
                {
                    UserInfo = user,
                    UserExam = await this.GetStuExamAsync(user.Id, exam.ExamId),
                    Answers = await this.GetStuAnswerListAsync(user.Id, questions)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserRole> GetExamRoleAsync(int examID, string userID)
        {
            try
            {
                var exam = await this.GetExamAsync(examID);
                return await this.GetCourseRoleAsync(exam.CourseId, userID);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 教师获取课程考试列表
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public async Task<List<ExamOverview>> GetCourseExamOverviewAsync(int courseID)
        {
            try
            {
                var resList = new List<ExamOverview>();
                var examList = await dbContext.Exams.Where(e => e.CourseId == courseID).ToListAsync();
                var stuList = await this.GetCourseStuListAsync(courseID);
                foreach(var exam in examList)
                {
                    resList.Add(await this.GetExamOverviewAsync(exam, stuList));
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Acion Failed!");
            }
        }

        /// <summary>
        /// 学生获取课程考试列表
        /// </summary>
        /// <param name="stuID"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public async Task<List<UserExamOverView>> GetStuCourseExamOverviewAsync(string stuID, int courseID)
        {
            try
            {
                var resList = new List<UserExamOverView>();
                var user = await dbContext.Users.FindAsync(stuID);
                var examList = await dbContext.Exams.Where(e => e.CourseId == courseID).ToListAsync();
                foreach(var exam in examList)
                {
                    resList.Add(await this.GetStuExamOverviewAsync(user, exam));
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Acion Failed!");
            }
        }

        /// <summary>
        /// 教师获取考试详情
        /// </summary>
        /// <param name="examID"></param>
        /// <returns></returns>
        public async Task<ExamDetail> GetExamDetailAsync(int examID)
        {
            try
            {
                var exam = await this.GetExamAsync(examID);
                var stuList = await this.GetCourseStuListAsync(exam.CourseId);
                var stat = await this.GetExamStatisticsAsync(exam, stuList);
                var questions = await this.GetExamQuestionsAsync(examID);
                return new ExamDetail()
                {
                    ExamWithQuestions = await this.GetExamWithQuestionDetailsAsync(exam, stuList, stat.FinishedCount, questions),
                    Statistics = stat,
                    UserExams = await this.GetStuExamOverviewListAsync(exam, stuList)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 学生获取考试详情
        /// </summary>
        /// <param name="stuID"></param>
        /// <param name="examID"></param>
        /// <returns></returns>
        public async Task<UserExamDetail> GetStuExamDetailAsync(string stuID, int examID)
        {
            try
            {
                var user = await dbContext.Users.FindAsync(stuID);
                if(user == null)
                {
                    throw new Exception("User Not Found!");
                }
                var exam = await this.GetExamAsync(examID);
                var questions = await this.GetExamQuestionsAsync(examID);
                return new UserExamDetail()
                {
                    Exam = await this.GetExamWithQuestionsAsync(exam, questions, await this.HasStuFinishedExamAsync(stuID, examID), stuID),
                    UserExam = await this.GetStuExamWithAnswersAsync(user, exam, questions)
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public Task<List<string>> ProcessStuAnswerAsync(string answer)
        {
            return Task.Run(() =>
            {
                var res = answer.Split(Question.ANSWER_SEPARATOR).ToList();
                return res;
            });
        }

        public Task<List<List<string>>> ProcessAnswerAsync(string answer)
        {
            return Task.Run(() =>
            {
                var answerList = answer.Split(Question.ANSWER_SEPARATOR).ToList();
                var res = new List<List<string>>();
                foreach (var a in answerList)
                {
                    res.Add(a.Split(Question.MUL_SEPARATOR).ToList());
                }
                return res;
            });
        }
    }
}
