namespace FiszkiNetNauka.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using NaukaFiszek.Models;

    public class Flashcard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Front { get; set; }

        [Required]
        [MaxLength(500)]
        public string Back { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        [Required]
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

        [Required]
        public bool IsGeneratedByAI { get; set; } = false;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal EaseFactor { get; set; } = 2.5m;

        [Required]
        public int Interval { get; set; } = 0;

        public DateTimeOffset? NextReviewDate { get; set; }

        public DateTimeOffset? LastReviewDate { get; set; }

        [Required]
        public int ReviewCount { get; set; } = 0;

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<StudySessionDetail> StudySessionDetails { get; set; }
    }
} 