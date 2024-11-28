using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Extensions;
using BackendGameVibes.Models.DTOs.Responses;

namespace BackendGameVibes.Services;


public class ReviewService : IReviewService {
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IForumExperienceService _forumExperienceService;

    public ReviewService(ApplicationDbContext context, IMapper mapper, IForumExperienceService forumExperienceService) {
        _context = context;
        _mapper = mapper;
        _forumExperienceService = forumExperienceService;
    }

    public async Task<GetAllReviewsResponse?> GetAllReviewsAsync(int pageNumber = 1, int resultSize = 10) {
        var query = await _context.Reviews
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * resultSize)
            .Take(resultSize)
            .Include(r => r.UserGameVibes)
            .SelectReviewColumns()
            .ToArrayAsync();

        int totalResults = await _context.Reviews.CountAsync();

        return new GetAllReviewsResponse() {
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = query
        };
    }

    public async Task<object?> GetFilteredReviewsAsync(string searchPhrase, int pageNumber = 1, int resultSize = 10) {
        searchPhrase = searchPhrase.ToLower();

        var query = await _context.Reviews
            .Where(r => r.Comment!.ToLower().Contains(searchPhrase))
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * resultSize)
            .Take(resultSize)
            .SelectReviewColumns()
            .ToArrayAsync();

        int totalResults = await _context.Reviews.Where(r => r.Comment!.ToLower().Contains(searchPhrase)).CountAsync();

        return new {
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = query
        };
    }

    public async Task<object?> GetReviewByIdAsync(int id) {
        var review = await _context.Reviews
            .Include(r => r.UserGameVibes)
            .Include(r => r.Game)
            .Where(r => r.Id == id)
            .SelectReviewColumns()
            .FirstOrDefaultAsync();


        return review;
    }

    public async Task<object[]> GetLandingReviewsAsync() {
        return await _context.Reviews
            .Include(r => r.Game)
            .Include(r => r.UserGameVibes)
            .SelectReviewColumns()
            .OrderBy(r => EF.Functions.Random())
            .Take(5)
            .ToArrayAsync();

    }

    public async Task<object?> GetGameReviewsAsync(int gameId, int pageNumber = 1, int resultSize = 10) {
        var query = await _context.Reviews
            .Where(g => g.GameId == gameId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * resultSize)
            .Take(resultSize)
            .SelectReviewColumns()
            .ToArrayAsync();

        int totalResults = await _context.Reviews!.Where(r => r.GameId == gameId).CountAsync();

        return new {
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = query
        };
    }

    public async Task<Review?> AddReviewAsync(Review review) {
        Game? foundGame = null;
        if (review.GameId != null && review.GameId != 0)
            foundGame = await _context.Games.Where(g => g.Id == review.GameId).FirstOrDefaultAsync();

        if (foundGame != null) {
            review.AverageRating = (review.GeneralScore + review.GraphicsScore + review.AudioScore + review.GameplayScore) / 4;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await _forumExperienceService.AddReviewPoints(review.UserGameVibesId!);

            await CalculateAndUpdateRatingForGame(review.GameId);

            return review;
        }
        else {
            return null;
        }
    }

    public async Task<bool> DeleteReviewAsync(string userId, int id) {
        var review = await _context.Reviews.FindAsync(id);

        if (review != null) {
            int? reviewGameId = review.GameId;

            if (review.UserGameVibesId == userId) {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                await CalculateAndUpdateRatingForGame(reviewGameId);
                return true;
            }
        }

        return false;
    }

    public async Task<ReportedReview?> ReportReviewAsync(string userId, ReportReviewDTO reportedReviewDTO) {
        var review = await _context.Reviews.FindAsync(reportedReviewDTO.ReviewId);
        if (review == null) {
            return null;
        }

        var reportedReview = _mapper.Map<ReportedReview>(reportedReviewDTO);
        reportedReview.ReporterUserId = userId;

        _context.ReportedReviews.Add(reportedReview);
        await _context.SaveChangesAsync();

        return reportedReview;
    }

    public async Task<ReportedReview?> FinishReportReviewAsync(int id, bool toRemoveReview) {
        var reportedReview = await _context.ReportedReviews.FindAsync(id);
        if (reportedReview == null) {
            return null;
        }

        reportedReview.IsFinished = true;
        _context.ReportedReviews.Update(reportedReview);

        if (toRemoveReview) {
            var review = await _context.Reviews.FindAsync(reportedReview.ReviewId);
            if (review != null) {
                _context.Reviews.Remove(review); //todo use DeleteReviewAsync() from up
            }
        }

        await _context.SaveChangesAsync();

        return reportedReview;
    }

    public async Task<object?> UpdateReviewByIdAsync(int reviewId, string userId, ReviewUpdateDTO reviewUpdateDTO) {
        var review = await _context.Reviews
           .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserGameVibesId == userId);

        if (review != null) {
            review.GeneralScore = reviewUpdateDTO.GeneralScore ?? review.GeneralScore;
            review.GraphicsScore = reviewUpdateDTO.GraphicsScore ?? review.GraphicsScore;
            review.AudioScore = reviewUpdateDTO.AudioScore ?? review.AudioScore;
            review.GameplayScore = reviewUpdateDTO.GameplayScore ?? review.GameplayScore;
            review.Comment = reviewUpdateDTO.Comment ?? review.Comment;

            review.UpdatedAt = DateTime.Now;

            review.AverageRating = (review.GeneralScore + review.GraphicsScore + review.AudioScore + review.GameplayScore) / 4;

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            await CalculateAndUpdateRatingForGame(review.GameId);
        }

        return await GetReviewByIdAsync(reviewId);
    }

    private async Task CalculateAndUpdateRatingForGame(int? gameId) {
        var game = await _context.Games
            .Include(g => g.Reviews)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game != null) {
            var averageRating = Math.Round(game.Reviews!.Select(c => c.AverageRating).Average(), 1);
            game.LastCalculatedRatingFromReviews = averageRating;
            _context.Games.Update(game);

            await _context.SaveChangesAsync();
        }
    }

    public void Dispose() {
        _context?.Dispose();
    }
}
