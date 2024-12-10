namespace BackendGameVibes.Tests.Controllers;

using AutoMapper;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;


public class ReviewControllerTests {
    private readonly ReviewController _controller;
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public ReviewControllerTests() {
        _reviewServiceMock = new Mock<IReviewService>();
        _mapperMock = new Mock<IMapper>();
        _controller = new ReviewController(_reviewServiceMock.Object, _mapperMock.Object);

        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.NameIdentifier, "user1")
        }));
    }

    [Fact]
    public async Task GetAllReviews_ReturnsOkResult_WithReviews() {
        // Arrange
        var getAllReviewsResponse = new GetAllReviewsResponse() {
            Data = [new Review { Id = 1 }, new Review { Id = 2 }],
            CurrentPage = 1,
            PageSize = 10,
            TotalPages = 1,
            TotalResults = 2
        };

        _reviewServiceMock.Setup(s => s.GetAllReviewsAsync(1, 10)).ReturnsAsync(getAllReviewsResponse);

        // Act
        var result = await _controller.GetAllReviews(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(getAllReviewsResponse, okResult.Value);
    }

    [Fact]
    public async Task GetReviewById_ReturnsOkResult_WhenReviewExists() {
        // Arrange
        var review = new Review { Id = 1 };
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(1)).ReturnsAsync(review);

        // Act
        var result = await _controller.GetReviewById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(review, okResult.Value);
    }

    [Fact]
    public async Task GetReviewById_ReturnsNotFound_WhenReviewDoesNotExist() {
        // Arrange
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(1)).ReturnsAsync((Review?)null);

        // Act
        var result = await _controller.GetReviewById(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddReview_ReturnsOkResult_WhenReviewIsAdded() {
        // Arrange
        var reviewDto = new ReviewDTO {
            Comment = "Test content",
            GameId = 1,
            AudioScore = 2,
            GameplayScore = 6,
            GeneralScore = 7,
            GraphicsScore = 4
        };
        var review = new Review {
            Id = 1,
            Comment = "Test content",
            GameId = 1,
            AudioScore = 2,
            GameplayScore = 6,
            GeneralScore = 7,
            GraphicsScore = 4,
            UserGameVibesId = "user1"
        };

        _mapperMock.Setup(m => m.Map<Review>(reviewDto)).Returns(review);
        _reviewServiceMock.Setup(s => s.AddReviewAsync(review)).ReturnsAsync(review);
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(review.Id)).ReturnsAsync(review);


        // Act
        var result = await _controller.AddReview(reviewDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var addedReview = Assert.IsType<Review>(okResult.Value);
        Assert.Equal(review.Id, addedReview.Id);
    }

    [Fact]
    public async Task DeleteReview_ReturnsOkResult_WhenReviewIsDeleted() {
        // Arrange
        _reviewServiceMock.Setup(s => s.DeleteReviewAsync("user1", 1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteReview(1);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteReview_ReturnsNotFound_WhenReviewIsNotDeleted() {
        // Arrange
        _reviewServiceMock.Setup(s => s.DeleteReviewAsync("user1", 1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteReview(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
