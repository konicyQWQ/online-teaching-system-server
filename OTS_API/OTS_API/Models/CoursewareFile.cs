using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    [Table("courseware_file")]
    public class CoursewareFile
    {
        [Key]
        [Column("courseware_id")]
        public int CoursewareId { get; set; }
        [Key]
        [Column("file_id")]
        public int FileId { get; set; }
    }
}
