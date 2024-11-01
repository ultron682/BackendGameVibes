using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;

namespace BackendGameVibes.IServices {
    public interface IForumThreadService : IDisposable {
        Task<ForumThread> AddThreadAsync(NewForumThreadDTO newThread);
        Task<IEnumerable<ForumThread>> GetAllThreads();
        Task<IEnumerable<object>> GetAllUserThreads(string userId);
        Task<IEnumerable<object>> GetForumRoles();
        Task<ForumThread?> GetForumThread(int id);
        Task<IEnumerable<object>> GetLandingThreads();
        Task<IEnumerable<object>> GetSections();
    }
}