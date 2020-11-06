using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class UserGroup
    {
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public int Identity { get; set; }

        public virtual CourseGroup Group { get; set; }
        public virtual User User { get; set; }
    }
}
