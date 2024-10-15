using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class ReviewService : IReviewService, IDisposable {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync() {
            return await _context.Reviews
                .Include(r => r.UserGameVibes)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id) {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task AddReviewAsync(Review review) {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review) {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id) {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null) {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public void Dispose() {
            //_context?.Dispose();
        }
    }
}
