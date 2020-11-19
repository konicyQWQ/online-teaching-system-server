using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    public enum TeachingStatus
    {
        Teaching = 1,
        NotTeaching = 0
    }

    [Table("teacher_page")]
    public class TeacherPage
    {
        [Key]
        [Column("teacher_id")]
        public string ID { get; set; }
        [Column("personal_url")]
        public string PersonalUrl { get; set; }
        [Column("office_place")]
        public string OfficePlace { get; set; }
        [Column("teaching_age")]
        public int TeachingAge { get; set; }
        [Column("positional_title")]
        public string PosTitle { get; set; }
        [Column("status")]
        public TeachingStatus Status { get; set; }
    }

    public class TeacherDetail
    {
        public User TeacherInfo { get; set; }
        public TeacherPage TeacherPage { get; set; }
    }
}
