using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum PassStatus
    {
        Passed = 1,
        Denined = 0
    }

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
        [Column("pass")]
        public PassStatus PassStatus { get; set; }
    }
}
