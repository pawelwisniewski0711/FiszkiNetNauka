namespace NaukaFiszek.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using FiszkiNetNauka.Models;

    public class StudySession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? EndedAt { get; set; }

        [Required]
        public int FlashcardsReviewed { get; set; } = 0;

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<StudySessionDetail> StudySessionDetails { get; set; }
    }
} 