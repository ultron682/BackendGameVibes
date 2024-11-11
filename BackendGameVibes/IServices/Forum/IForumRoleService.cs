
using BackendGameVibes.Models.DTOs;

namespace BackendGameVibes.IServices.Forum;

public interface IForumRoleService {
    Task<IEnumerable<object>> GetForumRolesAsync();
    Task<object?> AddForumRoleAsync(ForumRoleDTO addForumRoleDTO);
    Task<bool?> RemoveForumRoleAsync(int forumRoleId);
    Task<object?> UpdateForumRoleAsync(int forumRoleId, ForumRoleDTO updateForumRoleDTO);
}