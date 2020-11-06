using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class UserCourse
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public byte UserRole { get; set; }

        public virtual Course Course { get; set; }
        public virtual User User { get; set; }
    }
}
