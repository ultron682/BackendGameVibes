using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using BackendGameVibes.Models;

namespace BackendGameVibes.IServices {
    public interface IAccountService : IDisposable {
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<string> GenerateJwtTokenAsync(UserGameVibes user);
        Task<object?> GetAccountInfoAsync(string userId);
        Task<object?> GetPublicAccountInfoAsync(string userId);
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
        Task<object[]> FindUsersNickAndIdsByNickname(string myUserId, string myNickname, string searchName);
        Task<(bool, bool, FriendRequest?)> SendFriendRequestAsync(string senderId, string receiverId);
        Task<bool> ConfirmFriendRequestAsync(string userId, string friendId);
        Task<bool> RevokeFriendRequestAsync(string userId, string friendId);
        Task<bool> RemoveFriendAsync(string userId, string friendId);
        Task<IEnumerable<object>> GetAllFriendsOfUser(string userId);
        Task<IEnumerable<object>> GetFriendRequestsForUser(string userId);
        Task<bool> SendLockedOutAccountEmailAsync(string email, UserGameVibes user);
        Task<bool> UpdateProfilePictureAsync(string userId, byte[] imageData);
        Task<bool> UpdateProfileDescriptionAsync(string userId, string description);
        Task<bool> SendGeneralEmailToUserAsync(UserGameVibes user, string subject, string message);
        Task<(ActionCode? actionCode, bool isAlreadyExistValidExpiryDate)> SendCloseAccountRequestAsync(string userId);
        Task<bool> ConfirmCloseAccountRequest(string userId, string confirmationCode);
    }
}