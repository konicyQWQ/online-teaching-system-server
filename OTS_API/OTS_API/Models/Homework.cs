using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum HWType
    {
        Individual = 0,
        Group = 1
    }

    public enum HWStatus
    {
        Pending = 0,
        Active = 1,
        Finished = 2
    }

    [Table("homework")]
    public class Homework
    {
        [Key]
        [Column("hw_id")]
        public int HwId { get; set; }
        [Column("course_id")]
        public int CourseId { get; set; }
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
        [Column("type")]
        public HWType Type { get; set; }
        [Column("status")]
        public HWStatus Status { get; set; }
    }

    public class HomeworkStatistics
    {
        public int TotalCount { get; set; }
        public int SubmitCount { get; set; }
        public int ScoredCount { get; set; }
    }

    public class HomeworkOverview
    {
        public Homework Homework { get; set; }
        public HomeworkStatistics Statistics { get; set; }
    }

    public class StuHomeworkOverview
    {
        public Homework Homework { get; set; }
        public UserHomework UserHomework { get; set; }
    }

    public class HomeworkWithFiles
    {
        public Homework Homework { get; set; }
        public List<Models.File> Files { get; set; }
    }

    public class HomeworkDetail
    {
        public HomeworkWithFiles Homework { get; set; }
        public HomeworkStatistics Statistics { get; set; }
        public List<UserHomeworkWithFiles> StuHomeworkList { get; set; }
    }
}
