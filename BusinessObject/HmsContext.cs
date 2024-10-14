using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BusinessObjectLibrary
{
    public partial class HmsContext : DbContext
    {
        public HmsContext()
        {
        }

        public HmsContext(DbContextOptions<HmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cv> Cvs { get; set; }
        public virtual DbSet<MentorDetail> MentorDetails { get; set; }
        public virtual DbSet<MentorSkill> MentorSkills { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<RequestSkill> RequestSkills { get; set; }
        public virtual DbSet<Seminar> Seminars { get; set; }
        public virtual DbSet<SeminarParticipant> SeminarParticipants { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<TeachingThread> TeachingThreads { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Cv>(entity =>
            {
                entity.ToTable("CVs");

                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.MentorId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Cvs)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CVs_Users");
            });

            modelBuilder.Entity<MentorDetail>(entity =>
            {
                entity.HasKey(e => e.MentorId);

                entity.Property(e => e.MentorId).HasMaxLength(40);

                entity.Property(e => e.Salary).HasColumnType("money");

                entity.HasOne(d => d.Mentor)
                    .WithOne(p => p.MentorDetail)
                    .HasForeignKey<MentorDetail>(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorDetails_Users");
            });

            modelBuilder.Entity<MentorSkill>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.MentorId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.SkillId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.MentorSkills)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorSkills_Users");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.MentorSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MentorSkills_Skills");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.Comments)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MenteeId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.MentorId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.RequestId).HasMaxLength(40);

                entity.Property(e => e.SeminarId).HasMaxLength(40);

                entity.HasOne(d => d.Mentee)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.MenteeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ratings_Users_Mentee");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ratings_Users_Mentor");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK_Ratings_Requests");

                entity.HasOne(d => d.Seminar)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.SeminarId)
                    .HasConstraintName("FK_Ratings_Seminars");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.Content)
                    .IsRequired();
                    //.HasMaxLength(300);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MenteeId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.MentorId).HasMaxLength(40);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Mentee)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.MenteeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Requests_Users_Mentee");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.MentorId)
                    .HasConstraintName("FK_Requests_Users_Mentor");
            });

            modelBuilder.Entity<RequestSkill>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.SkillId });

                entity.Property(e => e.RequestId).HasMaxLength(40);

                entity.Property(e => e.SkillId).HasMaxLength(40);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestSkills)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Requests_RequestSkills");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.RequestSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Skills_RequestSkills");
            });

            modelBuilder.Entity<Seminar>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.MeetingUrl)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.MentorId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.RegistrationDeadline).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Seminars)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Seminars_Users_Mentor");
            });

            modelBuilder.Entity<SeminarParticipant>(entity =>
            {
                entity.HasKey(e => new { e.SeminarId, e.UserId })
                    .HasName("PK_SeminarParticipants_1");

                entity.Property(e => e.SeminarId).HasMaxLength(40);

                entity.Property(e => e.UserId).HasMaxLength(40);

                entity.HasOne(d => d.Seminar)
                    .WithMany(p => p.SeminarParticipants)
                    .HasForeignKey(d => d.SeminarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SeminarParticipants_Seminars");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SeminarParticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SeminarParticipants_Users_User");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TeachingThread>(entity =>
            {
                entity.ToTable("TeachingThread");

                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RequestId)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.TeachingThreads)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeachingThread_Requests");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(40);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Phone).HasMaxLength(11);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
