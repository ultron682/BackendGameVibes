using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;

namespace BackendGameVibes.IServices.Forum {
    public interface IForumThreadService : IDisposable {
        Task<object> GetThreadsGroupBySectionsAsync(int pageNumber, int threadsInsectionSize);
        Task<object> GetThreadsInSectionAsync(int sectionId, int pageNumber, int pageSize);
        Task<object> GetThreadsByUserIdAsync(string userId, int pageNumber = 1, int threadsSize = 10);
        Task<IEnumerable<object>> GetAllForumRolesAsync();
        Task<object?> GetThreadWithPostsAsync(int threadId, string? userId, int pageNumber, int postsSize);
        Task<object[]> GetLandingThreadsAsync();
        Task<object?> GetThreadsByPhraseAsync(string phrase, int pageNumber = 1, int threadsSize = 10);
        Task<IEnumerable<object>> GetSectionsAsync();
        Task<ForumThread> AddThreadAsync(NewForumThreadDTO newThread);
        Task<IEnumerable<object>> AddSectionAsync(AddSectionDTO addSectionDTO);
        Task<IEnumerable<object>> RemoveSectionAsync(int idSection);
        Task<IEnumerable<object>> UpdateSectionAsync(int idSection, AddSectionDTO addSectionDTO);
    }
}