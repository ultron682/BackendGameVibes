using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BackendGameVibes.Tests.Services {
    public class ReviewServiceTests {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IForumExperienceService> _forumExperienceServiceMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests() {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _mapperMock = new Mock<IMapper>();
            _forumExperienceServiceMock = new Mock<IForumExperienceService>();


            _reviewService = new ReviewService(_context, _mapperMock.Object, _forumExperienceServiceMock.Object);
        }

        [Fact]
        public async Task GetAllReviewsAsync_ReturnsCorrectData() {
            // Arrange
            var reviews = new List<Review>
            {
                new Review {
                    Id = 1,
                    Comment = "Great game!",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    GeneralScore = 8,
                    GraphicsScore = 9,
                    AudioScore = 7,
                    GameplayScore = 8,
                }
            }.AsQueryable();


            _context.Reviews.AddRange(reviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reviewService.GetAllReviewsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Length > 0);
        }

        [Fact]
        public async Task AddReviewAsync_AddsReview_WhenGameExists() {
            // Arrange
            var review = new Review {
                Id = 2,
                Comment = "Great game!",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                GeneralScore = 8,
                GraphicsScore = 9,
                AudioScore = 7,
                GameplayScore = 8,
                GameId = 2
            };

            var game = new Game { Id = 2, Title = "Test Game" };

            _context.Games.Add(game);

            await _context.SaveChangesAsync();

            // Act
            var result = await _reviewService.AddReviewAsync(review);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(review.GameId, result.GameId);
        }

        [Fact]
        public async Task DeleteReviewAsync_DeletesReview_WhenReviewExists() {
            // Arrange
            var review = new Review {
                Id = 2,
                Comment = "Great game!",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                GeneralScore = 8,
                GraphicsScore = 9,
                AudioScore = 7,
                GameplayScore = 8,
                GameId = 2
            };

            var mockSet = new Mock<DbSet<Review>>();
            mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(review);

            //_contextMock.Setup(c => c.Reviews).Returns(mockSet.Object);

            // Act
            var result = await _reviewService.DeleteReviewAsync("user1", 1);

            // Assert
            Assert.True(result);
            //_contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ReportReviewAsync_ReportsReview_WhenReviewExists() {
            // Arrange
            var review = new Review { Id = 1, Comment = "Great game!" };
            var reportReviewDTO = new ReportReviewDTO { ReviewId = 1, Reason = "Spam" };
            var reportedReview = new ReportedReview { ReviewId = 1, Reason = "Spam" };

            var mockSet = new Mock<DbSet<Review>>();
            mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(review);

            //_contextMock.Setup(c => c.Reviews).Returns(mockSet.Object);
            _mapperMock.Setup(m => m.Map<ReportedReview>(It.IsAny<ReportReviewDTO>())).Returns(reportedReview);

            // Act
            var result = await _reviewService.ReportReviewAsync("user1", reportReviewDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reportReviewDTO.ReviewId, result.ReviewId);
            //_contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}