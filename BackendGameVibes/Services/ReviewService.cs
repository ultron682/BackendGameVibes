using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class ReviewService : IReviewService {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetAllReviewsAsync() {
            return await _context.Reviews
                .Include(r => r.UserGameVibes)
                .Select(r => new {
                    r.Id,
                    r.GeneralScore,
                    r.GameplayScore,
                    r.GraphicsScore,
                    r.AudioScore,
                    r.Comment,
                    Username = r.UserGameVibes != null ? r.UserGameVibes.UserName : "NoUsername",
                    GameTitle = r.Game != null ? r.Game.Title : "NoData",
                    r.CreatedAt,
                })
                .ToArrayAsync();
        }

        public async Task<object?> GetReviewByIdAsync(int id) {
            var review = await _context.Reviews
                .Include(r => r.UserGameVibes)
                .Include(r => r.Game)
                .Where(r => r.Id == id)
                .Select(r => new {
                    r.Id,
                    r.GeneralScore,
                    r.GameplayScore,
                    r.GraphicsScore,
                    r.AudioScore,
                    r.Comment,
                    Username = r.UserGameVibes != null ? r.UserGameVibes.UserName : "NoUsername",
                    GameTitle = r.Game != null ? r.Game.Title : "NoData",
                    r.CreatedAt,
                })
                .FirstOrDefaultAsync();


            return review;
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
