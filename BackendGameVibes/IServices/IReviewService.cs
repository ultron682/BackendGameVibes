﻿using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.DTOs.Responses;

namespace BackendGameVibes.IServices;
public interface IReviewService : IDisposable {
    Task<GetAllReviewsResponse?> GetAllReviewsAsync(int pageNumber = 1, int resultSize = 10);
    Task<object?> GetFilteredReviewsAsync(string searchPhrase, int pageNumber = 1, int resultSize = 10);
    Task<object?> GetReviewByIdAsync(int id);
    Task<Review?> AddReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(string userId, int id);
    Task<object[]> GetLandingReviewsAsync();
    Task<ReportedReview?> ReportReviewAsync(string userId, ReportReviewDTO reportedReviewDTO);
    Task<ReportedReview?> FinishReportReviewAsync(int id, bool toRemove);
    Task<object?> UpdateReviewByIdAsync(int reviewId, string userId, ReviewUpdateDTO reviewUpdateDTO);
    Task<object?> GetGameReviewsAsync(int gameId, int pageNumber = 1, int resultSize = 10);
}
