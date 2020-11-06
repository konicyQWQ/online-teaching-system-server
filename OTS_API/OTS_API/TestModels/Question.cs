using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class Question
    {
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public byte Type { get; set; }
        public string Content { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string RightAnswer { get; set; }
        public int Mark { get; set; }
    }
}
