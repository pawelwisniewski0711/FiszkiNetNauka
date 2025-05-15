using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiszkiNetNauka.Components.FishCardsGenerator.Models;
using NaukaFiszek.Models;

namespace FiszkiNetNauka.Services
{
    using FiszkiNetNauka.Models;

    public class FlashcardManagementService : IFlashcardManagementService
    {
        private readonly FiszkiNetDbContext _dbContext;
        private readonly IFlashcardService _flashcardService;

        public FlashcardManagementService(FiszkiNetDbContext dbContext, IFlashcardService flashcardService)
        {
            _dbContext = dbContext;
            _flashcardService = flashcardService;
        }

        public async Task<Flashcard> CreateCustomFlashcardAsync(string front, string back, string userId)
        {
            return await _flashcardService.CreateFlashcardAsync(front, back, userId, false);
        }

        public async Task<List<Flashcard>> SaveSelectedFlashcardsAsync(
            List<FlashcardViewModel> flashcards, 
            string userId, 
            bool onlyAccepted,
            int? sourceTextLength = null)
        {
            // Filter flashcards based on acceptance status if required
            var flashcardsToSave = onlyAccepted
                ? flashcards.Where(f => f.IsAccepted).ToList()
                : flashcards;

            if (!flashcardsToSave.Any())
            {
                return new List<Flashcard>();
            }

            // Convert to domain model
            var flashcardEntities = flashcardsToSave.Select(f => new Flashcard
            {
                Front = f.Front,
                Back = f.Back,
                UserId = userId,
                IsGeneratedByAI = !f.IsManuallyCreated,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            }).ToList();

            // Save to database
            await _flashcardService.CreateMultipleFlashcardsAsync(flashcardEntities);

            // Save generation statistics if there are AI-generated flashcards
            if (flashcardsToSave.Any(f => !f.IsManuallyCreated))
            {
                await SaveGenerationStatisticsAsync(
                    userId,
                    flashcards.Count(f => !f.IsManuallyCreated),
                    flashcardsToSave.Count(f => !f.IsManuallyCreated),
                    sourceTextLength ?? 0);
            }

            return flashcardEntities;
        }

        private async Task SaveGenerationStatisticsAsync(string userId, int totalGenerated, int totalAccepted, int sourceTextLength)
        {
            var statistic = new GenerationStatistic
            {
                UserId = userId,
                TotalGenerated = totalGenerated,
                TotalAccepted = totalAccepted,
                SourceTextLength = sourceTextLength,
                GeneratedAt = DateTimeOffset.Now
            };

            _dbContext.GenerationStatistics.Add(statistic);
            await _dbContext.SaveChangesAsync();
        }
    }
} 