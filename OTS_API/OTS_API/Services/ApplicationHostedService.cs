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
using Newtonsoft.Json;

namespace OTS_API.Services
{
    public class ExamTime
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int CourseID { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public ExamStatus Status { get; set; }
    }

    public class HWTime
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int CourseID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public HWStatus Status { get; set; }
    }

    public class QuestionAnswer
    {
        public int ID { get; set; }
        public int ExamID { get; set; }
        public QuestionType Type { get; set; }
        public string RightAnswer { get; set; }
        public int Mark { get; set; }
    }

    public class ApplicationHostedService : BackgroundService
    {
        private readonly ILogger<ApplicationHostedService> logger;
        private MySqlConnection sqlConnection;

        public ApplicationHostedService(ILogger<ApplicationHostedService> logger)
        {
            this.logger = logger;
            this.sqlConnection = new MySqlConnection(Config.connStr);
            try
            {
                sqlConnection.Open();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message + "\nFatal Error! Cannot connect to sql!");
            }
        }

        ~ApplicationHostedService()
        {
            this.sqlConnection.Close();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Hosted Service Starting!");
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckPendingHWAsync();
                await CheckActiveHWAsync();
                await CheckNearDDLHWAsync();

                await CheckPendingExamsAsync();
                await CheckActiveExamsAsync();

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            logger.LogInformation("Hosted Service Stopping!");
        }

        private async Task CheckPendingExamsAsync()
        {
            try
            {
                var examList = await this.GetExamTimeListAsync(ExamStatus.Pending);
                foreach(var exam in examList)
                {
                    if(DateTime.Now >= exam.StartTime)
                    {
                        await this.SetExamStatusAsync(exam.ID, ExamStatus.Active);
                        await this.AddExamOpenEventAsync(exam);
                    }
                }
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
                var examList = await this.GetExamTimeListAsync(ExamStatus.Active);
                var finishedList = new List<int>();
                foreach (var exam in examList)
                {
                    if (DateTime.Now >= exam.StartTime.AddMinutes(exam.Duration))
                    {
                        await this.SetExamStatusAsync(exam.ID, ExamStatus.Finished);
                        await this.ForceFinishExamAsync(exam.ID);
                        finishedList.Add(exam.ID);
                    }
                }
                foreach(var examID in finishedList)
                {
                    await this.AutoSetExamScoreAsync(examID);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task CheckPendingHWAsync()
        {
            try
            {
                var hwList = await this.GetHWTimeListAsync(HWStatus.Pending);
                foreach(var hw in hwList)
                {
                    if(DateTime.Now >= hw.StartTime)
                    {
                        await this.SetHWStatusAsync(hw.ID, HWStatus.Active);
                        await this.AddHWOpenEventAsync(hw);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task CheckActiveHWAsync()
        {
            try
            {
                var hwList = await this.GetHWTimeListAsync(HWStatus.Active);
                foreach (var hw in hwList)
                {
                    if (DateTime.Now.AddHours(Config.NearWindow) > hw.EndTime)
                    {
                        await this.SetHWStatusAsync(hw.ID, HWStatus.NearDDL);
                        await this.AddHWNearDDLEventAsync(hw);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task CheckNearDDLHWAsync()
        {
            try
            {
                var hwList = await this.GetHWTimeListAsync(HWStatus.NearDDL);
                foreach (var hw in hwList)
                {
                    if (DateTime.Now > hw.EndTime)
                    {
                        await this.SetHWStatusAsync(hw.ID, HWStatus.Finished);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task ForceFinishExamAsync(int examID)
        {
            try
            {
                var stuList = await this.GetStuExamListAsync(examID);
                foreach(var stu in stuList)
                {
                    if(stu.Mark == null)
                    {
                        stu.Mark = 0;
                        await this.UpdateStuExamAsync(stu);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task AutoSetExamScoreAsync(int examID)
        {
            try
            {
                var questionList = await this.GetQuestionAnswerListAsync(examID);
                var stuList = await this.GetStuExamListAsync(examID);
                foreach(var stu in stuList)
                {
                    await this.AutoSetStuExamScoreAsync(stu.UserId, examID, questionList);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task AutoSetStuExamScoreAsync(string stuID, int examID, List<QuestionAnswer> questions)
        {
            try
            {
                var stuAnswers = await this.GetStuAnswerListAsync(examID, stuID);
                foreach(var answer in stuAnswers)
                {
                    var q = questions.Find(q => q.ID == answer.QuestionId);
                    if(q == null)
                    {
                        continue;
                    }
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
                            for (int i = 0; i < ua.Count; i++)
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
                await this.UpdateUserAnswersAsync(stuAnswers);
                await this.CalculateStuScore(stuID, examID);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task CalculateStuScore(string stuID, int examID)
        {
            try
            {
                var answerList = await this.GetStuAnswerListAsync(examID, stuID);
                var totalScore = 0;
                foreach(var an in answerList)
                {
                    if(an.Mark != null)
                    {
                        totalScore += an.Mark.Value;
                    }
                }
                var ueToUpdate = new UserExam()
                {
                    UserId = stuID,
                    ExamId = examID,
                    Mark = totalScore
                };
                await this.UpdateStuExamAsync(ueToUpdate);
                await this.AddExamGradedEventAsync(ueToUpdate);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private Task<List<string>> ProcessStuAnswerAsync(string answer)
        {
            return Task.Run(() =>
            {
                var res = answer.Split(Question.ANSWER_SEPARATOR).ToList();
                return res;
            });
        }

        private Task<List<List<string>>> ProcessAnswerAsync(string answer)
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

        private async Task<List<ExamTime>> GetExamTimeListAsync(ExamStatus status)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select exam_id, course_id, title, start_time, duration from exam where status = @status";
                cmd.Parameters.Add("@status", MySqlDbType.Int16);
                cmd.Parameters["@status"].Value = (int)status;
                using var reader = await cmd.ExecuteReaderAsync();
                var resList = new List<ExamTime>();
                while(await reader.ReadAsync())
                {
                    var examID = Convert.ToInt32(reader["exam_id"]);
                    var title = Convert.ToString(reader["title"]);
                    var courseID = Convert.ToInt32(reader["course_id"]);
                    var startTime = Convert.ToDateTime(reader["start_time"]);
                    var duration = Convert.ToInt32(reader["duration"]);
                    resList.Add(new ExamTime()
                    {
                        ID = examID,
                        Title = title,
                        CourseID = courseID,
                        StartTime = startTime,
                        Duration = duration,
                        Status = status
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

        private async Task<ExamTime> GetExamAsync(int id)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select course_id, title from exam where exam_id = @exam_id";
                cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                cmd.Parameters["@exam_id"].Value = id;
                using var reader = await cmd.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                {
                    var title = Convert.ToString(reader["title"]);
                    var courseID = Convert.ToInt32(reader["course_id"]);
                    return new ExamTime()
                    {
                        ID = id,
                        CourseID = courseID,
                        Title = title
                    };
                }
                throw new Exception("Unable to Find Exam ID: " + id);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task<List<HWTime>> GetHWTimeListAsync(HWStatus status)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select hw_id, course_id, title, startTime, endTime from homework where status = @status";
                cmd.Parameters.Add("@status", MySqlDbType.Int16);
                cmd.Parameters["@status"].Value = (int)status;
                using var reader = await cmd.ExecuteReaderAsync();
                var resList = new List<HWTime>();
                while (await reader.ReadAsync())
                {
                    var id = Convert.ToInt32(reader["hw_id"]);
                    var title = Convert.ToString(reader["title"]);
                    var courseID = Convert.ToInt32(reader["course_id"]);
                    var startTime = Convert.ToDateTime(reader["startTime"]);
                    var endTime = Convert.ToDateTime(reader["endTime"]);
                    resList.Add(new HWTime()
                    {
                        ID = id,
                        Title = title,
                        CourseID = courseID,
                        StartTime = startTime,
                        EndTime = endTime,
                        Status = status
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

        private async Task<string> GetCourseNameAsync(int id)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select name from course where id = @id";
                cmd.Parameters.Add("@id", MySqlDbType.Int32);
                cmd.Parameters["@id"].Value = id;
                using var reader = await cmd.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                {
                    return Convert.ToString(reader["name"]);
                }
                throw new Exception("Unable to Find Course ID: " + id);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task SetExamStatusAsync(int examID, ExamStatus status)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "update exam set status = @status where exam_id = @exam_id";
                cmd.Parameters.Add("@status", MySqlDbType.Int16);
                cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                cmd.Parameters["@status"].Value = (int)status;
                cmd.Parameters["@exam_id"].Value = examID;
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task SetHWStatusAsync(int hwID, HWStatus status)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "update homework set status = @status where hw_id = @hw_id";
                cmd.Parameters.Add("@status", MySqlDbType.Int16);
                cmd.Parameters.Add("@hw_id", MySqlDbType.Int32);
                cmd.Parameters["@status"].Value = (int)status;
                cmd.Parameters["@hw_id"].Value = hwID;
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task<List<UserExam>> GetStuExamListAsync(int examID)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select user_id, mark from user_exam where exam_id = @exam_id";
                cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                cmd.Parameters["@exam_id"].Value = examID;
                using var reader = await cmd.ExecuteReaderAsync();
                var resList = new List<UserExam>();
                while(await reader.ReadAsync())
                {
                    var userID = Convert.ToString(reader["user_id"]);
                    int? mark;
                    if (Convert.IsDBNull(reader["mark"]))
                    {
                        mark = null;
                    }
                    else
                    {
                        mark = Convert.ToInt32(reader["mark"]);
                    }
                    resList.Add(new UserExam()
                    {
                        UserId = userID,
                        ExamId = examID,
                        Mark = mark
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

        private async Task UpdateStuExamAsync(UserExam ue)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "update user_exam set mark = @mark where exam_id = @exam_id and user_id = @user_id";
                cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                cmd.Parameters.Add("@user_id", MySqlDbType.VarChar);
                cmd.Parameters.Add("@mark", MySqlDbType.Int32);
                cmd.Parameters["@exam_id"].Value = ue.ExamId;
                cmd.Parameters["@user_id"].Value = ue.UserId;
                cmd.Parameters["@mark"].Value = ue.Mark;
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task<List<UserAnswer>> GetStuAnswerListAsync(int examID, string stuID)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select question_id, answer, mark from user_answer where exam_id = @exam_id and user_id = @user_id";
                cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                cmd.Parameters.Add("@user_id", MySqlDbType.VarChar);
                cmd.Parameters["@exam_id"].Value = examID;
                cmd.Parameters["@user_id"].Value = stuID;
                using var reader = await cmd.ExecuteReaderAsync();
                var resList = new List<UserAnswer>();
                while(await reader.ReadAsync())
                {
                    var qID = Convert.ToInt32(reader["question_id"]);
                    var answer = Convert.ToString(reader["answer"]);
                    int? mark;
                    if (Convert.IsDBNull(reader["mark"]))
                    {
                        mark = null;
                    }
                    else
                    {
                        mark = Convert.ToInt32(reader["mark"]);
                    }
                    resList.Add(new UserAnswer()
                    {
                        UserId = stuID,
                        ExamId = examID,
                        QuestionId = qID,
                        Answer = answer,
                        Mark = mark
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

        private async Task UpdateUserAnswersAsync(List<UserAnswer> answers)
        {
            try
            {
                foreach(var an in answers)
                {
                    if(an.Mark == null)
                    {
                        continue;
                    }
                    var cmd = this.sqlConnection.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "update user_answer set mark = @mark where user_id = @user_id and exam_id = @exam_id and question_id = @question_id";
                    cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                    cmd.Parameters.Add("@user_id", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@question_id", MySqlDbType.Int32);
                    cmd.Parameters.Add("@mark", MySqlDbType.Int32);
                    cmd.Parameters["@exam_id"].Value = an.ExamId;
                    cmd.Parameters["@user_id"].Value = an.UserId;
                    cmd.Parameters["@question_id"].Value = an.QuestionId;
                    cmd.Parameters["@mark"].Value = an.Mark;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task<List<QuestionAnswer>> GetQuestionAnswerListAsync(int examID)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select question_id, type, right_answer, mark from question where exam_id = @exam_id";
                cmd.Parameters.Add("@exam_id", MySqlDbType.Int32);
                cmd.Parameters["@exam_id"].Value = examID;
                using var reader = await cmd.ExecuteReaderAsync();
                var resList = new List<QuestionAnswer>();
                while (await reader.ReadAsync())
                {
                    var id = Convert.ToInt32(reader["question_id"]);
                    var type = (QuestionType)Convert.ToInt32(reader["type"]);
                    var rA = Convert.ToString(reader["right_answer"]);
                    var mark = Convert.ToInt32(reader["mark"]);
                    resList.Add(new QuestionAnswer()
                    {
                        ID = id,
                        ExamID = examID,
                        Type = type,
                        RightAnswer = rA,
                        Mark = mark
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

        private async Task AddEventAsync(Event ev)
        {
            try
            {
                var cmd = this.sqlConnection.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "insert into event values(0, @event_type, @content, @course_id, @course_name, @related_user, @time)";
                cmd.Parameters.Add("@event_type", MySqlDbType.Int16);
                cmd.Parameters.Add("@content", MySqlDbType.VarChar);
                cmd.Parameters.Add("@course_id", MySqlDbType.Int32);
                cmd.Parameters.Add("@course_name", MySqlDbType.VarChar);
                cmd.Parameters.Add("@related_user", MySqlDbType.VarChar);
                cmd.Parameters.Add("@time", MySqlDbType.DateTime);

                cmd.Parameters["@event_type"].Value = (int)ev.EventType;
                cmd.Parameters["@content"].Value = ev.Content;
                cmd.Parameters["@course_id"].Value = ev.CourseID;
                cmd.Parameters["@course_name"].Value = ev.CourseName;
                cmd.Parameters["@related_user"].Value = ev.RelatedUser;
                cmd.Parameters["@time"].Value = ev.Time;

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task AddHWOpenEventAsync(HWTime hw)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.HomeworkOpen,
                    Content = JsonConvert.SerializeObject(new { Id = hw.ID, Title = hw.Title }),
                    CourseID = hw.CourseID,
                    CourseName = await this.GetCourseNameAsync(hw.CourseID),
                    Time = DateTime.Now
                };
                await this.AddEventAsync(e);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task AddHWNearDDLEventAsync(HWTime hw)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.HomeworkNearDDL,
                    Content = JsonConvert.SerializeObject(new { Id = hw.ID, Title = hw.Title }),
                    CourseID = hw.CourseID,
                    CourseName = await this.GetCourseNameAsync(hw.CourseID),
                    Time = DateTime.Now
                };
                await this.AddEventAsync(e);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task AddExamOpenEventAsync(ExamTime ex)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.ExamOpen,
                    Content = JsonConvert.SerializeObject(new { Id = ex.ID, Title = ex.Title }),
                    CourseID = ex.CourseID,
                    CourseName = await this.GetCourseNameAsync(ex.CourseID),
                    Time = DateTime.Now
                };
                await this.AddEventAsync(e);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        private async Task AddExamGradedEventAsync(UserExam ue)
        {
            try
            {
                var exam = await this.GetExamAsync(ue.ExamId);
                var e = new Event()
                {
                    EventType = EventType.ExamGraded,
                    Content = JsonConvert.SerializeObject(new { Id = exam.ID, Title = exam.Title, Score = ue.Mark.Value }),
                    CourseID = exam.CourseID,
                    CourseName = await this.GetCourseNameAsync(exam.CourseID),
                    RelatedUser = ue.UserId,
                    Time = DateTime.Now
                };
                await this.AddEventAsync(e);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }
    }
}
