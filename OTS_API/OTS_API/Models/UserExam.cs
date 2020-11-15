using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("user_exam")]
    public class UserExam
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; }
        [Key]
        [Column("exam_id")]
        public int ExamId { get; set; }
        [Column("mark")]
        public int? Mark { get; set; }
    }

    /// <summary>
    /// 用户信息+用户考试信息+用户答案
    /// </summary>
    public class UserExamWithAnswers
    {
        public User UserInfo { get; set; }
        public UserExam UserExam { get; set; }
        public List<UserAnswer> Answers { get; set; }
    }

    /// <summary>
    /// 考试信息+题目列表+用户考试信息+用户答案,考试前题目列表的RightAnswer应置null,考试后可显示
    /// </summary>
    public class UserExamDetail
    {
        public ExamWithQuestions Exam { get; set; }
        public UserExamWithAnswers UserExam { get; set; }
    }

    /// <summary>
    /// 考试信息+本人考试信息
    /// </summary>
    public class UserExamOverView
    {
        public User UserInfo { get; set; }
        public Exam Exam { get; set; }
        public UserExam UserExam { get; set; }
    }
}
