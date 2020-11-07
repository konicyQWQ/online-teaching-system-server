using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public enum Pirvilege
    {
        NotDownloadable = 0,
        Downloadable = 1
    }

    public class Courseware
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int FileId { get; set; }
        public Pirvilege Privilege { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
    }
}
