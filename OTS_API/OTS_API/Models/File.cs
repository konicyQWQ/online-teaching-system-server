using System;
using System.Collections.Generic;

namespace OTS_API.Models
{
    public partial class File
    {
        public File()
        {
            Courseware = new HashSet<Courseware>();
            User = new HashSet<User>();
            UserHomework = new HashSet<UserHomework>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public virtual ICollection<Courseware> Courseware { get; set; }
        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<UserHomework> UserHomework { get; set; }
    }
}
