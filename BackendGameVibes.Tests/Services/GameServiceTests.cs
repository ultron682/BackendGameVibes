using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Steam;
using BackendGameVibes.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Services;

public class GameServiceTests {
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly Mock<ISteamService> _steamServiceMock;
    private readonly GameService _gameService;

    public GameServiceTests() {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContextMock = new Mock<ApplicationDbContext>(options);
        _steamServiceMock = new Mock<ISteamService>();
        _gameService = new GameService(_dbContextMock.Object, _steamServiceMock.Object);
    }

    [Fact]
    public void FindSteamAppByName_ReturnsMatchingApps() {
        // Arrange
        var searchResult = new[] {
            new SteamApp { Name = "Test Game" }
        };

        // Setup mock, aby zwrócić dane
        _steamServiceMock.Setup(s => s.FindSteamApp(It.IsAny<string>())).Returns(searchResult);

        // Act
        var result = _gameService.FindSteamAppByName("Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Game", result[0].Name);
    }

    [Fact]
    public void FindSteamAppByName_ReturnsEmpty_WhenNoMatchingApps() {
        // Arrange
        var searchResult = new SteamApp[0]; // Brak wyników
        _steamServiceMock.Setup(s => s.FindSteamApp(It.IsAny<string>())).Returns(searchResult);

        // Act
        var result = _gameService.FindSteamAppByName("NonExistentGame");

        // Assert
        Assert.Empty(result); // Powinno zwrócić pustą tablicę
    }
}
