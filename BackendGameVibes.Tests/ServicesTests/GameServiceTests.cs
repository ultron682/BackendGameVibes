using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Steam;
using BackendGameVibes.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Services;

public class GameServiceTests {
    private readonly ApplicationDbContext _context;
    private readonly Mock<ISteamService> _steamServiceMock;
    private readonly GameService _gameService;
    private readonly Mock<DbSet<Game>> _gamesDbSetMock;


    public GameServiceTests() {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _gamesDbSetMock = new Mock<DbSet<Game>>();
        _context.Database.EnsureCreated();

        _steamServiceMock = new Mock<ISteamService>();

        _gameService = new GameService(_context, _steamServiceMock.Object);
    }

    [Fact]
    public void FindSteamAppByName_ReturnsMatchingApps() {
        // Arrange
        var searchResult = new[] {
            new SteamApp { Name = "Test Game" }
        };
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

    [Fact]
    public async Task GetFilteredGamesAsync_ReturnsCorrectData() {
        // Arrange
        var filters = new FiltersGamesDTO {
            RatingMin = 0,
            RatingMax = 5,
        };

        var game = new Game {
            Title = "Test Game",
            LastCalculatedRatingFromReviews = 4.5f
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        // Act
        var result = await _gameService.GetFilteredGamesAsync(filters);

        // Assert
        var castedResult = result as dynamic;
        Assert.NotNull(castedResult);
        Assert.NotNull(castedResult!.Data);
        Assert.True(castedResult.Data.Length > 0);
    }

    [Fact]
    public async Task AddGameAsync_AddsGame_WhenNotExists() {
        // Arrange
        var steamGameId = 292030; // The Witcher 3

        _steamServiceMock.Setup(s => s.GetInfoGame(It.IsAny<int>())).ReturnsAsync(new GameData() { name = "The Witcher 3" });

        // Act
        (Game? game, bool isCreated) = await _gameService.AddGameAsync(steamGameId);

        // Assert
        Assert.NotNull(game);
        Assert.True(isCreated); // Nowa gra została dodana
        Assert.Equal(steamGameId, game.SteamId);
        Assert.Equal("The Witcher 3", game.Title);
    }

    [Fact]
    public async Task AddGameAsync_ReturnsExistingGame_WhenAlreadyExists() {
        // Arrange
        var steamGameId = 12345;
        var existingGame = new Game {
            SteamId = steamGameId,
            Title = "Existing Game"
        };
        _context.Games.Add(existingGame);
        await _context.SaveChangesAsync();

        // Act
        (Game? game, bool isCreated) = await _gameService.AddGameAsync(steamGameId);

        // Assert
        Assert.NotNull(game);
        Assert.False(isCreated); // Gra już istnieje, więc nie powinna zostać dodana
        Assert.Equal("Existing Game", game!.Title);
    }
}
