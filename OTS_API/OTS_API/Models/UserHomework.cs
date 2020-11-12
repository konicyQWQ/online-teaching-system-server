using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("user_homework")]
    public class UserHomework
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; }
        [Key]
        [Column("hw_id")]
        public int HwId { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("mark")]
        public int? Mark { get; set; }
    }
}
