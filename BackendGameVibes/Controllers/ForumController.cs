﻿using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;
using BackendGameVibes.IServices.Forum;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Hosting;

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


        [HttpGet("threads")]
        public async Task<ActionResult<IEnumerable<Thread>>> GetThreads() {
            return Ok(await _threadService.GetAllThreads());
        }

        [HttpGet("threads/:id")]
        public async Task<ActionResult<IEnumerable<ForumPost>>> GetPosts(int id) {
            return Ok(await _threadService.GetForumThread(id));
        }

        [HttpGet("threads/sections")]
        public async Task<ActionResult<IEnumerable<ForumPost>>> GetThreadsGroupBySections() {
            return Ok(await _threadService.GetThreadsGroupBySectionsAsync());
        }

        [HttpGet("threads/sections/{sectionId:int}")]
        public async Task<ActionResult<IEnumerable<ForumPost>>> GetThreadsInSections(int sectionId) {
            return Ok(await _threadService.GetThreadsInSectionAsync(sectionId));
        }

        [HttpPost("threads")]
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

        [HttpPost("posts")]
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

        [HttpGet("posts/:id")]
        public async Task<ActionResult<object>> GetPostById(int id) {
            return Ok(await _postService.GetPostByIdAsync(id));
        }

        [HttpPost("posts/report")]
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

        [HttpGet("search-phrase")]
        public async Task<ActionResult> SearchForumByPhrase([Required] string phrase) {
            phrase = phrase.ToLower();

            var result = new {
                threads = await _threadService.GetThreadsByPhrase(phrase),
                posts = await _postService.GetPostsByPhrase(phrase)
            };

            return Ok(result);
        }

        [HttpPatch("{postId}")]
        [Authorize]
        [SwaggerResponse(404, "no post or post doesnt belong to user")]
        [SwaggerResponse(200, "updated")]
        public async Task<IActionResult> UpdatePost(int postId, ForumPostUpdateDTO postUpdateDTO) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            ForumPost? post = await _postService.UpdatePostByIdAsync(postId, userId, postUpdateDTO);
            if (post == null) {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpDelete("{postId}")]
        [Authorize]
        [SwaggerResponse(404, "no post or post doesnt belong to user")]
        [SwaggerResponse(200, "deleted")]
        public async Task<IActionResult> DeletePost(int postId) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            bool isSuccess = await _postService.DeletePostByIdAsync(postId, userId);
            if (isSuccess) {
                return Ok();
            }

            return NotFound();
        }
    }
}
