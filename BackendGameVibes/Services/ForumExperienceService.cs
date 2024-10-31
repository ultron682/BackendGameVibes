using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class ForumExperienceService : IForumExperienceService {
        private readonly PointsSettings _pointsSettings;
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly ApplicationDbContext _context;

        public ForumExperienceService(PointsSettings pointsSettings, UserManager<UserGameVibes> userManager, ApplicationDbContext context) {
            _pointsSettings = pointsSettings;
            _userManager = userManager;
            _context = context;
        }

        public async Task<int?> AddThreadPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.OnAddThreadPoints);
        }

        public async Task<int?> AddPostPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.OnAddPostPoints);
        }

        public async Task<int?> AddReviewPoints(string userId) {
            return await IncreaseExperiencePointsForUserAsync(userId, _pointsSettings.OnAddReviewPoints);
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
                .FirstOrDefaultAsync(fr => fr.Threshold <= user.ExperiencePoints);

            if (forumRole != null) {
                user.ForumRole = forumRole;
            }

            await _context.SaveChangesAsync();
            return user.ExperiencePoints;
        }
    }
}
