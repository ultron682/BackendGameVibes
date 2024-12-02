using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.Services {
    public interface IAdministrationService {
        Task<bool> AddUserAsync(RegisterDTO newUserData);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<object>> GetAllUsersWithRolesAsync();
        Task<object?> GetUserAsync(string userId);
        Task<(UserGameVibes user, IList<string> roles)> UpdateUserAsync(UserGameVibesDTO userDTO);
        Task<bool> DeleteReviewAsync(int id);
        Task<bool> DeletePostAsync(int id);
    }
}