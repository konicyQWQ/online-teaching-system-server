using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("user_course")]
    public class UserCourse
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; }
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("user_role")]
        public UserRole UserRole { get; set; }
    }
}
