using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("user_answer")]
    public class UserAnswer
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; }
        [Key]
        [Column("exam_id")]
        public int ExamId { get; set; }
        [Key]
        [Column("question_id")]
        public int QuestionId { get; set; }
        [Column("answer")]
        public string Answer { get; set; }
    }
}
