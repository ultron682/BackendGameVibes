using BackendGameVibes.Data;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.User;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Services {
    public class ForumExperienceServiceTests {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<UserManager<UserGameVibes>> _mockUserManager;
        private readonly Mock<IOptions<ExperiencePointsSettings>> _mockOptions;
        private readonly ForumExperienceService _service;

        public ForumExperienceServiceTests() {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockUserManager = new Mock<UserManager<UserGameVibes>>(Mock.Of<IUserStore<UserGameVibes>>(), null, null, null, null, null, null, null, null);
            _mockOptions = new Mock<IOptions<ExperiencePointsSettings>>();

            _mockOptions.Setup(x => x.Value)
                        .Returns(new ExperiencePointsSettings { OnAddThreadPoints = 10 });

            _service = new ForumExperienceService(_mockOptions.Object, _mockUserManager.Object, _mockContext.Object);
        }

        [Fact]
        public async Task AddThreadPoints_ValidUserId_IncreasesPoints() {
            // Arrange
            var userId = "test-user-id";
            var user = new UserGameVibes { Id = userId, ExperiencePoints = 0 };
            var forumRole = new ForumRole { Threshold = 10 };

            _mockContext.Setup(x => x.Users)
                        .Returns(MockDbSet(new List<UserGameVibes> { user }).Object);

            _mockContext.Setup(x => x.ForumRoles)
                        .Returns(MockDbSet(new List<ForumRole> { forumRole }).Object);

            // Act
            var newPoints = await _service.AddThreadPoints(userId);

            // Assert
            Assert.Equal(10, newPoints);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddThreadPoints_InvalidUserId_ReturnsMinusOne() {
            // Arrange
            var userId = "invalid-user-id";
            _mockContext.Setup(x => x.Users)
                        .Returns(MockDbSet(new List<UserGameVibes>()).Object);

            // Act
            var result = await _service.AddThreadPoints(userId);

            // Assert
            Assert.Equal(-1, result);
        }


        private Mock<DbSet<T>> MockDbSet<T>(List<T> data) where T : class {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

    }
}
