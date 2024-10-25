using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;

namespace BackendGameVibes.IServices {
    public interface IForumThreadService : IDisposable {
        Task<ForumThread> AddThreadAsync(NewForumThreadDTO newThread);
        Task<IEnumerable<ForumThread>> GetAllThreads();
        Task<ForumThread?> GetForumThread(int id);
        Task<IEnumerable<object>> GetLandingThreads();
    }
}