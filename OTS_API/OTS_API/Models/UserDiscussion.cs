using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public partial class UserDiscussion
    {
        public int DiscussionId { get; set; }
        public int Level { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }

        public virtual User User { get; set; }
    }
}
