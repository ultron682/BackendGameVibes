using BackendGameVibes.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.IServices {
    public interface IReviewService : IDisposable {
        Task<IEnumerable<object>> GetAllReviewsAsync();
        Task<object?> GetReviewByIdAsync(int id);
        Task<Review?> AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
    }
}
