using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class CourseGroup
    {
        public CourseGroup()
        {
            UserGroup = new HashSet<UserGroup>();
        }

        public int GroupId { get; set; }
        public int CourseId { get; set; }
        public int PreCount { get; set; }
        public int MaxCount { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<UserGroup> UserGroup { get; set; }
    }
}
