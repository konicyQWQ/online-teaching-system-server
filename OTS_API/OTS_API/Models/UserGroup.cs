using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum GroupIdentity
    {
        Member = 0,
        Leader = 1
    }

    [Table("user_group")]
    public class UserGroup
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }
        [Key]
        [Column("user_id")]
        public string UserId { get; set; }
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("identity")]
        public GroupIdentity Identity { get; set; }
    }

    public class GroupMemberInfo
    {
        public User UserInfo { get; set; }
        public UserGroup UserGroup { get; set; }
    }
}
