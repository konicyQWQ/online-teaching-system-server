using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public class Bulletin
    {
        public int CourseId { get; set; }
        public int BulletinId { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
