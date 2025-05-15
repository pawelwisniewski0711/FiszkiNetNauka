namespace FiszkiNetNaukaTests
{
    using FiszkiNetNauka.Components.FishCardsGenerator.Services;
    using FiszkiNetNauka.Services;
    using Moq;

    public class FlashcardGeneratorServiceTests
    {
        private readonly Mock<IOpenRouterService> _mockOpenRouterService;
        private readonly FlashcardGeneratorService _flashcardGeneratorService;
        private readonly string _userId = "test-user-id";

        public FlashcardGeneratorServiceTests()
        {
            _mockOpenRouterService = new Mock<IOpenRouterService>();
            _flashcardGeneratorService = new FlashcardGeneratorService(_mockOpenRouterService.Object);
        }

        [Fact]
        public async Task GenerateFlashcardsAsync_Success_ReturnsFlashcards()
        {
            // Arrange
            string content = "Test content for flashcard generation";
            int count = 3;
            
            // Mock AI response
            string aiResponse = @"[
                {
                    ""front"": ""Test pojęcie 1"",
                    ""back"": ""Test definicja 1""
                },
                {
                    ""front"": ""Test pojęcie 2"",
                    ""back"": ""Test definicja 2""
                },
                {
                    ""front"": ""Test pojęcie 3"",
                    ""back"": ""Test definicja 3""
                }
            ]";
            
            _mockOpenRouterService
                .Setup(s => s.GetChatResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _flashcardGeneratorService.GenerateFlashcardsAsync(content, count, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.Count);
            Assert.All(result, flashcard => Assert.Equal(_userId, flashcard.UserId));
            Assert.All(result, flashcard => Assert.True(flashcard.IsGeneratedByAI));
            
            // Verify expected content
            Assert.Contains(result, f => f.Front == "Test pojęcie 1" && f.Back == "Test definicja 1");
            Assert.Contains(result, f => f.Front == "Test pojęcie 2" && f.Back == "Test definicja 2");
            Assert.Contains(result, f => f.Front == "Test pojęcie 3" && f.Back == "Test definicja 3");
            
            // Verify service was called with proper prompt
            _mockOpenRouterService.Verify(
                s => s.GetChatResponseAsync(It.Is<string>(prompt => 
                    prompt.Contains($"Wygeneruj {count} unikalnych fiszek") && 
                    prompt.Contains(content))), 
                Times.Once);
        }

        [Fact]
        public async Task GenerateFlashcardsAsync_InvalidJsonResponse_ThrowsException()
        {
            // Arrange
            string content = "Test content";
            int count = 3;
            
            // Invalid JSON response from AI
            string aiResponse = "This is not a valid JSON response";
            
            _mockOpenRouterService
                .Setup(s => s.GetChatResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(aiResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _flashcardGeneratorService.GenerateFlashcardsAsync(content, count, _userId));
            
            Assert.Contains("Błąd generowania fiszek", exception.Message);
        }

        [Fact]
        public async Task GenerateFlashcardsAsync_EmptyContent_StillMakesRequest()
        {
            // Arrange
            string content = "";
            int count = 2;
            
            string aiResponse = @"[
                {
                    ""front"": ""Empty pojęcie"",
                    ""back"": ""Empty definicja""
                },
                {
                    ""front"": ""Empty pojęcie 2"",
                    ""back"": ""Empty definicja 2""
                }
            ]";
            
            _mockOpenRouterService
                .Setup(s => s.GetChatResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _flashcardGeneratorService.GenerateFlashcardsAsync(content, count, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockOpenRouterService.Verify(s => s.GetChatResponseAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GenerateFlashcardsAsync_ServiceThrowsException_PropagatesException()
        {
            // Arrange
            string content = "Test content";
            int count = 3;
            
            _mockOpenRouterService
                .Setup(s => s.GetChatResponseAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("API service failure"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _flashcardGeneratorService.GenerateFlashcardsAsync(content, count, _userId));
            
            Assert.Contains("Błąd generowania fiszek", exception.Message);
            Assert.Contains("API service failure", exception.InnerException.Message);
        }
    }
} 