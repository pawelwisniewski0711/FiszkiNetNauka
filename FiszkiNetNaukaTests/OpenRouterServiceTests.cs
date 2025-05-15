namespace FiszkiNetNaukaTests
{
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using FiszkiNetNauka.Components.FishCardsGenerator.Services;
    using Moq;
    using Moq.Protected;

    public class OpenRouterServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly OpenRouterService _openRouterService;

        public OpenRouterServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://openrouter.ai/api/v1/")
            };

            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClientFactory
                .Setup(factory => factory.CreateClient("OpenRouter"))
                .Returns(_httpClient);

            _openRouterService = new OpenRouterService(_mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task GetChatResponseAsync_ApiReturnsError_ThrowsException()
        {
            // Arrange
            string userInput = "Generate flashcards";
            string errorMessage = "API error occurred";

            // Setup mock handler to return an error response
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(errorMessage)
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _openRouterService.GetChatResponseAsync(userInput));
            
            Assert.Contains("Błąd API", exception.Message);
            Assert.Contains(errorMessage, exception.Message);
        }

        [Fact]
        public async Task GetChatResponseAsync_EmptyResponse_ReturnsDefaultMessage()
        {
            // Arrange
            string userInput = "Generate flashcards";
            
            var responseContent = new
            {
                choices = new object[] { }  // Empty choices array
            };

            var responseJson = JsonSerializer.Serialize(responseContent);
            
            // Setup mock handler
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            // Act
            string result = await _openRouterService.GetChatResponseAsync(userInput);

            // Assert
            Assert.Equal("Brak odpowiedzi", result);
        }
    }
} 