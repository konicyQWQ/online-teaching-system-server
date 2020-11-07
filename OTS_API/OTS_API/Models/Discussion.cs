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
        [Column("course_id")]
        public int CourseId { get; set; }
        [Key]
        [Column("discussion_id")]
        public int DiscussionId { get; set; }
        [Column("title")]
        public string Title { get; set; }
    }
}
