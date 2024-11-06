using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;

namespace BackendGameVibes.IServices.Forum {
    public interface IForumThreadService : IDisposable {
        Task<ForumThread> AddThreadAsync(NewForumThreadDTO newThread);
        Task<IEnumerable<object>> GetAllUserThreads(string userId);
        Task<IEnumerable<object>> GetForumRoles();
        Task<object?> GetForumThreadWithPosts(int threadId, string? userId = null);
        Task<object[]> GetLandingThreads();
        Task<object[]?> GetThreadsByPhrase(string phrase);
        Task<IEnumerable<object>> GetSections();
        Task<object> GetThreadsGroupBySectionsAsync(int pageNumber = 1, int pageSize = 10);
        Task<object[]> GetThreadsInSectionAsync(int sectionId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<object>> RemoveSection(int idSection);
        Task<IEnumerable<object>> UpdateSection(int idSection, AddSectionDTO addSectionDTO);
        Task<IEnumerable<object>> AddSection(AddSectionDTO addSectionDTO);
    }
}