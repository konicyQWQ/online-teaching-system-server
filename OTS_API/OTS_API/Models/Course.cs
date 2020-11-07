using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public enum CourseStatus
    {
        Pending = 0,
        Open = 1,
        Closed = 2
    }

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Institute { get; set; }
        public CourseStatus Status { get; set; }
        public int Year { get; set; }
        public DateTime StartTime { get; set; }
        public string Description { get; set; }
        public string ScoringMethod { get; set; }
        public string Textbook { get; set; }
    }
}
