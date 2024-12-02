using BackendGameVibes.Models.DTOs.Forum;

namespace BackendGameVibes.IServices.Forum {
    public interface IForumThreadService : IDisposable {
        Task<object> GetThreadsGroupBySectionsAsync(int pageNumber, int threadsInsectionSize);
        Task<object> GetThreadsInSectionAsync(int sectionId, int pageNumber, int pageSize);
        Task<object> GetThreadsByUserIdAsync(string userId, int pageNumber = 1, int threadsSize = 10);
        Task<object?> GetThreadWithPostsAsync(int threadId, string? userAccessToken, int pageNumber, int postsSize);
        Task<object[]> GetThreadsLandingAsync();
        Task<object?> GetThreadsByPhraseAsync(string phrase, int pageNumber = 1, int threadsSize = 10);
        Task<IEnumerable<object>> GetForumSectionsAsync();
        Task<object?> AddThreadAsync(string userId, NewForumThreadDTO newThread);
        Task<IEnumerable<object>> AddSectionAsync(AddSectionDTO addSectionDTO);
        Task<IEnumerable<object>> RemoveSectionAsync(int idSection);
        Task<IEnumerable<object>> UpdateSectionAsync(int idSection, AddSectionDTO addSectionDTO);
    }
}