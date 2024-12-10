namespace BackendGameVibes.Tests.Controllers;

using AutoMapper;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;


public class AdministrationControllerTests {
    private readonly Mock<UserManager<UserGameVibes>> _mockUserManager;
    private readonly Mock<IHostApplicationLifetime> _mockApplicationLifetime;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IForumPostService> _mockForumPostService;
    private readonly Mock<IForumThreadService> _mockForumThreadService;
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly Mock<IGameService> _mockGameService;
    private readonly Mock<IForumRoleService> _mockForumRoleService;
    private readonly Mock<IAdministrationService> _mockAdministrationService;

    private readonly AdministrationController _controller;

    public AdministrationControllerTests() {
        // Mockowanie zależności
        _mockUserManager = new Mock<UserManager<UserGameVibes>>(
            Mock.Of<IUserStore<UserGameVibes>>(), null, null, null, null, null, null, null, null
        );
        _mockApplicationLifetime = new Mock<IHostApplicationLifetime>();
        _mockMapper = new Mock<IMapper>();
        _mockAccountService = new Mock<IAccountService>();
        _mockRoleService = new Mock<IRoleService>();
        _mockForumPostService = new Mock<IForumPostService>();
        _mockForumThreadService = new Mock<IForumThreadService>();
        _mockReviewService = new Mock<IReviewService>();
        _mockGameService = new Mock<IGameService>();
        _mockForumRoleService = new Mock<IForumRoleService>();
        _mockAdministrationService = new Mock<IAdministrationService>();

        _controller = new AdministrationController(
            _mockUserManager.Object,
            _mockApplicationLifetime.Object,
            _mockMapper.Object,
            _mockAccountService.Object,
            _mockRoleService.Object,
            _mockForumPostService.Object,
            _mockForumThreadService.Object,
            _mockReviewService.Object,
            _mockGameService.Object,
            _mockForumRoleService.Object,
            _mockAdministrationService.Object
        );
    }

    [Fact]
    public async Task DeleteUser_ValidUserId_ReturnsOkResult() {
        // Arrange
        var userId = "test-user-id";
        var user = new UserGameVibes { Id = userId, UserName = "testuser" };

        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteUser_InvalidUserId_ReturnsNotFoundResult() {
        // Arrange
        var userId = "nonexistent-user-id";

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteUser_DeletionFails_ReturnsBadRequestResult() {
        // Arrange
        var userId = "test-user-id";
        var user = new UserGameVibes { Id = userId, UserName = "testuser" };

        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Deletion failed." }));

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAllUsersWithRoles_ShouldReturnUsersWithRoles() {
        // Arrange
        var users = new List<object>
        {
            "user1 data",
            "user2 data"
        };

        _mockAdministrationService.Setup(aservice => aservice.GetAllUsersWithRolesAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsersWithRoles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsType<List<object>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count);
    }
}
