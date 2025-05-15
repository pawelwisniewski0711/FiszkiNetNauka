namespace FiszkiNetNauka.Models
{
    using Microsoft.AspNetCore.Identity;
    using NaukaFiszek.Models;

    public class ApplicationUser : IdentityUser
    {
        // Navigation properties
        public virtual ICollection<Flashcard> Flashcards { get; set; }
        public virtual ICollection<StudySession> StudySessions { get; set; }
        public virtual ICollection<GenerationStatistic> GenerationStatistics { get; set; }

        // Relationship to UserSettings
        public virtual UserSettings UserSettings { get; set; }
    }
}