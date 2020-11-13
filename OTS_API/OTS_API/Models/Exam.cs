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
}
