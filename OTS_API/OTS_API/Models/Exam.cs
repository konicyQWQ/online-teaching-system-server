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

    public class ExamWithQuestions
    {
        public Exam Exam { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class ExamStatistics
    {
        public int TotalCount { get; set; }
        public int StartCount { get; set; }
        public int FinishedCount { get; set; }
        public double AverageMark { get; set; }
    }

    public class ExamOverview
    {
        public Exam Exam { get; set; }
        public ExamStatistics Statistics { get; set; }
    }

    public class ExamDetail
    {
        public ExamWithQuestions ExamWithQuestions { get; set; }
        public ExamStatistics Statistics { get; set; }
    }
}
