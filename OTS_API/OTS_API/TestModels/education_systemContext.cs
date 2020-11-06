using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OTS_API.TestModels
{
    public partial class education_systemContext : DbContext
    {
        public education_systemContext()
        {
        }

        public education_systemContext(DbContextOptions<education_systemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bulletin> Bulletin { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseGroup> CourseGroup { get; set; }
        public virtual DbSet<Courseware> Courseware { get; set; }
        public virtual DbSet<Discussion> Discussion { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<Homework> Homework { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAnswer> UserAnswer { get; set; }
        public virtual DbSet<UserCourse> UserCourse { get; set; }
        public virtual DbSet<UserDiscussion> UserDiscussion { get; set; }
        public virtual DbSet<UserExam> UserExam { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserHomework> UserHomework { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=124.70.177.180;port=3306;user=root;password=a123456; database=education_system;sslmode=none;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bulletin>(entity =>
            {
                entity.HasKey(e => new { e.CourseId, e.BulletinId })
                    .HasName("PRIMARY");

                entity.ToTable("bulletin");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BulletinId)
                    .HasColumnName("bulletin_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(1024);

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(32);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Bulletin)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("course_id_in_bulletin");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("course");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1024);

                entity.Property(e => e.Institute)
                    .IsRequired()
                    .HasColumnName("institute")
                    .HasMaxLength(32);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(32);

                entity.Property(e => e.ScoringMethod)
                    .IsRequired()
                    .HasColumnName("scoringMethod")
                    .HasMaxLength(64);

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("date");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasComment("0-not begin, 1-have started, 2-end");

                entity.Property(e => e.Textbook)
                    .HasColumnName("textbook")
                    .HasMaxLength(20);

                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<CourseGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId)
                    .HasName("PRIMARY");

                entity.ToTable("course_group");

                entity.HasIndex(e => e.CourseId)
                    .HasName("group_course_id");

                entity.Property(e => e.GroupId)
                    .HasColumnName("group_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MaxCount)
                    .HasColumnName("max_count")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PreCount)
                    .HasColumnName("pre_count")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseGroup)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("group_course_id");
            });

            modelBuilder.Entity<Courseware>(entity =>
            {
                entity.ToTable("courseware");

                entity.HasIndex(e => e.CourseId)
                    .HasName("course_id_in_courseware");

                entity.HasIndex(e => e.FileId)
                    .HasName("file_id_in_courseware");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(512);

                entity.Property(e => e.FileId)
                    .HasColumnName("file_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Privilege)
                    .HasColumnName("privilege")
                    .HasColumnType("tinyint(4)")
                    .HasComment("0 - can't download 1- can download");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("date");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Courseware)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("course_id_in_courseware");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Courseware)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("file_id_in_courseware");
            });

            modelBuilder.Entity<Discussion>(entity =>
            {
                entity.HasKey(e => new { e.CourseId, e.DiscussionId })
                    .HasName("PRIMARY");

                entity.ToTable("discussion");

                entity.HasIndex(e => e.DiscussionId)
                    .HasName("discussion_id");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DiscussionId)
                    .HasColumnName("discussion_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(64);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Discussion)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("discoussion_course_id");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => new { e.ExamId, e.CourseId })
                    .HasName("PRIMARY");

                entity.ToTable("exam");

                entity.HasIndex(e => e.CourseId)
                    .HasName("exam_course_id");

                entity.HasIndex(e => e.ExamId)
                    .HasName("exam_id");

                entity.Property(e => e.ExamId)
                    .HasColumnName("exam_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MaxMark)
                    .HasColumnName("max_mark")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(32);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Exam)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("exam_course_id");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.ToTable("file");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(32);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(e => new { e.HwId, e.CourseId })
                    .HasName("PRIMARY");

                entity.ToTable("homework");

                entity.HasIndex(e => e.CourseId)
                    .HasName("id");

                entity.HasIndex(e => e.HwId)
                    .HasName("hw_id");

                entity.Property(e => e.HwId)
                    .HasColumnName("hw_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(1024);

                entity.Property(e => e.Percentage)
                    .HasColumnName("percentage")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(10);

                entity.Property(e => e.TotalMark)
                    .HasColumnName("total_mark")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Homework)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("id");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => new { e.QuestionId, e.ExamId })
                    .HasName("PRIMARY");

                entity.ToTable("question");

                entity.HasIndex(e => e.ExamId)
                    .HasName("q_exam_id");

                entity.Property(e => e.QuestionId)
                    .HasColumnName("question_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ExamId)
                    .HasColumnName("exam_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(1024);

                entity.Property(e => e.Mark)
                    .HasColumnName("mark")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OptionA)
                    .HasColumnName("optionA")
                    .HasMaxLength(64);

                entity.Property(e => e.OptionB)
                    .HasColumnName("optionB")
                    .HasMaxLength(64);

                entity.Property(e => e.OptionC)
                    .HasColumnName("optionC")
                    .HasMaxLength(64);

                entity.Property(e => e.OptionD)
                    .HasColumnName("optionD")
                    .HasMaxLength(64);

                entity.Property(e => e.RightAnswer)
                    .IsRequired()
                    .HasColumnName("right_answer")
                    .HasMaxLength(1024);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(4)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.AvatarId)
                    .HasName("id_idx");

                entity.HasIndex(e => e.Role)
                    .HasName("identity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(10);

                entity.Property(e => e.AvatarId)
                    .HasColumnName("avatar_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(32);

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Grade)
                    .HasColumnName("grade")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Introduction)
                    .HasColumnName("introduction")
                    .HasMaxLength(1024);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(11);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasColumnType("tinyint(4)")
                    .HasComment("0-student, 1-teacher, 2-teach assistant,3-admin,4-visitor");

                entity.HasOne(d => d.Avatar)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AvatarId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("avater_id");
            });

            modelBuilder.Entity<UserAnswer>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ExamId, e.QuestionId })
                    .HasName("PRIMARY");

                entity.ToTable("user_answer");

                entity.HasIndex(e => e.ExamId)
                    .HasName("user_answser_exam_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10);

                entity.Property(e => e.ExamId)
                    .HasColumnName("exam_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.QuestionId)
                    .HasColumnName("question_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Answer)
                    .HasColumnName("answer")
                    .HasMaxLength(1024);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAnswer)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_answer_id");
            });

            modelBuilder.Entity<UserCourse>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CourseId })
                    .HasName("PRIMARY");

                entity.ToTable("user_course");

                entity.HasIndex(e => e.CourseId)
                    .HasName("2");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10);

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserRole)
                    .HasColumnName("user_role")
                    .HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.UserCourse)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCourse)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("1");
            });

            modelBuilder.Entity<UserDiscussion>(entity =>
            {
                entity.HasKey(e => new { e.DiscussionId, e.Level })
                    .HasName("PRIMARY");

                entity.ToTable("user_discussion");

                entity.HasIndex(e => e.UserId)
                    .HasName("discussion_user_id");

                entity.Property(e => e.DiscussionId)
                    .HasColumnName("discussion_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasMaxLength(1024);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasMaxLength(10);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDiscussion)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("discussion_user_id");
            });

            modelBuilder.Entity<UserExam>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ExamId })
                    .HasName("PRIMARY");

                entity.ToTable("user_exam");

                entity.HasIndex(e => e.ExamId)
                    .HasName("exam_exam_id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10);

                entity.Property(e => e.ExamId)
                    .HasColumnName("exam_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mark)
                    .HasColumnName("mark")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserExam)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_exam_id");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("user_group");

                entity.HasIndex(e => e.UserId)
                    .HasName("group_user_id");

                entity.Property(e => e.GroupId)
                    .HasColumnName("group_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10);

                entity.Property(e => e.Identity)
                    .HasColumnName("identity")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UserGroup)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("group_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserGroup)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("group_user_id");
            });

            modelBuilder.Entity<UserHomework>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.FileId })
                    .HasName("PRIMARY");

                entity.ToTable("user_homework");

                entity.HasIndex(e => e.FileId)
                    .HasName("file_id_in_user_homework");

                entity.HasIndex(e => e.HwId)
                    .HasName("hw_id_in_user_homework");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(10);

                entity.Property(e => e.FileId)
                    .HasColumnName("file_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Desription)
                    .HasColumnName("desription")
                    .HasMaxLength(256);

                entity.Property(e => e.HwId)
                    .HasColumnName("hw_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mark)
                    .HasColumnName("mark")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.UserHomework)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("file_id_in_user_homework");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserHomework)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_id_in_user_homework");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
