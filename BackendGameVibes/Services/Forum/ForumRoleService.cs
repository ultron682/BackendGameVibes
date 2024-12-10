using BackendGameVibes.Data;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Forum;
using Microsoft.EntityFrameworkCore;


namespace BackendGameVibes.Services.Forum;

public class ForumRoleService : IForumRoleService {
    private readonly ApplicationDbContext _context;

    public ForumRoleService(ApplicationDbContext context, IForumPostService postService) {
        _context = context;
    }

    public async Task<IEnumerable<object>> GetForumRolesAsync() {
        var forumRoles = await _context.ForumRoles
         .OrderBy(fr => fr.Threshold)
         .Select(fr => new {
             fr.Id,
             fr.Name,
             currentThreshold = fr.Threshold,
         })
         .ToArrayAsync();

        return forumRoles;
    }

    public async Task<object?> AddForumRoleAsync(ForumRoleDTO addForumRoleDTO) {
        var newForumRole = new ForumRole {
            Name = addForumRoleDTO.Name,
            Threshold = addForumRoleDTO.Threshold
        };

        _context.ForumRoles.Add(newForumRole);
        await _context.SaveChangesAsync();

        return newForumRole;
    }

    public async Task<bool?> RemoveForumRoleAsync(int forumRoleId) {
        var forumRole = await _context.ForumRoles.FirstOrDefaultAsync(fr => fr.Id == forumRoleId);

        if (forumRole == null) {
            return false;
        }

        _context.ForumRoles.Remove(forumRole);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<object?> UpdateForumRoleAsync(int forumRoleId, ForumRoleDTO updateForumRoleDTO) {
        var forumRole = await _context.ForumRoles.FirstOrDefaultAsync(fr => fr.Id == forumRoleId);

        if (forumRole == null) {
            return false;
        }

        forumRole.Name = updateForumRoleDTO.Name ?? forumRole.Name;
        forumRole.Threshold = updateForumRoleDTO.Threshold ?? forumRole.Threshold;

        _context.ForumRoles.Update(forumRole);
        await _context.SaveChangesAsync();
        return forumRole;
    }
}
