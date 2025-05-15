using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaukaFiszek.Models;

namespace FiszkiNetNauka.Services
{
    using FiszkiNetNauka.Models;

    public interface IFlashcardService
    {
        /// <summary>
        /// Gets all flashcards for a user
        /// </summary>
        Task<List<Flashcard>> GetUserFlashcardsAsync(string userId);
        
        /// <summary>
        /// Gets a specific flashcard by its ID
        /// </summary>
        Task<Flashcard> GetFlashcardByIdAsync(Guid id, string userId);
        
        /// <summary>
        /// Creates a new flashcard
        /// </summary>
        Task<Flashcard> CreateFlashcardAsync(string front, string back, string userId, bool isGeneratedByAI = false);
        
        /// <summary>
        /// Updates an existing flashcard
        /// </summary>
        Task<bool> UpdateFlashcardAsync(Guid id, string front, string back, string userId);
        
        /// <summary>
        /// Deletes a flashcard
        /// </summary>
        Task<bool> DeleteFlashcardAsync(Guid id, string userId);
        
        /// <summary>
        /// Creates multiple flashcards at once
        /// </summary>
        Task<List<Flashcard>> CreateMultipleFlashcardsAsync(List<Flashcard> flashcards);
    }
} 