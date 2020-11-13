﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    public enum UserRole
    {
        Student = 0,
        Teacher = 1,
        Admin = 2,
        Unknown = 3,
        TA = 4
    }

    public enum Gender
    {
        Male,
        Female
    }

    [Table("user")]
    public class User
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("gender")]
        public Gender Gender { get; set; }
        [Column("grade")]
        public byte Grade { get; set; }
        [Column("department")]
        public string Department { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("role")]
        public UserRole Role { get; set; }
        [Column("avatar_id")]
        public int? AvatarId { get; set; }
        [Column("introduction")]
        public string Introduction { get; set; }
    }

    public class UserResList
    {
        public int TotalCount { get; set; }
        public List<User> ResList { get; set; }
    }

    public class UserCourseResList
    {
        public List<CourseWithTeachers> CourseList { get; set; }
        public List<CourseWithTeachers> TeachList { get; set; }
        public List<CourseWithTeachers> AssistList { get; set; }
    }
}
