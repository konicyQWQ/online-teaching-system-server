using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("course_group")]
    public class CourseGroup
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("max_count")]
        public int MaxCount { get; set; }
    }

    public class GroupInfo
    {
        public CourseGroup CourseGroup { get; set; }
        public int GroupMemberCount { get; set; }
        public List<GroupMemberInfo> Members { get; set; }
    }
}
