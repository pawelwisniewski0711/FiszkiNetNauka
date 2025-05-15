using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FiszkiNetNauka.Models;

namespace FiszkiNetNauka.Components.FishCardsGenerator.Services
{
    public class OpenRouterService : IOpenRouterService
    {
        private readonly HttpClient _httpClient;

        public OpenRouterService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("OpenRouter");
        }

        public async Task<string> GetChatResponseAsync(string userInput)
        {
            var request = new ChatRequest
            {
                Model = "openai/gpt-3.5-turbo", // lub np. "mistral/mistral-7b-instruct"
                messages = new List<Message>
                {
                    new Message { role = "user", content = userInput }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("chat/completions", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result?.choices?.FirstOrDefault()?.Message?.content ?? "Brak odpowiedzi";
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd API: {error}");
        }
    }
}
