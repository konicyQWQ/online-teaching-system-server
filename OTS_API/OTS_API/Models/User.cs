using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    public enum UserRole
    {
        Teacher,
        Student,
        Admin,
        Unknown,
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class User
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int AvatarID { get; set; }
        public Gender Gender { get; set; }
        public int Grade { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
