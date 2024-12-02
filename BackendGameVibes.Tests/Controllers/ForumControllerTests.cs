namespace BackendGameVibes.Tests.Controllers;

using Xunit;
using Moq;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class ForumControllerTests {
    private readonly Mock<IForumPostService> _mockPostService;
    private readonly Mock<IForumThreadService> _mockThreadService;
    private readonly Mock<IForumRoleService> _mockRoleService;
    private readonly ForumController _controller;

    public ForumControllerTests() {
        _mockPostService = new Mock<IForumPostService>();
        _mockThreadService = new Mock<IForumThreadService>();
        _mockRoleService = new Mock<IForumRoleService>();

        _controller = new ForumController(_mockPostService.Object, _mockThreadService.Object, _mockRoleService.Object);
    }

    [Fact]
    public async Task GetThreadsGroupBySections_ReturnsOkWithThreads() {
        // Arrange
        var mockThreads = new List<object> { new { Id = 1, Title = "Test Thread" } };
        _mockThreadService
            .Setup(s => s.GetThreadsGroupBySectionsAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(mockThreads);

        // Act
        var result = await _controller.GetThreadsGroupBySections();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockThreads, okResult.Value);
    }

    [Fact]
    public async Task GetThreadWithPosts_ThreadNotFound_ReturnsNotFound() {
        // Arrange
        _mockThreadService
            .Setup(s => s.GetThreadWithPostsAsync(It.IsAny<int>(), null, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((object)null);

        // Act
        var result = await _controller.GetThreadWithPosts(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetThreadWithPosts_ThreadExists_ReturnsOk() {
        // Arrange
        var mockThread = new { Id = 1, Title = "Test Thread", Posts = new List<object>() };
        _mockThreadService
            .Setup(s => s.GetThreadWithPostsAsync(It.IsAny<int>(), null, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(mockThread);

        // Act
        var result = await _controller.GetThreadWithPosts(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockThread, okResult.Value);
    }

    [Fact]
    public async Task CreateThread_InvalidModelState_ReturnsBadRequest() {
        // Arrange
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.CreateThread(new NewForumThreadDTO());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Wrong ForumThreadDTO", badRequestResult.Value);
    }


}
