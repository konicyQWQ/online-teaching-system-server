using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public partial class Discussion
    {
        public int CourseId { get; set; }
        public int DiscussionId { get; set; }
        public string Title { get; set; }
    }
}
