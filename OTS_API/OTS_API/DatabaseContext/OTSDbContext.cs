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
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<Courseware> Coursewares { get; set; }
        public DbSet<CoursewareFile> CoursewareFile { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Homework> Homework { get; set; }
        public DbSet<HomeworkFile> HomeworkFile { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAnswer> UserAnswer { get; set; }
        public DbSet<UserCourse> UserCourse { get; set; }
        public DbSet<UserDiscussion> UserDiscussion { get; set; }
        public DbSet<UserExam> UserExam { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
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
            modelBuilder.Entity<Question>().HasKey(q => new { q.QuestionId, q.ExamId });
            modelBuilder.Entity<UserExam>().HasKey(ue => new { ue.UserId, ue.ExamId });
            modelBuilder.Entity<UserAnswer>().HasKey(ua => new { ua.UserId, ua.ExamId, ua.QuestionId });
            modelBuilder.Entity<CourseGroup>().HasKey(cg => new { cg.GroupId, cg.CourseId });
            modelBuilder.Entity<UserGroup>().HasKey(ug => new { ug.GroupId, ug.UserId, ug.CourseId });
            modelBuilder.Entity<UserDiscussion>().HasKey(ud => new { ud.DiscussionId, ud.Level });
        }
    }
}
