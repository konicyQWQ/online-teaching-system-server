﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    public class Token
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }

        public Token(string userID, UserRole role, int validHours)
        {
            UserID = userID;
            Role = role;
            CreateTime = DateTime.Now;
            ExpireTime = CreateTime.AddHours(validHours);
        }

        public bool IsValid()
        {
            return DateTime.Now < ExpireTime;
        }
    }

    public class SToken
    {
        public string ID { get; set; }
        public string ValidationCode { get; set; }
        public string UserID { get; set; }
        public string Email { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public bool IsVerified { get; set; }

        public SToken(string userID, string email)
        {
            this.UserID = userID;
            this.Email = email;
            this.CreateTime = DateTime.Now;
            this.ExpireTime = CreateTime.AddMinutes(5);
            this.IsVerified = false;
        }

        public bool IsValid()
        {
            return DateTime.Now < ExpireTime;
        }
    }
}
