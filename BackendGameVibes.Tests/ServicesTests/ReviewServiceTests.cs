﻿namespace BackendGameVibes.Tests.Services;

using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Services;
using Microsoft.EntityFrameworkCore;
using Moq;


public class ReviewServiceTests {
    private readonly ApplicationDbContext _context;
    private readonly Mock<IForumExperienceService> _forumExperienceServiceMock;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests() {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        _forumExperienceServiceMock = new Mock<IForumExperienceService>();


        _reviewService = new ReviewService(_context, null!, _forumExperienceServiceMock.Object);
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
            Id = 1,
            Comment = "Great game!",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            GeneralScore = 8,
            GraphicsScore = 9,
            AudioScore = 7,
            GameplayScore = 8,
            GameId = 2,
            UserGameVibesId = "user1ID"
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        // Act
        var result = await _reviewService.DeleteReviewAsync("user1ID", 1);

        // Assert
        Assert.True(result);
    }
}