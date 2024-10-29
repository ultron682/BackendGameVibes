using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.Requests.Forum;
using BackendGameVibes.Models.Requests.Reported;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.IServices {
    public interface IForumPostService : IDisposable {
        Task<ForumPost> AddForumPost(ForumPostDTO forumPostDTO);
        Task<ActionResult<IEnumerable<ForumPost>>> GetAllPosts(int idThread);
        Task<ReportedPost?> ReportPostAsync(string userId, ReportPostDTO reportPostDTO);
    }
}