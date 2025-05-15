namespace FiszkiNetNauka.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using NaukaFiszek.Models;

    public class StudySessionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public Guid FlashcardId { get; set; }

        [Required]
        public int PerformanceRating { get; set; }

        [Required]
        public DateTimeOffset ReviewedAt { get; set; } = DateTimeOffset.Now;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PreviousEaseFactor { get; set; }

        [Required]
        public int PreviousInterval { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal NewEaseFactor { get; set; }

        [Required]
        public int NewInterval { get; set; }

        // Navigation properties
        public virtual StudySession Session { get; set; }
        public virtual Flashcard Flashcard { get; set; }
    }
} 