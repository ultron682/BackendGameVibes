using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.IServices.Forum {
    public interface IForumPostService : IDisposable {
        Task<ForumPost> AddForumPost(ForumPostDTO forumPostDTO);
        Task<ReportedPost?> FinishReportPostAsync(int id, bool toRemovePost);
        Task<ActionResult<IEnumerable<ForumPost>>> GetAllPosts(int idThread);
        Task<IEnumerable<object>> GetAllUserPosts(string userId);
        Task<object?> GetPostByIdAsync(int id);
        Task<object[]?> GetPostsByPhrase(string phrase);
        Task<ReportedPost?> ReportPostAsync(string userId, ReportPostDTO reportPostDTO);
    }
}