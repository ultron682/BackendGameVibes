namespace BackendGameVibes.Tests.IntegrationTests;

using BackendGameVibes.Data;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Program = BackendGameVibes.Program;


public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>> {
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;

    public IntegrationTestBase(WebApplicationFactory<Program> factory) {
        _factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null) {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options => {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    protected async Task<string> GetJwtTokenAsync(string email, string password) {
        var loginDto = new LoginDTO {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/account/login", loginDto);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return loginResponse!.AccessToken!;
    }
}
