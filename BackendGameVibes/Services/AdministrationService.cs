using BackendGameVibes.Data;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class AdministrationService : IAdministrationService {
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly IForumPostService _forumPostService;
        private readonly IForumThreadService _forumThreadService;
        private readonly IReviewService _reviewService;
        private readonly IGameService _gameService;
        private readonly IForumRoleService _forumRoleService;

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
            _roleService = roleService;
            _forumPostService = forumPostService;
            _forumThreadService = forumThreadService;
            _reviewService = reviewService;
            _gameService = gameService;
            _forumRoleService = forumRoleService;
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
    }
}
