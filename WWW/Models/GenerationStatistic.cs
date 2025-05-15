namespace FiszkiNetNauka.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class GenerationStatistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.Now;

        [Required]
        public int TotalGenerated { get; set; } = 0;

        [Required]
        public int TotalAccepted { get; set; } = 0;

        [Required]
        public int SourceTextLength { get; set; } = 0;

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
    }
} 