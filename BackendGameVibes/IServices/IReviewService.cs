using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;

namespace BackendGameVibes.IServices {
    public interface IReviewService : IDisposable {
        Task<IEnumerable<object>> GetAllReviewsAsync();
        Task<object?> GetReviewByIdAsync(int id);
        Task<Review?> AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<object[]> GetLandingReviewsAsync();
        Task<ReportedReview?> ReportReviewAsync(string userId, ReportReviewDTO reportedReviewDTO);
        Task<ReportedReview?> FinishReportReviewAsync(int id, bool toRemove);
        Task<ReportedReview[]?> GetFilteredReviews(string searchPhrase);
    }
}
