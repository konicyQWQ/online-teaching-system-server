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
        public int Mark { get; set; }
    }
}
