using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("exam")]
    public class Exam
    {
        [Key]
        [Column("exam_id")]
        public int ExamId { get; set; }
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("max_mark")]
        public int MaxMark { get; set; }
        [Column("percentage")]
        public int Percentage { get; set; }
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("duration")]
        public int Duration { get; set; }
    }

    //View Models

    /// <summary>
    /// 考试基本信息+题目列表，用于老师设计试卷时以及学生作答时，对于Stu，Question的RightAnswer应置null
    /// </summary>
    public class ExamWithQuestions
    {
        public Exam Exam { get; set; }
        public List<Question> Questions { get; set; }
    }

    /// <summary>
    /// 考试基本信息+题目列表+题目数据，用于老师考试中和考试后查看
    /// </summary>
    public class ExamWithQuestionDetails
    {
        public Exam Exam { get; set; }
        public List<QuestionDetail> Questions { get; set; }
    }

    /// <summary>
    /// 考试数据，用于老师查看
    /// </summary>
    public class ExamStatistics
    {
        public int TotalCount { get; set; }
        public int StartCount { get; set; }
        public int FinishedCount { get; set; }
        public double AverageMark { get; set; }
    }

    /// <summary>
    /// 考试简略信息，包括考试基本信息和考试数据，老师在课程测试列表查看
    /// </summary>
    public class ExamOverview
    {
        public Exam Exam { get; set; }
        public ExamStatistics Statistics { get; set; }
    }

    /// <summary>
    /// 考试详细信息，包括考试基本信息+考试数据+题目列表+题目数据，用于老师在考试详情查看
    /// </summary>
    public class ExamDetail
    {
        public ExamWithQuestionDetails ExamWithQuestions { get; set; }
        public ExamStatistics Statistics { get; set; }
        public List<UserExamOverView> UserExams { get; set; }
    }
}
