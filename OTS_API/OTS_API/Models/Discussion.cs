using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("discussion")]
    public class Discussion
    {
        [Key]
        [Column("discussion_id")]
        public int DiscussionId { get; set; }
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("creator_id")]
        public string CreatorID { get; set; }
        [Column("create_time")]
        public DateTime CreateTime { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }

    public class DiscussionWithCreatorInfo
    {
        public User CreatorInfo { get; set; }
        public Discussion Discussion { get; set; }
    }

    public class DiscussionDetail
    {
        public DiscussionWithCreatorInfo Discussion { get; set; }
        public List<UserDiscussionWithUserInfo> UserDiscussionList { get; set; }
    }
}
