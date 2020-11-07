using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    public enum Privilege
    {
        NotDownloadable = 0,
        Downloadable = 1
    }

    [Table("courseware")]
    public class Courseware
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("file_id")]
        public int FileId { get; set; }
        [Column("privilege")]
        public Privilege Privilege { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}
