using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public partial class Exam
    {
        public int ExamId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int MaxMark { get; set; }

        public virtual Course Course { get; set; }
    }
}
