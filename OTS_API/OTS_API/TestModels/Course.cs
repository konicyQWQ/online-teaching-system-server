using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class Course
    {
        public Course()
        {
            Bulletin = new HashSet<Bulletin>();
            CourseGroup = new HashSet<CourseGroup>();
            Courseware = new HashSet<Courseware>();
            Discussion = new HashSet<Discussion>();
            Exam = new HashSet<Exam>();
            Homework = new HashSet<Homework>();
            UserCourse = new HashSet<UserCourse>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Institute { get; set; }
        public byte Status { get; set; }
        public int Year { get; set; }
        public DateTime StartTime { get; set; }
        public string Description { get; set; }
        public string ScoringMethod { get; set; }
        public string Textbook { get; set; }

        public virtual ICollection<Bulletin> Bulletin { get; set; }
        public virtual ICollection<CourseGroup> CourseGroup { get; set; }
        public virtual ICollection<Courseware> Courseware { get; set; }
        public virtual ICollection<Discussion> Discussion { get; set; }
        public virtual ICollection<Exam> Exam { get; set; }
        public virtual ICollection<Homework> Homework { get; set; }
        public virtual ICollection<UserCourse> UserCourse { get; set; }
    }
}
