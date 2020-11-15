using Microsoft.EntityFrameworkCore;
using OTS_API.Models;
using OTS_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.DatabaseContext
{
    public class ExamDBContext : DbContext
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<UserExam> UserExams { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySQL(Config.connStr);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserExam>().HasKey(ue => new { ue.UserId, ue.ExamId });
            modelBuilder.Entity<UserAnswer>().HasKey(ua => new { ua.UserId, ua.ExamId, ua.QuestionId });
        }
    }
}
