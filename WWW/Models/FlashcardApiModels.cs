namespace FiszkiNetNauka.Models
{
    // Model for creating a new flashcard
    public class CreateFlashcardRequest
    {
        public string Front { get; set; }
        public string Back { get; set; }
        public bool IsGeneratedByAI { get; set; } = true;
    }

    // Model for updating a flashcard
    public class UpdateFlashcardRequest
    {
        public string Front { get; set; }
        public string Back { get; set; }
    }

    // Model for generating flashcards using AI
    public class GenerateFlashcardsRequest
    {
        public string Content { get; set; }
        public int Count { get; set; } = 5;
    }

    // Model for flashcard response
    public class FlashcardResponse
    {
        public Guid Id { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? NextReviewDate { get; set; }
        public int ReviewCount { get; set; }
        public decimal EaseFactor { get; set; }
    }

    // Model for batch flashcard generation response
    public class GenerateFlashcardsResponse
    {
        public List<FlashcardResponse> Flashcards { get; set; }
        public int TotalGenerated { get; set; }
    }
} 