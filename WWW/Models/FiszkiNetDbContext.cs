namespace FiszkiNetNauka.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NaukaFiszek.Models;

    public class FiszkiNetDbContext : IdentityDbContext<ApplicationUser>
    {
        public FiszkiNetDbContext(DbContextOptions<FiszkiNetDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }
        public DbSet<StudySessionDetail> StudySessionDetails { get; set; }
        public DbSet<GenerationStatistic> GenerationStatistics { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<Flashcard>().ToTable("Flashcards");
            modelBuilder.Entity<StudySession>().ToTable("StudySessions");
            modelBuilder.Entity<StudySessionDetail>().ToTable("StudySessionDetails");
            modelBuilder.Entity<GenerationStatistic>().ToTable("GenerationStatistics");
            modelBuilder.Entity<UserSettings>().ToTable("UserSettings");

            // Configure relationships
            modelBuilder.Entity<Flashcard>()
                .HasOne(f => f.User)
                .WithMany(u => u.Flashcards)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudySession>()
                .HasOne(s => s.User)
                .WithMany(u => u.StudySessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudySessionDetail>()
                .HasOne(d => d.Session)
                .WithMany(s => s.StudySessionDetails)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudySessionDetail>()
                .HasOne(d => d.Flashcard)
                .WithMany(f => f.StudySessionDetails)
                .HasForeignKey(d => d.FlashcardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GenerationStatistic>()
                .HasOne(g => g.User)
                .WithMany(u => u.GenerationStatistics)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSettings>()
                .HasOne(s => s.User)
                .WithOne(u => u.UserSettings)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes
            modelBuilder.Entity<Flashcard>()
                .HasIndex(f => f.UserId)
                .HasDatabaseName("idx_flashcards_user_id");

            modelBuilder.Entity<Flashcard>()
                .HasIndex(f => new { f.UserId, f.NextReviewDate })
                .HasDatabaseName("idx_flashcards_next_review_date");

            modelBuilder.Entity<Flashcard>()
                .HasIndex(f => new { f.UserId, f.CreatedAt })
                .HasDatabaseName("idx_flashcards_created_at");

            modelBuilder.Entity<StudySession>()
                .HasIndex(s => s.UserId)
                .HasDatabaseName("idx_study_sessions_user_id");

            modelBuilder.Entity<StudySession>()
                .HasIndex(s => new { s.UserId, s.StartedAt })
                .HasDatabaseName("idx_study_sessions_started_at");

            modelBuilder.Entity<StudySessionDetail>()
                .HasIndex(d => d.SessionId)
                .HasDatabaseName("idx_study_session_details_session_id");

            modelBuilder.Entity<StudySessionDetail>()
                .HasIndex(d => d.FlashcardId)
                .HasDatabaseName("idx_study_session_details_flashcard_id");

            modelBuilder.Entity<GenerationStatistic>()
                .HasIndex(g => g.UserId)
                .HasDatabaseName("idx_generation_statistics_user_id");

            modelBuilder.Entity<GenerationStatistic>()
                .HasIndex(g => new { g.UserId, g.GeneratedAt })
                .HasDatabaseName("idx_generation_statistics_generated_at");

            modelBuilder.Entity<UserSettings>()
                .HasIndex(s => s.UserId)
                .HasDatabaseName("idx_user_settings_user_id");
        }
    }
}