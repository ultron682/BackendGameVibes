using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;

namespace BackendGameVibes.IServices.Forum {
    public interface IForumThreadService : IDisposable {
        Task<ForumThread> AddThreadAsync(NewForumThreadDTO newThread);
        Task<IEnumerable<object>> GetAllUserThreads(string userId);
        Task<IEnumerable<object>> GetForumRoles();
        Task<object?> GetForumThreadWithPosts(int id, string? userId = null);
        Task<object[]> GetLandingThreads();
        Task<object[]?> GetThreadsByPhrase(string phrase);
        Task<IEnumerable<object>> GetSections();
        Task<object[]> GetThreadsGroupBySectionsAsync();
        Task<object[]> GetThreadsInSectionAsync(int sectionId);
    }
}