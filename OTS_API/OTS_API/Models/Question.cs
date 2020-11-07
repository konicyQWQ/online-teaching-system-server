using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("question")]
    public class Question
    {
        [Key]
        [Column("question_id")]
        public int QuestionId { get; set; }
        [Key]
        [Column("exam_id")]
        public int ExamId { get; set; }
        public byte Type { get; set; }
        public string Content { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string RightAnswer { get; set; }
        public int Mark { get; set; }
    }
}
