using System;
using System.Collections.Generic;
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
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class User
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public byte Grade { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public int? AvatarId { get; set; }
        public string Introduction { get; set; }
    }
}
