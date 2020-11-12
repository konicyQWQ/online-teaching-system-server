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
        public DbSet<Bulletin> Bulletins { get; set; }
        public DbSet<Course> Courses { get; set; }
        //public DbSet<CourseGroup> CourseGroup { get; set; }
        public DbSet<Courseware> Coursewares { get; set; }
        public DbSet<CoursewareFile> CoursewareFile { get; set; }
        //public DbSet<Discussion> Discussion { get; set; }
        //public DbSet<Exam> Exam { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Homework> Homework { get; set; }
        public DbSet<HomeworkFile> HomeworkFile { get; set; }
        //public DbSet<Question> Question { get; set; }
        //public DbSet<UserAnswer> UserAnswer { get; set; }
        public DbSet<UserCourse> UserCourse { get; set; }
        //public DbSet<UserDiscussion> UserDiscussion { get; set; }
        //public DbSet<UserExam> UserExam { get; set; }
        //public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<UserHomework> UserHomework { get; set; }
        public DbSet<UserHomeworkFile> UserHomeworkFile { get; set; }

        public OTSDbContext(DbContextOptions<OTSDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCourse>().HasKey(uc => new { uc.UserId, uc.CourseId });
            modelBuilder.Entity<CoursewareFile>().HasKey(cf => new { cf.CoursewareId, cf.FileId });
            modelBuilder.Entity<HomeworkFile>().HasKey(hf => new { hf.HwID, hf.FileID });
            modelBuilder.Entity<UserHomework>().HasKey(uh => new { uh.UserId, uh.HwId });
        }
    }
}
