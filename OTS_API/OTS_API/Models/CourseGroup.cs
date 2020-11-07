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
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("pre_count")]
        public int PreCount { get; set; }
        [Column("max_count")]
        public int MaxCount { get; set; }
    }
}
