namespace BackendGameVibes.Tests.IntegrationTests;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

public class GameControllerIntegrationTests : IntegrationTestBase {
    public GameControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetGames_ReturnsOk() {
        // Act
        var response = await _client.GetAsync("/api/games");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetGameById_ReturnsNotFound_WhenGameDoesNotExist() {
        // Act
        var response = await _client.GetAsync("/api/games/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGameById_ReturnsOk_WhenGameExists() {
        // Arrange
        await TryRegisterAndConfirmTestUser();

        var token = await GetJwtTokenAsync("admin999@admin.com", "Test123.");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


        var createResponse = await _client.PostAsJsonAsync("/api/games?steamGameId=292030", "");
        createResponse.EnsureSuccessStatusCode();

        string content = await createResponse.Content.ReadAsStringAsync();

        // Act
        var response = await _client.GetAsync($"/api/games/1"); // jedyna gra w bazie

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateGame_ReturnsUnauthorized_WhenNotAuthenticated() {
        // Act
        var response = await _client.PostAsJsonAsync("/api/games", 292030);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
