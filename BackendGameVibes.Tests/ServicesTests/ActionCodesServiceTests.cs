using BackendGameVibes.Data;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Services {


    public class ActionCodesServiceTests {
        private readonly Mock<ApplicationDbContext> _dbContextMock;

        public ActionCodesServiceTests() {
            // Tworzenie mocka DbContext z wirtualnymi DbSet
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContextMock = new Mock<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GenerateUniqueActionCode_ReturnsNewActionCode_WhenNoExistingCodeForUser() {
            // Arrange
            var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var service = new ActionCodesService(dbContext);
            var userId = "test-user";

            // Act
            var (generatedCode, isExisting) = await service.GenerateUniqueActionCode(userId);

            // Assert
            Assert.NotNull(generatedCode);
            Assert.Equal(userId, generatedCode.UserId);
            Assert.False(isExisting);
            Assert.True(DateTime.Now <= generatedCode.ExpirationDateTime);
        }

        [Fact]
        public async Task GenerateUniqueActionCode_ReturnsExistingCode_WhenValidCodeExists() {
            // Arrange
            var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var service = new ActionCodesService(dbContext);
            var userId = "test-user";
            var existingCode = new ActionCode {
                Code = "123456",
                CreatedDateTime = DateTime.Now.AddMinutes(-30),
                ExpirationDateTime = DateTime.Now.AddMinutes(30),
                UserId = userId
            };

            dbContext.ActiveActionCodes.Add(existingCode);
            await dbContext.SaveChangesAsync();

            // Act
            var (generatedCode, isExisting) = await service.GenerateUniqueActionCode(userId);

            // Assert
            Assert.NotNull(generatedCode);
            Assert.Equal(existingCode.Code, generatedCode.Code);
            Assert.True(isExisting);
        }

        [Fact]
        public async Task GenerateUniqueActionCode_GeneratesNewCode_WhenExistingCodeIsExpired() {
            // Arrange
            var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            var service = new ActionCodesService(dbContext);
            var userId = "test-user";
            var expiredCode = new ActionCode {
                Code = "123456",
                CreatedDateTime = DateTime.Now.AddHours(-2),
                ExpirationDateTime = DateTime.Now.AddHours(-1),
                UserId = userId
            };

            dbContext.ActiveActionCodes.Add(expiredCode);
            await dbContext.SaveChangesAsync();

            // Act
            var (generatedCode, isExisting) = await service.GenerateUniqueActionCode(userId);

            // Assert
            Assert.NotNull(generatedCode);
            Assert.NotEqual(expiredCode.Code, generatedCode.Code);
            Assert.False(isExisting);
            Assert.True(DateTime.Now <= generatedCode.ExpirationDateTime);
        }
    }

}
