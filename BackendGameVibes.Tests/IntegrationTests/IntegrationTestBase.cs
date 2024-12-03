namespace BackendGameVibes.Tests.IntegrationTests;

using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.DTOs.Responses;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
            builder.UseEnvironment("IntegrationTest");

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

        var response = await _client.PostAsJsonAsync("/account/login", loginDto);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return loginResponse!.AccessToken!;
    }

    public async Task TryRegisterAndConfirmTestUser() {
        using (var scope = _factory.Services.CreateScope()) {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserGameVibes>>();

            if (!await roleManager.RoleExistsAsync("user")) {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            var registerDto = new RegisterDTO {
                Email = "test999@test.com",
                Password = "Test123.",
                UserName = "Test999",
            };

            var registerResponse = await _client.PostAsJsonAsync("/account/register", registerDto);
            if (registerResponse.IsSuccessStatusCode) {
                var user = await accountService.GetUserByEmailAsync(registerDto.Email);
                var token = await accountService.GenerateEmailConfirmationTokenAsync(user!.Id);
                await accountService.ConfirmEmailAsync(user.Id, token);
            }



            if (!await roleManager.RoleExistsAsync("admin")) {
                await roleManager.CreateAsync(new IdentityRole("admin"));

                //Admin           
                var newProfilePicture = new ProfilePicture();
                var newUser = new UserGameVibes {
                    UserName = "admin999",
                    Email = "admin999@admin.com",
                    EmailConfirmed = true,
                    ProfilePicture = newProfilePicture
                };
                string userPWD = "Test123.";

                IdentityResult chkUser = await userManager.CreateAsync(newUser, userPWD);

                newUser = await userManager.FindByEmailAsync(newUser.Email);
                var result1 = await userManager.AddToRoleAsync(newUser!, "admin");
            }



        }

    }
}
