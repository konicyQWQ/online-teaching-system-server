using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("homework")]
    public class Homework
    {
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }
        [Key]
        [Column("hw_id")]
        public int HwId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("startTime")]
        public DateTime StartTime { get; set; }
        [Column("endTime")]
        public DateTime EndTime { get; set; }
        [Column("content")]
        public string Content { get; set; }
        [Column("total_mark")]
        public int TotalMark { get; set; }
        [Column("percentage")]
        public int Percentage { get; set; }
    }
}
