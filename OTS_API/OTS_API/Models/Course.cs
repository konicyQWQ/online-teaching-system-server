using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum CourseStatus
    {
        Pending = 0,
        Open = 1,
        Closed = 2
    }

    [Table("course")]
    public class Course
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("institute")]
        public string Institute { get; set; }
        [Column("status")]
        public CourseStatus Status { get; set; }
        [Column("year")]
        public int Year { get; set; }
        [Column("startTime")]
        public DateTime StartTime { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("scoringMethod")]
        public string ScoringMethod { get; set; }
        [Column("textbook")]
        public string Textbook { get; set; }
    }

    public class CourseWithTeachers
    {
        public Course Course { get; set; }
        public List<User> Teachers { get; set; }
    }
}
