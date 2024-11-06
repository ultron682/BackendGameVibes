using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.IServices.Forum {
    public interface IForumPostService : IDisposable {
        Task<ForumPost> AddForumPost(ForumPostDTO forumPostDTO);
        Task<bool> DeletePostByIdAsync(int postId, string userId);
        Task<ReportedPost?> FinishReportPostAsync(int id, bool toRemovePost);
        Task<IEnumerable<object>> GetAllForumPostsByForumThreadId(int idThread);
        Task<IEnumerable<object>> GetAllUserPosts(string userId);
        Task<object?> GetPostByIdAsync(int id);
        Task<object[]?> GetPostsByPhrase(string phrase);
        Task<ReportedPost?> ReportPostAsync(string userId, ReportPostDTO reportPostDTO);
        Task<ForumPost?> UpdatePostByIdAsync(int postId, string userId, ForumPostUpdateDTO postUpdateDTO);
        Task<object?> InteractPostAsync(string userId, int postId, bool? isLike);

    }
}