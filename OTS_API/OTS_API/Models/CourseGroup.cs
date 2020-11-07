using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public class CourseGroup
    {
        public int GroupId { get; set; }
        public int CourseId { get; set; }
        public int PreCount { get; set; }
        public int MaxCount { get; set; }
    }
}
