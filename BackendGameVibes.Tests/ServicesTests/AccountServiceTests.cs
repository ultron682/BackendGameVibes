namespace BackendGameVibes.Tests.Services;

using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.User;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;


public class AccountServiceTests {
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<UserManager<UserGameVibes>> _mockUserManager;
    private readonly Mock<SignInManager<UserGameVibes>> _mockSignInManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly Mock<HtmlTemplateService> _mockHtmlTemplateService;
    private readonly Mock<IForumExperienceService> _mockForumExperienceService;
    private readonly Mock<IActionCodesService> _mockActionCodesService;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly AccountService _accountService;

    public AccountServiceTests() {
        // Mockowanie DbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new Mock<ApplicationDbContext>(options);

        var userStore = new Mock<IUserStore<UserGameVibes>>();
        _mockUserManager = new Mock<UserManager<UserGameVibes>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<UserGameVibes>>();
        _mockSignInManager = new Mock<SignInManager<UserGameVibes>>(
            _mockUserManager.Object, httpContextAccessor.Object, userClaimsPrincipalFactory.Object, null, null, null, null);

        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
            roleStore.Object, null, null, null, null);

        _mockHtmlTemplateService = new Mock<HtmlTemplateService>();
        _mockForumExperienceService = new Mock<IForumExperienceService>();
        _mockActionCodesService = new Mock<IActionCodesService>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        _accountService = new AccountService(
            _mockContext.Object,
            _mockUserManager.Object,
            _mockSignInManager.Object,
            null!, // mail
            _mockHtmlTemplateService.Object,
            _mockRoleManager.Object,
            _mockForumExperienceService.Object,
            _mockActionCodesService.Object,
            _mockJwtTokenService.Object,
            new HttpClient()
        );
    }

    [Fact]
    public async Task ConfirmFriendRequestAsync_ShouldConfirmRequestAndAddFriend_WhenRequestExists() {
        // Arrange
        var userId = "user1";
        var friendId = "user2";

        var friendRequest = new FriendRequest {
            SenderUserId = friendId,
            ReceiverUserId = userId,
            IsAccepted = null
        };

        _mockContext.Setup(c => c.FriendRequests)
            .ReturnsDbSet(new List<FriendRequest> { friendRequest });


        _mockContext.Setup(c => c.Friends)
            .ReturnsDbSet(new List<Friend>());

        _mockForumExperienceService
            .Setup(f => f.AddNewFriendPoints(It.IsAny<string>()))
            .ReturnsAsync(23); // points

        // Act
        var result = await _accountService.ConfirmFriendRequestAsync(userId, friendId);

        // Assert
        Assert.True(result);
        Assert.Equal(true, friendRequest.IsAccepted);

        _mockContext.Verify(c => c.FriendRequests.Update(friendRequest), Times.Once);
        _mockContext.Verify(c => c.Friends.AddRange(It.IsAny<Friend>(), It.IsAny<Friend>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockForumExperienceService.Verify(f => f.AddNewFriendPoints(userId), Times.Once);
    }

    [Fact]
    public async Task RevokeFriendRequestAsync_ShouldSetRequestToRejected_WhenRequestExists() {
        // Arrange
        var userId = "user1";
        var friendId = "user2";

        var friendRequest = new FriendRequest {
            SenderUserId = friendId,
            ReceiverUserId = userId,
            IsAccepted = null
        };

        _mockContext.Setup(c => c.FriendRequests)
            .ReturnsDbSet(new List<FriendRequest> { friendRequest });

        // Act
        var result = await _accountService.RevokeFriendRequestAsync(userId, friendId);

        // Assert
        Assert.True(result);
        Assert.Equal(false, friendRequest.IsAccepted);

        _mockContext.Verify(c => c.FriendRequests.Update(friendRequest), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfilePictureAsync_ShouldUpdatePicture_WhenUserExists() {
        // Arrange
        var userId = "user1";
        var imageData = new byte[] { 0x01, 0x02 };

        var user = new UserGameVibes {
            Id = userId,
            ProfilePicture = new ProfilePicture()
        };

        _mockContext.Setup(c => c.Users)
            .ReturnsDbSet(new List<UserGameVibes> { user });

        _mockUserManager
            .Setup(um => um.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _accountService.UpdateProfilePictureAsync(userId, imageData);

        // Assert
        Assert.True(result);
        Assert.Equal(imageData, user.ProfilePicture!.ImageData);

        _mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);
    }


}
