using FiszkiNetNauka.Components.FishCardsGenerator.Services;
using NaukaFiszek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FiszkiNetNauka.Services
{
    using FiszkiNetNauka.Models;

    public class FlashcardGeneratorService : IFlashcardGeneratorService
    {
        private readonly IOpenRouterService _openRouterService;

        public FlashcardGeneratorService(IOpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;
        }

        public async Task<List<Flashcard>> GenerateFlashcardsAsync(string content, int count, string userId)
        {
            // Prepare prompt for the AI model
            string prompt = $@"Wygeneruj {count} unikalnych fiszek w języku polskim na podstawie następującego tekstu:
{content}

Każda fiszka powinna zawierać pojęcie na przodzie i definicję na odwrocie.
Zwróć odpowiedź jako tablicę JSON w następującym formacie:
[
  {{
    ""front"": ""pojęcie"",
    ""back"": ""definicja""
  }},
  ...
]";

            try
            {
                // Get response from the AI model
                string aiResponse = await _openRouterService.GetChatResponseAsync(prompt);

                // Extract JSON array from the response
                int startIndex = aiResponse.IndexOf('[');
                int endIndex = aiResponse.LastIndexOf(']') + 1;

                if (startIndex >= 0 && endIndex > 0 && endIndex > startIndex)
                {
                    string jsonArray = aiResponse.Substring(startIndex, endIndex - startIndex);
                    
                    // Deserialize the JSON array
                    var flashcardDtos = JsonSerializer.Deserialize<List<FlashcardDto>>(jsonArray, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Convert to Flashcard entities
                    var flashcards = flashcardDtos.Select(dto => new Flashcard
                    {
                        Front = dto.Front,
                        Back = dto.Back,
                        UserId = userId,
                        IsGeneratedByAI = true,
                        CreatedAt = DateTimeOffset.Now,
                        UpdatedAt = DateTimeOffset.Now
                    }).ToList();

                    return flashcards;
                }
                
                throw new Exception("Nie udało się przetworzyć odpowiedzi z modelu AI.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd generowania fiszek: {ex.Message}", ex);
            }
        }

        // Helper DTO class for deserializing the AI response
        private class FlashcardDto
        {
            public string Front { get; set; }
            public string Back { get; set; }
        }
    }
} 