using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.Services {
    public interface IForumPostService : IDisposable {
        Task<ForumPost> AddForumPost(ForumPostDTO forumPostDTO);
        Task<ActionResult<IEnumerable<ForumPost>>> GetAllPosts(int idThread);
    }
}