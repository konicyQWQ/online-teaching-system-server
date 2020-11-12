using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    [Table("homework_file")]
    public class HomeworkFile
    {
        [Key]
        [Column("hw_id")]
        public int HwID { get; set; }
        [Key]
        [Column("file_id")]
        public int FileID { get; set; }
    }
}
