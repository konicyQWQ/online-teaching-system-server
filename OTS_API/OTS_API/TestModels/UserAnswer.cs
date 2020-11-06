using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class UserAnswer
    {
        public string UserId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }

        public virtual User User { get; set; }
    }
}
