using BackendGameVibes.Models.Requests;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.IServices
{
    public interface IAccountService {
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<string> GenerateJwtTokenAsync(UserGameVibes user);
        Task<object> GetAccountInfoAsync(string userId, UserGameVibes userGameVibes);
        Task<UserGameVibes?> GetUserByEmailAsync(string email);
        Task<UserGameVibes?> GetUserByIdAsync(string userId);
        Task<SignInResult?> LoginUserAsync(UserGameVibes user, string password);
        Task<IdentityResult> RegisterUserAsync(RegisterDTO model);
        Task SaveTokenToDbAsync(IdentityUserToken<string> userToken);
        Task<bool> SendConfirmationEmailAsync(string email, UserGameVibes user);
        Task<bool> UpdateUserNameAsync(string userId, string newUsername);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<(bool, string)> StartResetPasswordAsync(string email);
        Task<IdentityResult> ConfirmResetPasswordAsync(string email, string resetToken, string newPassword);
        Task<object[]> FindUsersNickAndIdsByNickname(string myNickname, string searchName);
        Task<(bool, bool)> SendFriendRequestAsync(string senderId, string receiverId);
        Task<bool> ConfirmFriendRequestAsync(string userId, string friendId);
        Task<bool> RevokeFriendRequestAsync(string userId, string friendId);
        Task<bool> RemoveFriendAsync(string userId, string friendId);
        Task<IEnumerable<object>> GetAllFriendsOfUser(string userId);
    }
}