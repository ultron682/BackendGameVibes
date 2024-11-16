using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.User;
using BackendGameVibes.Services;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using MailService = BackendGameVibes.Services.MailService;

namespace BackendGameVibes.Tests.Services;

public class AccountServiceTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<UserManager<UserGameVibes>> _mockUserManager;
    private readonly Mock<SignInManager<UserGameVibes>> _mockSignInManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HtmlTemplateService> _mockHtmlTemplateService;
    private readonly Mock<IForumExperienceService> _mockForumExperienceService;
    private readonly Mock<IActionCodesService> _mockActionCodesService;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        // Mockowanie DbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new Mock<ApplicationDbContext>(options);

        // Mockowanie UserManager
        var userStore = new Mock<IUserStore<UserGameVibes>>();
        _mockUserManager = new Mock<UserManager<UserGameVibes>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        // Mockowanie SignInManager
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<UserGameVibes>>();
        _mockSignInManager = new Mock<SignInManager<UserGameVibes>>(
            _mockUserManager.Object, httpContextAccessor.Object, userClaimsPrincipalFactory.Object, null, null, null, null);

        // Mockowanie RoleManager
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
            roleStore.Object, null, null, null, null);

        // Mockowanie pozostałych zależności
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHtmlTemplateService = new Mock<HtmlTemplateService>();
        _mockForumExperienceService = new Mock<IForumExperienceService>();
        _mockActionCodesService = new Mock<IActionCodesService>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        // Inicjalizacja AccountService
        _accountService = new AccountService(
            _mockContext.Object,
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockConfiguration.Object,
            null, // mail
            _mockHtmlTemplateService.Object,
            _mockRoleManager.Object,
            _mockForumExperienceService.Object,
            _mockActionCodesService.Object,
            _mockJwtTokenService.Object
        );
    }

    [Fact]
    public async Task ConfirmFriendRequestAsync_ShouldConfirmRequestAndAddFriend_WhenRequestExists()
    {
        // Arrange
        var userId = "user1";
        var friendId = "user2";

        var friendRequest = new FriendRequest
        {
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

    


}
