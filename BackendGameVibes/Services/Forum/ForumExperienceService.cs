using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackendGameVibes.Services.Forum {
    public class ForumExperienceService : IForumExperienceService {
        private readonly IOptions<ExperiencePointsSettings> _pointsSettings;
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly ApplicationDbContext _context;

        public ForumExperienceService(IOptions<ExperiencePointsSettings> pointsSettings, UserManager<UserGameVibes> userManager, ApplicationDbContext context) {
            _pointsSettings = pointsSettings;
            _userManager = userManager;
            _context = context;
        }

        public async Task<int?> AddThreadPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.Value.OnAddThreadPoints);
        }

        public async Task<int?> AddPostPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.Value.OnAddPostPoints);
        }

        public async Task<int?> AddReviewPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.Value.OnAddReviewPoints);
        }

        public async Task<int?> AddNewFriendPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.Value.OnAddNewFriendPoints);
        }

        private async Task<int?> IncreaseExperiencePointsForUserAsync(string userId, int count) {
            var user = await _context.Users
                .Include(u => u.ForumRole)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) {
                return -1;
            }

            user.ExperiencePoints += count;

            var forumRole = await _context.ForumRoles
                .OrderByDescending(fr => fr.Threshold)
                .FirstOrDefaultAsync(fr => user.ExperiencePoints >= fr.Threshold);

            if (forumRole != null) {
                user.ForumRole = forumRole;
            }

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            return user.ExperiencePoints;
        }
    }
}
