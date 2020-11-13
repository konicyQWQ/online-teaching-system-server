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
                return new UserHomeworkWithFiles()
                {
                    UserInfo = null,
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
    }
}
