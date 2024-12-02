using BackendGameVibes.Data;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models.DTOs;

namespace BackendGameVibes.Services {
    public class AdministrationService : IAdministrationService {
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public AdministrationService(ApplicationDbContext context,
            UserManager<UserGameVibes> userManager,
            IAccountService accountService,
            IRoleService roleService,
            IForumPostService forumPostService,
            IForumThreadService forumThreadService,
            IReviewService reviewService,
            IGameService gameService,
            IForumRoleService forumRoleService) {
            _context = context;
            _accountService = accountService;
            _userManager = userManager;
        }

        public async Task<IEnumerable<object>> GetAllUsersWithRolesAsync() {
            var users = await _context.Users
                .Select(u => new {
                    u.Id,
                    u.Email,
                    u.EmailConfirmed,
                    u.UserName,
                    ForumRoleName = u.ForumRole!.Name,
                    u.ExperiencePoints,
                    u.PhoneNumber,
                    u.PhoneNumberConfirmed,
                    u.AccessFailedCount,
                    u.Description
                }).ToArrayAsync();

            var usersWithRoles = new List<object>();
            foreach (var user in users) {
                var userGameVibes = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(userGameVibes);
                usersWithRoles.Add(new {
                    user.Id,
                    user.Email,
                    user.EmailConfirmed,
                    user.UserName,
                    user.ForumRoleName,
                    user.ExperiencePoints,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.AccessFailedCount,
                    user.Description,
                    Roles = roles.ToArray()
                });
            }
            return usersWithRoles;
        }

        public async Task<object?> GetUserAsync(string userId) {
            var accountInfo = await _accountService.GetBasicAccountInfoAsync(userId);
            return accountInfo;
        }

        public async Task<bool> AddUserAsync(RegisterDTO newUserData) {
            IdentityResult result = await _accountService.RegisterUserAsync(newUserData);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<(UserGameVibes user, IList<string> roles)> UpdateUserAsync(UserGameVibesDTO userDTO) {
            var userGameVibes = await _userManager.FindByIdAsync(userDTO.Id);
            if (userGameVibes == null)
                throw new Exception("UserNotFound");

            IList<string> userRoles = await _userManager.GetRolesAsync(userGameVibes);

            if (!string.IsNullOrEmpty(userDTO.UserName))
                userGameVibes.UserName = userDTO.UserName;

            if (!string.IsNullOrEmpty(userDTO.Description))
                userGameVibes.Description = userDTO.Description;

            if (userDTO.ExperiencePoints != null)
                userGameVibes.ExperiencePoints = userDTO.ExperiencePoints;

            if (userDTO.ForumRoleId != null)
                userGameVibes.ForumRoleId = userDTO.ForumRoleId;

            if (!string.IsNullOrEmpty(userDTO.Email)) {
                var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(userGameVibes, userDTO.Email);
                var changeEmailResult = await _userManager.ChangeEmailAsync(userGameVibes, userDTO.Email, emailChangeToken);
                if (!changeEmailResult.Succeeded)
                    throw new Exception("FailedToChangeEmail");
            }

            if (!string.IsNullOrEmpty(userDTO.Password)) {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userGameVibes);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(userGameVibes, resetToken, userDTO.Password);
                if (!resetPasswordResult.Succeeded)
                    throw new Exception("FailedToResetPassword");
            }

            if (!string.IsNullOrEmpty(userDTO.PhoneNumber))
                await _userManager.SetPhoneNumberAsync(userGameVibes, userDTO.PhoneNumber);

            if (userDTO.PhoneNumberConfirmed != null)
                userGameVibes.PhoneNumberConfirmed = userDTO.PhoneNumberConfirmed ?? false;

            if (!string.IsNullOrEmpty(userDTO.RoleName)) {
                foreach (var role in userRoles)
                    await _userManager.RemoveFromRoleAsync(userGameVibes, role);

                await _userManager.AddToRoleAsync(userGameVibes, userDTO.RoleName!);
            }

            if (userDTO.LockoutEnabled != null)
                await _userManager.SetLockoutEnabledAsync(userGameVibes, userDTO.LockoutEnabled.Value);

            if (userDTO.LockoutEnd != null)
                await _userManager.SetLockoutEndDateAsync(userGameVibes, userDTO.LockoutEnd);

            if (userDTO.AccessFailedCount != null)
                userGameVibes.AccessFailedCount = userDTO.AccessFailedCount.Value;

            await _userManager.UpdateAsync(userGameVibes);

            var currentRoles = await _userManager.GetRolesAsync(userGameVibes);
            return (userGameVibes, currentRoles);
        }

        public async Task<bool> DeleteReviewAsync(int id) {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) {
                return false;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(int id) {
            var post = await _context.ForumPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) {
                return false;
            }

            _context.ForumPosts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetReportedReviewsAsync() {
            return await _context.ReportedReviews
                .Include(r => r.ReporterUser)
                .Include(r => r.Review)
                .Where(r => r.IsFinished == false)
                .Select(r => new {
                    r.Id,
                    r.ReporterUserId,
                    ReporterUserName = r.ReporterUser!.UserName,
                    r.ReviewId,
                    r.Review!.Comment,
                    r.Reason,
                    r.IsFinished
                })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<object>> GetReportedPostsAsync() {
            return await _context.ReportedForumPosts
                .Include(p => p.ReporterUser)
                .Include(p => p.ForumPost)
                .Where(p => p.IsFinished == false)
                .Select(p => new {
                    p.Id,
                    p.ReporterUserId,
                    ReporterUserName = p.ReporterUser!.UserName,
                    p.ForumPostId,
                    p.ForumPost!.Content,
                    p.Reason,
                    p.IsFinished
                })
                .ToArrayAsync();
        }

        public async Task AddGameGenreAsync(ValueModel genreModel) {
            _context.Genres.Add(new Models.Games.Genre { Name = genreModel.Value });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateGameGenreAsync(int genreId, ValueModel genreModel) {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
            if (genre == null) {
                return false;
            }

            genre.Name = genreModel.Value;
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveGameGenreAsync(int genreId) {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
            if (genre == null) {
                return false;
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddGamePlatformAsync(ValueModel platformModel) {
            _context.Platforms.Add(new Models.Games.Platform { Name = platformModel.Value });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateGamePlatformAsync(int platformId, ValueModel platformModel) {
            var platform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == platformId);
            if (platform == null) {
                return false;
            }

            platform.Name = platformModel.Value;
            _context.Platforms.Update(platform);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveGamePlatformAsync(int platformId) {
            var platform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == platformId);
            if (platform == null) {
                return false;
            }

            _context.Platforms.Remove(platform);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
