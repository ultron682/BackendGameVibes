using BackendGameVibes.Models;
using BackendGameVibes.Models.Requests;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.IServices {
    public interface IAccountService {
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<string> GenerateJwtToken(UserGameVibes user);
        Task<object> GetAccountInfoAsync(string userId, UserGameVibes userGameVibes);
        Task<UserGameVibes?> GetUserByEmailAsync(string email);
        Task<UserGameVibes?> GetUserByIdAsync(string userId);
        Task<SignInResult?> LoginUser(UserGameVibes user, string password);
        Task<IdentityResult> RegisterUser(RegisterRequest model);
        Task SaveTokenToDB(IdentityUserToken<string> userToken);
        Task<bool> SendConfirmationEmail(string email, UserGameVibes user);
        Task<bool> UpdateUserNameAsync(string userId, string newUsername);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}