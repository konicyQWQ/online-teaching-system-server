using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum QuestionType
    {
        True_False = 0,
        Multi_Choice = 1,
        Fill_In_Blanks = 2,
        Sub_Question = 3
    }

    [Table("question")]
    public class Question
    {
        [Key]
        [Column("question_id")]
        public int QuestionId { get; set; }
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
        [Column("right_answer")]
        public string RightAnswer { get; set; }
        [Column("mark")]
        public int Mark { get; set; }
    }
}
