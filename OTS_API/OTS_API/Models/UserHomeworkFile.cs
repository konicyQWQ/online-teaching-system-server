using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    [Table("user_homework_file")]
    public class UserHomeworkFile
    {
        [Key]
        [Column("file_id")]
        public int FileID { get; set; }
        [Column("user_id")]
        public string UserID { get; set; }
        [Column("hw_id")]
        public int HwID { get; set; }
    }
}
