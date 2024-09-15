using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.IServices
{
    public interface IAccountService
    {
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<string> GenerateJwtToken(UserGameVibes user);
        Task<object> GetAccountInfoAsync(string userId);
        Task<UserGameVibes?> GetUserByEmailAsync(string email);
        Task<UserGameVibes?> GetUserByIdAsync(string userId);
        Task<SignInResult?> LoginUser(UserGameVibes user, string password);
        Task<IdentityResult> RegisterUser(RegisterRequestCodeShare model);
        Task<bool> SendConfirmationEmail(string email, UserGameVibes user);
        Task<bool> UpdateUserNameAsync(string userId, string newUsername);
    }
}