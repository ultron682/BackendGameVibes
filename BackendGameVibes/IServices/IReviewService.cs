using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.DTOs;

namespace BackendGameVibes.IServices {
    public interface IReviewService : IDisposable {
        Task<IEnumerable<object>> GetAllReviewsAsync();
        Task<object?> GetReviewByIdAsync(int id);
        Task<Review?> AddReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(string userId, int id);
        Task<object[]> GetLandingReviewsAsync();
        Task<ReportedReview?> ReportReviewAsync(string userId, ReportReviewDTO reportedReviewDTO);
        Task<ReportedReview?> FinishReportReviewAsync(int id, bool toRemove);
        Task<object[]?> GetFilteredReviewsAsync(string searchPhrase);
        Task<Review?> UpdateReviewByIdAsync(int reviewId, string userId, ReviewUpdateDTO reviewUpdateDTO);
    }
}
