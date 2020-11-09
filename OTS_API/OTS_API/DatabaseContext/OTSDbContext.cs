using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OTS_API.Models;

namespace OTS_API.DatabaseContext
{
    public class OTSDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        //public virtual DbSet<Bulletin> Bulletin { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        //public virtual DbSet<CourseGroup> CourseGroup { get; set; }
        //public virtual DbSet<Courseware> Courseware { get; set; }
        //public virtual DbSet<Discussion> Discussion { get; set; }
        //public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<File> Files { get; set; }
        //public virtual DbSet<Homework> Homework { get; set; }
        //public virtual DbSet<Question> Question { get; set; }
        //public virtual DbSet<UserAnswer> UserAnswer { get; set; }
        public virtual DbSet<UserCourse> UserCourse { get; set; }
        //public virtual DbSet<UserDiscussion> UserDiscussion { get; set; }
        //public virtual DbSet<UserExam> UserExam { get; set; }
        //public virtual DbSet<UserGroup> UserGroup { get; set; }
        //public virtual DbSet<UserHomework> UserHomework { get; set; }

        public OTSDbContext(DbContextOptions<OTSDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCourse>().HasKey(uc => new { uc.CourseId, uc.UserId });
        }
    }
}
