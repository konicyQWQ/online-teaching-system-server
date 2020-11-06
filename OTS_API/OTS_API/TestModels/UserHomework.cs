using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class UserHomework
    {
        public string UserId { get; set; }
        public int HwId { get; set; }
        public int FileId { get; set; }
        public string Desription { get; set; }
        public int? Mark { get; set; }

        public virtual File File { get; set; }
        public virtual User User { get; set; }
    }
}
