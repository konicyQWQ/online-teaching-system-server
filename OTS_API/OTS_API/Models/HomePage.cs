using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    [Table("home_page")]
    public class HomePage
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Column("file_id")]
        public int FileId { get; set; }
        [Column("url")]
        public string Url { get; set; }
    }
}
