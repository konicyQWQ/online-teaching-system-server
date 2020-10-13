using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;

namespace OTS_API.Services
{
    public class AuthenticateService
    {
        public List<User> Users { get; set; }

        public AuthenticateService()
        {
            this.Users = new List<User>
            {
                new User
                {
                    Name = "Jack",
                    Password = "Jack123",
                    Role = UserRole.Student
                },
                new User
                {
                    Name = "Lisa",
                    Password = "Lisa123",
                    Role = UserRole.Teacher
                },
                new User
                {
                    Name = "Admin",
                    Password = "Admin",
                    Role = UserRole.Admin
                }
            };
        }

        public Task<UserRole> AuthenticateAsync(string name, string password)
        {
            return Task.Run(() =>
            {
                var user = Users.FirstOrDefault(u => u.Name == name && u.Password == password);
                if(user == null)
                {
                    return UserRole.Unkown;
                }
                return user.Role;
            });
        }
    }
}
