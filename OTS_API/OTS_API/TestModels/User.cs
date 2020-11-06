using System;
using System.Collections.Generic;

namespace OTS_API.TestModels
{
    public partial class User
    {
        public User()
        {
            UserAnswer = new HashSet<UserAnswer>();
            UserCourse = new HashSet<UserCourse>();
            UserDiscussion = new HashSet<UserDiscussion>();
            UserExam = new HashSet<UserExam>();
            UserGroup = new HashSet<UserGroup>();
            UserHomework = new HashSet<UserHomework>();
        }

        public string Id { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public byte Grade { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public byte? Role { get; set; }
        public int? AvatarId { get; set; }
        public string Introduction { get; set; }

        public virtual File Avatar { get; set; }
        public virtual ICollection<UserAnswer> UserAnswer { get; set; }
        public virtual ICollection<UserCourse> UserCourse { get; set; }
        public virtual ICollection<UserDiscussion> UserDiscussion { get; set; }
        public virtual ICollection<UserExam> UserExam { get; set; }
        public virtual ICollection<UserGroup> UserGroup { get; set; }
        public virtual ICollection<UserHomework> UserHomework { get; set; }
    }
}
