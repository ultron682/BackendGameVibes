using BackendGameVibes.Data;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ForumController : ControllerBase {
        private readonly PostService _postService;
        private readonly ThreadService _threadService;

        public ForumController(PostService postService, ThreadService threadService) {
            _postService = postService;
            _threadService = threadService;
        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Thread>>> GetThreads() {
        //    return await _threadService.GetAllThreads();
        //    ;
        //}

        //[HttpPost]
        //public async Task<ActionResult<Thread>> CreateThread(ForumThread thread, ForumPost forumPost) {
        //    thread.CreatedAt = DateTime.UtcNow;
        //    _context.Threads.Add(thread);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetThreadById), new { id = thread.Id }, thread);
        //}
    }
}
