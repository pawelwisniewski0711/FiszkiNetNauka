namespace FiszkiNetNauka.Components.FishCardsGenerator.Models
{
    public class FlashcardViewModel
    {
        public string Front { get; set; }
        public string Back { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsEditing { get; set; }
        public bool IsManuallyCreated { get; set; } = false;
    }
} 