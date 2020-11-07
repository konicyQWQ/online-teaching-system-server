using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTS_API.Models
{
    [Table("user_group")]
    public class UserGroup
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }
        [Key]
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("identity")]
        public int Identity { get; set; }
    }
}
