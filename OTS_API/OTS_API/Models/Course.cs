using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    public enum CourseType
    {
        Normal,
        Advanced,
    }
    public class Course
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Institute { get; set; }
        public int Year { get; set; }
        public DateTime StartTime { get; set; }
        public string Description { get; set; }
        public string ScoringMethod { get; set; }
        public string TextBook { get; set; }
        public string Other { get; set; }
    }

    public class Bulletin
    {
        public string CourseID { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }

    }

    public class Courseware
    {
        public string CourseID { get; set; }
        public File File { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
    }

    public class Homework
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Content { get; set; }
    }
}
