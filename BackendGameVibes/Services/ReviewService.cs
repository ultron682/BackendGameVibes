﻿using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models.DTOs;

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

        public async Task<object?> GetAllReviewsAsync(int pageNumber = 1, int resultSize = 10) {
            var query = await _context.Reviews
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * resultSize)
                .Take(resultSize)
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

            int totalResults = await _context.Reviews.CountAsync();

            return new {
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

        public async Task<bool> DeleteReviewAsync(string userId, int id) {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null) {
                if (review.UserGameVibesId == userId) {
                    _context.Reviews.Remove(review);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<object[]> GetLandingReviewsAsync() {
            return await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.UserGameVibes)
                .AsSplitQuery()
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

        public async Task<Review?> UpdateReviewByIdAsync(int reviewId, string userId, ReviewUpdateDTO reviewUpdateDTO) {
            var review = await _context.Reviews
               .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserGameVibesId == userId);

            if (review != null) {
                review.GeneralScore = reviewUpdateDTO.GeneralScore ?? review.GeneralScore;
                review.GraphicsScore = reviewUpdateDTO.GraphicsScore ?? review.GraphicsScore;
                review.AudioScore = reviewUpdateDTO.AudioScore ?? review.AudioScore;
                review.GameplayScore = reviewUpdateDTO.GameplayScore ?? review.GameplayScore;
                review.Comment = reviewUpdateDTO.Comment ?? review.Comment;

                review.UpdatedAt = DateTime.Now;
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
            }

            return review;
        }

        public void Dispose() {
            _context?.Dispose();
        }
    }
}
