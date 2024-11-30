using AutoMapper;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace BackendGameVibes.Tests.Controllers {
    public class ReviewControllerTests {
        private readonly ReviewController _controller;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        public ReviewControllerTests() {
            _reviewServiceMock = new Mock<IReviewService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ReviewController(_reviewServiceMock.Object, _mapperMock.Object);
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


    }
}
