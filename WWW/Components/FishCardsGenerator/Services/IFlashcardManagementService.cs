using System.Collections.Generic;
using System.Threading.Tasks;
using FiszkiNetNauka.Components.FishCardsGenerator.Models;
using NaukaFiszek.Models;

namespace FiszkiNetNauka.Services
{
    using FiszkiNetNauka.Models;

    public interface IFlashcardManagementService
    {
        /// <summary>
        /// Creates a custom flashcard manually entered by a user
        /// </summary>
        Task<Flashcard> CreateCustomFlashcardAsync(string front, string back, string userId);
        
        /// <summary>
        /// Saves selected flashcards to the database and optionally records generation statistics
        /// </summary>
        /// <param name="flashcards">List of flashcards to save</param>
        /// <param name="userId">ID of the user</param>
        /// <param name="onlyAccepted">Whether to save only accepted flashcards</param>
        /// <param name="sourceTextLength">Length of the source text (for statistics)</param>
        Task<List<Flashcard>> SaveSelectedFlashcardsAsync(
            List<FlashcardViewModel> flashcards, 
            string userId, 
            bool onlyAccepted,
            int? sourceTextLength = null);
    }
} 