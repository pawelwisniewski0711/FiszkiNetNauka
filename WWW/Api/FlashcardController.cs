using FiszkiNetNauka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FiszkiNetNauka.Api
{
    using FiszkiNetNauka.Models;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlashcardController : ControllerBase
    {
        private readonly FiszkiNetDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FlashcardGeneratorService _flashcardGenerator;

        public FlashcardController(
            FiszkiNetDbContext context, 
            UserManager<ApplicationUser> userManager,
            FlashcardGeneratorService flashcardGenerator)
        {
            _context = context;
            _userManager = userManager;
            _flashcardGenerator = flashcardGenerator;
        }

        // GET: api/Flashcard
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlashcardResponse>>> GetFlashcards()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var flashcards = await _context.Flashcards
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FlashcardResponse
                {
                    Id = f.Id,
                    Front = f.Front,
                    Back = f.Back,
                    CreatedAt = f.CreatedAt,
                    NextReviewDate = f.NextReviewDate,
                    ReviewCount = f.ReviewCount,
                    EaseFactor = f.EaseFactor
                })
                .ToListAsync();

            return Ok(flashcards);
        }

        // GET: api/Flashcard/pending
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<FlashcardResponse>>> GetPendingFlashcards()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTimeOffset.Now;
            
            var flashcards = await _context.Flashcards
                .Where(f => f.UserId == userId && (f.NextReviewDate == null || f.NextReviewDate <= now))
                .OrderBy(f => f.NextReviewDate)
                .Select(f => new FlashcardResponse
                {
                    Id = f.Id,
                    Front = f.Front,
                    Back = f.Back,
                    CreatedAt = f.CreatedAt,
                    NextReviewDate = f.NextReviewDate,
                    ReviewCount = f.ReviewCount,
                    EaseFactor = f.EaseFactor
                })
                .ToListAsync();

            return Ok(flashcards);
        }

        // GET: api/Flashcard/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlashcardResponse>> GetFlashcard(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var flashcard = await _context.Flashcards
                .Where(f => f.Id == id && f.UserId == userId)
                .Select(f => new FlashcardResponse
                {
                    Id = f.Id,
                    Front = f.Front,
                    Back = f.Back,
                    CreatedAt = f.CreatedAt,
                    NextReviewDate = f.NextReviewDate,
                    ReviewCount = f.ReviewCount,
                    EaseFactor = f.EaseFactor
                })
                .FirstOrDefaultAsync();

            if (flashcard == null)
            {
                return NotFound();
            }

            return Ok(flashcard);
        }

        // POST: api/Flashcard
        [HttpPost]
        public async Task<ActionResult<FlashcardResponse>> CreateFlashcard(CreateFlashcardRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var flashcard = new Flashcard
            {
                Front = request.Front,
                Back = request.Back,
                UserId = userId,
                IsGeneratedByAI = request.IsGeneratedByAI,
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };

            _context.Flashcards.Add(flashcard);
            await _context.SaveChangesAsync();

            var response = new FlashcardResponse
            {
                Id = flashcard.Id,
                Front = flashcard.Front,
                Back = flashcard.Back,
                CreatedAt = flashcard.CreatedAt,
                NextReviewDate = flashcard.NextReviewDate,
                ReviewCount = flashcard.ReviewCount,
                EaseFactor = flashcard.EaseFactor
            };

            return CreatedAtAction(nameof(GetFlashcard), new { id = flashcard.Id }, response);
        }

        // PUT: api/Flashcard/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlashcard(Guid id, UpdateFlashcardRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var flashcard = await _context.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (flashcard == null)
            {
                return NotFound();
            }

            flashcard.Front = request.Front;
            flashcard.Back = request.Back;
            flashcard.UpdatedAt = DateTimeOffset.Now;

            _context.Entry(flashcard).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Flashcard/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcard(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var flashcard = await _context.Flashcards
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (flashcard == null)
            {
                return NotFound();
            }

            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Flashcard/generate
        [HttpPost("generate")]
        public async Task<ActionResult<GenerateFlashcardsResponse>> GenerateFlashcards(GenerateFlashcardsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                // Generate flashcards using AI
                var flashcards = await _flashcardGenerator.GenerateFlashcardsAsync(
                    request.Content, 
                    request.Count, 
                    userId);
                
                // Save to database
                _context.Flashcards.AddRange(flashcards);
                await _context.SaveChangesAsync();
                
                // Create response
                var response = new GenerateFlashcardsResponse
                {
                    Flashcards = flashcards.Select(f => new FlashcardResponse
                    {
                        Id = f.Id,
                        Front = f.Front,
                        Back = f.Back,
                        CreatedAt = f.CreatedAt,
                        NextReviewDate = f.NextReviewDate,
                        ReviewCount = f.ReviewCount,
                        EaseFactor = f.EaseFactor
                    }).ToList(),
                    TotalGenerated = flashcards.Count
                };
                
                // Create a record in the GenerationStatistics
                var statistic = new GenerationStatistic
                {
                    UserId = userId,
                    TotalGenerated = flashcards.Count,
                    TotalAccepted = flashcards.Count,
                    SourceTextLength = request.Content.Length,
                    GeneratedAt = DateTimeOffset.Now
                };
                
                _context.GenerationStatistics.Add(statistic);
                await _context.SaveChangesAsync();
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
} 