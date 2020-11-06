using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class Homework
    {
        public int CourseId { get; set; }
        public int HwId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int TotalMark { get; set; }
        public int Percentage { get; set; }

        public virtual Course Course { get; set; }
    }
}
