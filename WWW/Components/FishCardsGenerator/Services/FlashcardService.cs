using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NaukaFiszek.Models;

namespace FiszkiNetNauka.Services
{
    using FiszkiNetNauka.Models;

    public class FlashcardService : IFlashcardService
    {
        private readonly FiszkiNetDbContext _dbContext;

        public FlashcardService(FiszkiNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Flashcard>> GetUserFlashcardsAsync(string userId)
        {
            return await _dbContext.Flashcards
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Flashcard> GetFlashcardByIdAsync(Guid id, string userId)
        {
            return await _dbContext.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        public async Task<Flashcard> CreateFlashcardAsync(string front, string back, string userId, bool isGeneratedByAI = false)
        {
            var flashcard = new Flashcard
            {
                Front = front,
                Back = back,
                UserId = userId,
                IsGeneratedByAI = isGeneratedByAI,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };

            _dbContext.Flashcards.Add(flashcard);
            await _dbContext.SaveChangesAsync();

            return flashcard;
        }

        public async Task<bool> UpdateFlashcardAsync(Guid id, string front, string back, string userId)
        {
            var flashcard = await GetFlashcardByIdAsync(id, userId);
            
            if (flashcard == null)
            {
                return false;
            }

            flashcard.Front = front;
            flashcard.Back = back;
            flashcard.UpdatedAt = DateTimeOffset.Now;

            _dbContext.Entry(flashcard).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteFlashcardAsync(Guid id, string userId)
        {
            var flashcard = await GetFlashcardByIdAsync(id, userId);
            
            if (flashcard == null)
            {
                return false;
            }

            _dbContext.Flashcards.Remove(flashcard);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Flashcard>> CreateMultipleFlashcardsAsync(List<Flashcard> flashcards)
        {
            _dbContext.Flashcards.AddRange(flashcards);
            await _dbContext.SaveChangesAsync();
            return flashcards;
        }
    }
} 