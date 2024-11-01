using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;

namespace BackendGameVibes.Controllers {
    [Route("api/forum")]
    [ApiController]
    public class ForumController : ControllerBase {
        private readonly IForumPostService _postService;
        private readonly IForumThreadService _threadService;

        public ForumController(IForumPostService postService, IForumThreadService threadService) {
            _postService = postService;
            _threadService = threadService;
        }


        [HttpGet("thread")]
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
        [Authorize]
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

        [HttpPost("post/report")]
        [Authorize]
        public async Task<ActionResult<object>> ReportPost(ReportPostDTO reportPostDTO) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            var reportedForumPost = await _postService.ReportPostAsync(userId, reportPostDTO);

            if (reportedForumPost != null)
                return Ok(new {
                    reportedForumPost.Id,
                    reportedForumPost.ForumPostId,
                    reportedForumPost.ReporterUserId,
                    reportedForumPost.Reason,
                });
            else {
                return BadRequest("ErrorOnReportPost");
            }
        }

        [HttpGet("{userId}/threads")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserThreads(string userId) {
            return Ok(await _threadService.GetAllUserThreads(userId));
        }


        [HttpGet("{userId}/posts")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserPosts(string userId) {
            return Ok(await _postService.GetAllUserPosts(userId));
        }
    }
}
