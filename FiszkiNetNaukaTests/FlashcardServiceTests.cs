namespace FiszkiNetNaukaTests
{
    using FiszkiNetNauka.Models;
    using FiszkiNetNauka.Services;
    using Microsoft.EntityFrameworkCore;
    using NaukaFiszek.Models;

    public class FlashcardServiceTests
    {
        private readonly string _userId = "test-user-id";
        private readonly List<Flashcard> _testFlashcards;

        public FlashcardServiceTests()
        {
            // Sample flashcards for testing
            _testFlashcards = new List<Flashcard>
            {
                new Flashcard 
                { 
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Front = "Test 1",
                    Back = "Definition 1",
                    UserId = _userId,
                    CreatedAt = DateTimeOffset.Now.AddDays(-5)
                },
                new Flashcard 
                { 
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Front = "Test 2",
                    Back = "Definition 2",
                    UserId = _userId,
                    CreatedAt = DateTimeOffset.Now.AddDays(-3)
                },
                new Flashcard 
                { 
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Front = "Other User Card",
                    Back = "Other Definition",
                    UserId = "other-user-id",
                    CreatedAt = DateTimeOffset.Now.AddDays(-1)
                }
            };
        }
        
        private FiszkiNetDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<FiszkiNetDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
                
            var context = new FiszkiNetDbContext(options);
            
            // Add test data
            context.Flashcards.AddRange(_testFlashcards);
            context.SaveChanges();
            
            return context;
        }

        [Fact]
        public async Task GetUserFlashcardsAsync_ReturnsOnlyUserFlashcards()
        {
            // Arrange
            using var context = CreateDbContext();
            var flashcardService = new FlashcardService(context);

            // Act
            var result = await flashcardService.GetUserFlashcardsAsync(_userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, f => Assert.Equal(_userId, f.UserId));
            
            // Check if they're ordered by CreatedAt DESC
            var resultList = result.ToList();
            Assert.Equal("Test 2", resultList[0].Front); // More recent should be first
            Assert.Equal("Test 1", resultList[1].Front);
        }

        [Fact]
        public async Task GetFlashcardByIdAsync_UserOwnsFlashcard_ReturnsFlashcard()
        {
            // Arrange
            using var context = CreateDbContext();
            var flashcardService = new FlashcardService(context);
            var flashcardId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await flashcardService.GetFlashcardByIdAsync(flashcardId, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(flashcardId, result.Id);
            Assert.Equal("Test 1", result.Front);
            Assert.Equal("Definition 1", result.Back);
        }

        [Fact]
        public async Task GetFlashcardByIdAsync_UserDoesNotOwnFlashcard_ReturnsNull()
        {
            // Arrange
            using var context = CreateDbContext();
            var flashcardService = new FlashcardService(context);
            
            // Card exists but belongs to another user
            var flashcardId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            // Act
            var result = await flashcardService.GetFlashcardByIdAsync(flashcardId, _userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateFlashcardAsync_SavesFlashcardToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var flashcardService = new FlashcardService(context);
            
            string front = "New Flashcard";
            string back = "New Definition";
            bool isGeneratedByAI = true;

            // Act
            var result = await flashcardService.CreateFlashcardAsync(front, back, _userId, isGeneratedByAI);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(front, result.Front);
            Assert.Equal(back, result.Back);
            Assert.Equal(_userId, result.UserId);
            Assert.Equal(isGeneratedByAI, result.IsGeneratedByAI);
            
            // Verify it was saved to the database
            var savedFlashcard = await context.Flashcards.FindAsync(result.Id);
            Assert.NotNull(savedFlashcard);
            Assert.Equal(front, savedFlashcard.Front);
        }

        [Fact]
        public async Task CreateMultipleFlashcardsAsync_SavesAllFlashcardsToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var flashcardService = new FlashcardService(context);
            
            var flashcardsToCreate = new List<Flashcard>
            {
                new Flashcard { Front = "Batch 1", Back = "Def 1", UserId = _userId },
                new Flashcard { Front = "Batch 2", Back = "Def 2", UserId = _userId }
            };

            // Get initial count
            int initialCount = context.Flashcards.Count();

            // Act
            var result = await flashcardService.CreateMultipleFlashcardsAsync(flashcardsToCreate);

            // Assert
            Assert.Equal(flashcardsToCreate.Count, result.Count);
            
            // Verify they were saved
            Assert.Equal(initialCount + flashcardsToCreate.Count, context.Flashcards.Count());
            Assert.Contains(context.Flashcards, f => f.Front == "Batch 1" && f.Back == "Def 1");
            Assert.Contains(context.Flashcards, f => f.Front == "Batch 2" && f.Back == "Def 2");
        }
    }
} 