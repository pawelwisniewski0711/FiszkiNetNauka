using System.Threading.Tasks;

namespace FiszkiNetNauka.Components.FishCardsGenerator.Services
{
    public interface IOpenRouterService
    {
        /// <summary>
        /// Sends a message to the AI chat model and returns the response
        /// </summary>
        /// <param name="userInput">The message to send to the AI model</param>
        Task<string> GetChatResponseAsync(string userInput);
    }
} 