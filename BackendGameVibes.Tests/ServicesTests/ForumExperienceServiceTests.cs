namespace BackendGameVibes.Tests.Services;

using BackendGameVibes.Data;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.User;
using BackendGameVibes.Services.Forum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

public class ForumExperienceServiceTests {
    private readonly Mock<IOptions<ExperiencePointsSettings>> _pointsSettingsMock;
    private readonly Mock<UserManager<UserGameVibes>> _userManagerMock;
    private readonly ApplicationDbContext _dbContext;

    public ForumExperienceServiceTests() {
        _pointsSettingsMock = new Mock<IOptions<ExperiencePointsSettings>>();
        _userManagerMock = MockUserManager(new List<UserGameVibes>());
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddThreadPoints_IncreasesExperiencePointsAndUpdatesRole() {
        // Arrange
        const string userId = "test-user";
        _pointsSettingsMock.Setup(p => p.Value).Returns(new ExperiencePointsSettings { OnAddThreadPoints = 10 });

        var user = new UserGameVibes {
            Id = userId,
            ExperiencePoints = 0,
            ForumRole = null
        };

        var newRole = new ForumRole { Id = 1, Name = "Newbie", Threshold = 10 };

        _dbContext.Users.Add(user);
        _dbContext.ForumRoles.Add(newRole);
        await _dbContext.SaveChangesAsync();

        var service = new ForumExperienceService(_pointsSettingsMock.Object, _dbContext);

        // Act
        var newExperience = await service.AddThreadPoints(userId);

        // Assert
        Assert.Equal(10, newExperience);
        Assert.Equal(newRole, user.ForumRole);
    }

    [Fact]
    public async Task AddPostPoints_IncreasesExperiencePointsWithoutChangingRole() {
        // Arrange
        const string userId = "test-user";
        _pointsSettingsMock.Setup(p => p.Value).Returns(new ExperiencePointsSettings { OnAddPostPoints = 5 });

        var user = new UserGameVibes {
            Id = userId,
            ExperiencePoints = 3,
            ForumRole = new ForumRole { Id = 1, Name = "Beginner", Threshold = 0 }
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var service = new ForumExperienceService(_pointsSettingsMock.Object, _dbContext);

        // Act
        var newExperience = await service.AddPostPoints(userId);

        // Assert
        Assert.Equal(8, newExperience);
        Assert.Equal("Beginner", user.ForumRole.Name);
    }

    [Fact]
    public async Task AddReviewPoints_ReturnsMinusOne_WhenUserDoesNotExist() {
        // Arrange
        const string nonExistentUserId = "non-existent-user";
        _pointsSettingsMock.Setup(p => p.Value).Returns(new ExperiencePointsSettings { OnAddReviewPoints = 15 });

        var service = new ForumExperienceService(_pointsSettingsMock.Object, _dbContext);

        // Act
        var result = await service.AddReviewPoints(nonExistentUserId);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task AddNewFriendPoints_IncreasesExperiencePointsAndChangesRole_WhenThresholdIsReached() {
        // Arrange
        const string userId = "test-user";
        _pointsSettingsMock.Setup(p => p.Value).Returns(new ExperiencePointsSettings { OnAddNewFriendPoints = 20 });

        var user = new UserGameVibes {
            Id = userId,
            ExperiencePoints = 80,
            ForumRole = new ForumRole { Id = 1, Name = "Intermediate", Threshold = 50 }
        };

        var advancedRole = new ForumRole { Id = 2, Name = "Advanced", Threshold = 100 };

        _dbContext.Users.Add(user);
        _dbContext.ForumRoles.Add(advancedRole);
        await _dbContext.SaveChangesAsync();

        var service = new ForumExperienceService(_pointsSettingsMock.Object, _dbContext);

        // Act
        var newExperience = await service.AddNewFriendPoints(userId);

        // Assert
        Assert.Equal(100, newExperience);
        Assert.Equal("Advanced", user.ForumRole.Name);
    }

    private Mock<UserManager<UserGameVibes>> MockUserManager(List<UserGameVibes> users) {
        var store = new Mock<IUserStore<UserGameVibes>>();
        var mock = new Mock<UserManager<UserGameVibes>>(store.Object, null, null, null, null, null, null, null, null);

        mock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string id) => users.FirstOrDefault(u => u.Id == id));
        return mock;
    }
}
