using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class Courseware
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int FileId { get; set; }
        public byte Privilege { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }

        public virtual Course Course { get; set; }
        public virtual File File { get; set; }
    }
}
