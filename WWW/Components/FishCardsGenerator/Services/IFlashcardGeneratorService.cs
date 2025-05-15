using System.Collections.Generic;
using System.Threading.Tasks;
using NaukaFiszek.Models;

namespace FiszkiNetNauka.Services
{
    using FiszkiNetNauka.Models;

    public interface IFlashcardGeneratorService
    {
        /// <summary>
        /// Generates flashcards from text content using AI
        /// </summary>
        /// <param name="content">The text content to generate flashcards from</param>
        /// <param name="count">Maximum number of flashcards to generate</param>
        /// <param name="userId">ID of the user generating the flashcards</param>
        Task<List<Flashcard>> GenerateFlashcardsAsync(string content, int count, string userId);
    }
} 