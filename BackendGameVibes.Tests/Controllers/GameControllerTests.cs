using Moq;
using Microsoft.AspNetCore.Mvc;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using BackendGameVibes.Models.Games;

namespace BackendGameVibes.Tests.Controllers;

public class GameControllerTests {
    private readonly GameController _controller;
    private readonly Mock<IGameService> _gameServiceMock;

    public GameControllerTests() {
        _gameServiceMock = new Mock<IGameService>();
        _controller = new GameController(_gameServiceMock.Object);
    }

    [Fact]
    public void FindSteamAppByName_ReturnsOk_WhenAppsFound() {
        // Arrange
        var appName = "TestApp";
        var steamApps = new SteamApp[] { new SteamApp { Name = appName } };
        _gameServiceMock.Setup(service => service.FindSteamAppByName(appName)).Returns(steamApps);

        // Act
        var result = _controller.FindSteamAppByName(appName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(steamApps, okResult.Value);
    }

    [Fact]
    public void FindSteamAppByName_ReturnsBadRequest_WhenNoAppsFound() {
        // Arrange
        var appName = "NonExistentApp";
        _gameServiceMock.Setup(service => service.FindSteamAppByName(appName)).Returns((SteamApp[])null);

        // Act
        var result = _controller.FindSteamAppByName(appName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetGames_ReturnsOk_WithListOfGames() {
        // Arrange
        var games = new object[] { new { Id = 1, Title = "Game1" } };
        _gameServiceMock.Setup(service => service.GetGames()).ReturnsAsync(games);

        // Act
        var result = await _controller.GetGames();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(games, okResult.Value);
    }

    [Fact]
    public async Task GetGame_ReturnsNotFound_WhenGameNotFound() {
        // Arrange
        int gameId = 999;
        _gameServiceMock.Setup(service => service.GetGame(gameId)).ReturnsAsync((object)null);

        // Act
        var result = await _controller.GetGame(gameId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetGame_ReturnsOk_WhenGameFound() {
        // Arrange
        int gameId = 1;
        var game = new { Id = gameId, Title = "Game1" };
        _gameServiceMock.Setup(service => service.GetGame(gameId)).ReturnsAsync(game);

        // Act
        var result = await _controller.GetGame(gameId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(game, okResult.Value);
    }

    [Fact]
    public async Task GetGenres_ReturnsOk_WhenGenresFound() {
        // Arrange
        var genres = new object[] { new { Id = 1, Name = "Genre1" } };
        _gameServiceMock.Setup(service => service.GetGenres()).ReturnsAsync(genres);

        // Act
        var result = await _controller.GetGenres();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(genres, okResult.Value);
    }

    [Fact]
    public async Task GetFilteredGames_ReturnsOK_WhenGamesFound() {
        // Arrange
        var filters = new FiltersGamesDTO();
        _gameServiceMock.Setup(service => service.GetFilteredGames(filters)).ReturnsAsync(new Game[] { new Game() { Title = "test", Description = "test" }, new Game() { Title = "test2", Description = "test2" } });

        // Act
        var result = await _controller.GetFilteredGames(filters);

        // Assert
        Assert.IsType<ActionResult<IEnumerable<object>>>(result);
    }
}
