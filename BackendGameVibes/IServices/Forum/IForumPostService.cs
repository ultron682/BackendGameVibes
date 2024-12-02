using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;

namespace BackendGameVibes.IServices.Forum;

public interface IForumPostService : IDisposable {
    Task<object> GetPostsByThreadIdAsync(int idThread, string? userAccessToken, int pageNumber, int postsSize);
    Task<object> GetPostsByUserIdAsync(string userId, int pageNumber, int postsSize);
    Task<object?> GetPostByIdAsync(int postId);
    Task<object?> GetPostsByPhraseAsync(string phrase, string? userAccessToken, int pageNumber = 1, int postsSize = 10);
    Task<ForumPost?> AddForumPostAsync(ForumPostDTO forumPostDTO);
    Task<bool> DeletePostByIdAsync(int postId, string userId);
    Task<ReportedPost?> FinishReportPostAsync(int id, bool toRemovePost);
    Task<ReportedPost?> ReportPostAsync(string userId, ReportPostDTO reportPostDTO);
    Task<ForumPost?> UpdatePostByIdAsync(int postId, string userId, ForumPostUpdateDTO postUpdateDTO);
    Task<object?> InteractPostAsync(string userId, int postId, bool? isLike);
}