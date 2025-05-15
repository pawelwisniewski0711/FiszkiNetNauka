namespace FiszkiNetNauka.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Ustawienia ogólne
        public bool DarkMode { get; set; } = false;

        // Ustawienia nauki
        public int DefaultStudySessionLength { get; set; } = 20; // w minutach
        public int DefaultCardsPerSession { get; set; } = 20;

        // Ustawienia powiadomień
        public bool EmailNotificationsEnabled { get; set; } = true;
        public bool DailyReminderEnabled { get; set; } = false;
        public TimeSpan? DailyReminderTime { get; set; } = null;

        // Ustawienia interfejsu
        public string PreferredLanguage { get; set; } = "pl-PL";

        // Daty
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}