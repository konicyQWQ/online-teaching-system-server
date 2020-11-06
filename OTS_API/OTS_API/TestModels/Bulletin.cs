using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class Bulletin
    {
        public int CourseId { get; set; }
        public int BulletinId { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public virtual Course Course { get; set; }
    }
}
