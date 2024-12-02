using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.IServices {
    public interface IAdministrationService {
        Task<bool> AddUserAsync(RegisterDTO newUserData);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<object>> GetAllUsersWithRolesAsync();
        Task<object?> GetUserAsync(string userId);
        Task<(UserGameVibes user, IList<string> roles)> UpdateUserAsync(UserGameVibesDTO userDTO);
        Task<bool> DeleteReviewAsync(int id);
        Task<bool> DeletePostAsync(int id);
        Task<IEnumerable<object>> GetReportedReviewsAsync();
        Task<IEnumerable<object>> GetReportedPostsAsync();
        Task AddGameGenreAsync(ValueModel genreModel);
        Task<bool> UpdateGameGenreAsync(int genreId, ValueModel genreModel);
        Task<bool> RemoveGameGenreAsync(int genreId);
        Task AddGamePlatformAsync(ValueModel platformModel);
        Task<bool> UpdateGamePlatformAsync(int platformId, ValueModel platformModel);
        Task<bool> RemoveGamePlatformAsync(int platformId);
    }
}