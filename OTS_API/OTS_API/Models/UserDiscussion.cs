using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("user_discussion")]
    public class UserDiscussion
    {
        [Key]
        [Column("discussion_id")]
        public int DiscussionId { get; set; }
        [Key]
        [Column("level")]
        public int Level { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("content")]
        public string Content { get; set; }
        [Column("submit_time")]
        public DateTime SubmitTime { get; set; }
    }

    public class UserDiscussionWithUserInfo
    {
        public User UserInfo { get; set; }
        public UserDiscussion UserDiscussion { get; set; }
    }
}
