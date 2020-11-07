using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public partial class UserExam
    {
        public string UserId { get; set; }
        public int ExamId { get; set; }
        public int Mark { get; set; }

        public virtual User User { get; set; }
    }
}
