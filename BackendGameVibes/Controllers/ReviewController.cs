using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using BackendGameVibes.Data;

namespace BackendGameVibes.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context) {
            _context = context;
        }

        // Get all reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews() {
            return await _context.Reviews
                .Include(r => r.UserGameVibes)
                .Include(r => r.Game)
                .ToListAsync();
        }

        // Get a single review by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id) {
            var review = await _context.Reviews
                .Include(r => r.UserGameVibes)
                .Include(r => r.Game)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) {
                return NotFound();
            }

            return review;
        }

        // Create a new review
        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview(Review review) {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        // Update a review
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, Review review) {
            if (id != review.Id) {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_context.Reviews.Any(r => r.Id == id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete a review
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id) {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
