namespace BackendGameVibes.Tests.IntegrationTests;

using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Program = BackendGameVibes.Program;


public class ForumControllerIntegrationTests : IntegrationTestBase {
    public ForumControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) {

    }

    [Fact]
    public async Task GetThreadsGroupBySections_ReturnsOk() {
        // Act
        var response = await _client.GetAsync("/api/forum/threads/sections");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateThread_ReturnsUnauthorized_WhenNotAuthenticated() {
        // Arrange
        var newThread = new NewForumThreadDTO {
            Title = "Test Thread",
            SectionId = 1,
            FirstForumPostContent = "This is a test post."
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/forum/threads", newThread);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetThreadById_ReturnsNotFound_WhenThreadDoesNotExist() {
        // Act
        var response = await _client.GetAsync("/api/forum/threads/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetThreadById_ReturnsOk_WhenThreadExists() {
        // Arrange
        await TryRegisterAndConfirmTestUser();

        var token = await GetJwtTokenAsync("test999@test.com", "Test123.");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var newThread = new NewForumThreadDTO {
            Title = "Existing Thread",
            SectionId = 1,
            FirstForumPostContent = "This is an existing post.",
        };

        var createResponse = await _client.PostAsJsonAsync("/api/forum/threads", newThread);

        createResponse.EnsureSuccessStatusCode();
        var createdThread = await createResponse.Content.ReadFromJsonAsync<GetThreadWithPostsResponse>();

        // Act
        var response = await _client.GetAsync($"/api/forum/threads/{createdThread!.NewForumThreadId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
