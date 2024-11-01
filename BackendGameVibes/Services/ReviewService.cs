﻿using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class ReviewService : IReviewService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IForumExperienceService _forumExperienceService;

        public ReviewService(ApplicationDbContext context, IMapper mapper, IForumExperienceService forumExperienceService) {
            _context = context;
            _mapper = mapper;
            _forumExperienceService = forumExperienceService;
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

        public async Task<Review?> AddReviewAsync(Review review) {
            Game? foundGame = null;
            if (review.GameId != null && review.GameId != 0)
                foundGame = await _context.Games.Where(g => g.Id == review.GameId).FirstOrDefaultAsync();

            if (foundGame != null) {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                await _forumExperienceService.AddReviewPoints(review.UserGameVibesId!);

                return review;
            }
            else {
                return null;
            }

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

        public async Task<object[]> GetLandingReviewsAsync() {
            return await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.UserGameVibes)
                .Select(r => new {
                    r.UserGameVibesId,
                    r.GameId,
                    r.Game!.Title,
                    r.UserGameVibes!.UserName,
                    r.Comment
                })
                .OrderBy(r => EF.Functions.Random())
                .Take(5)
                .ToArrayAsync();

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
                    _context.Reviews.Remove(review);
                }
            }

            await _context.SaveChangesAsync();

            return reportedReview;
        }

        public void Dispose() {
            _context?.Dispose();
        }
    }
}
