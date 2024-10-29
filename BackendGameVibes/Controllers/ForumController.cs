using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ForumController : ControllerBase {
        private readonly IForumPostService _postService;
        private readonly IForumThreadService _threadService;

        public ForumController(IForumPostService postService, IForumThreadService threadService) {
            _postService = postService;
            _threadService = threadService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thread>>> GetThreads() {
            return Ok(await _threadService.GetAllThreads());
        }

        [HttpGet("thread/:id")]
        public async Task<ActionResult<IEnumerable<ForumPost>>> GetPosts(int id) {
            return Ok(await _threadService.GetForumThread(id));
        }

        [HttpPost("thread")]
        public async Task<ActionResult<Thread>> CreateThread(NewForumThreadDTO forumThreadDTO) {
            if (ModelState.IsValid) {
                ForumThread forumThread = await _threadService.AddThreadAsync(forumThreadDTO);
                return Ok(forumThread);
                //return CreatedAtAction(nameof(GetThreadById), new { id = thread.Id }, thread);
            }
            else {
                return BadRequest("Wrong ForumThreadDTO");
            }
        }

        [HttpPost("post")]
        public async Task<ActionResult<Thread>> CreatePost(ForumPostDTO forumPostDTO) {
            if (ModelState.IsValid) {
                ForumPost forumPost = await _postService.AddForumPost(forumPostDTO);
                return Ok(forumPost);
                //return CreatedAtAction(nameof(GetThreadById), new { id = thread.Id }, thread);
            }
            else {
                return BadRequest("Wrong ForumThreadDTO");
            }
        }

        [HttpGet("section")]
        public async Task<ActionResult<IEnumerable<object>>> GetSections() {
            return Ok(await _threadService.GetSections());
        }

        [HttpGet("forum-roles")]
        public async Task<ActionResult<IEnumerable<object>>> GetForumRoles() {
            return Ok(await _threadService.GetForumRoles());
        }
    }
}
