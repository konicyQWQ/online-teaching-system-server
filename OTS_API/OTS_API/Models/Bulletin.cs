using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("bulletin")]
    public class Bulletin
    {
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }
        [Key]
        [Column("bulletin_id")]
        public int BulletinId { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("content")]
        public string Content { get; set; }
    }
}
