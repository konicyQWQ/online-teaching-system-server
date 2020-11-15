using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum QuestionType
    {
        True_False = 0,
        Single_Choice = 1,
        Multi_Choice = 2,
        Fill_In_Blanks = 3,
        Sub_Question = 4
    }

    [Table("question")]
    public class Question
    {
        public const string ANSWER_SEPERATOR = "!&!";
        public const string MUL_SEPERATOR = "!|!";

        [Key]
        [Column("question_id")]
        public int QuestionId { get; set; }
        [Key]
        [Column("exam_id")]
        public int ExamId { get; set; }
        [Column("type")]
        public QuestionType Type { get; set; }
        [Column("content")]
        public string Content { get; set; }
        [Column("optionA")]
        public string OptionA { get; set; }
        [Column("optionB")]
        public string OptionB { get; set; }
        [Column("optionC")]
        public string OptionC { get; set; }
        [Column("optionD")]
        public string OptionD { get; set; }
        [Column("optionE")]
        public string OptionE { get; set; }
        [Column("optionF")]
        public string OptionF { get; set; }
        [Column("optionG")]
        public string OptionG { get; set; }
        [Column("optionH")]
        public string OptionH { get; set; }
        [Column("right_answer")]
        public string RightAnswer { get; set; }
        [Column("mark")]
        public int Mark { get; set; }
    }

    public class QuestionStatistics
    {
        public int CorrectCount { get; set; }
        public double AverageScore { get; set; }
        public int[] OptionCount { get; set; }
    }

    public class QuestionDetail
    {
        public Question Question { get; set; }
        public QuestionStatistics Statistics { get; set; }
    }
}
