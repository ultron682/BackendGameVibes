using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BackendGameVibes.IServices.Forum;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;


namespace BackendGameVibes.Controllers;

[Route("api/forum")]
[ApiController]
public class ForumController : ControllerBase {
    private readonly IForumPostService _postService;
    private readonly IForumThreadService _threadService;
    private readonly IForumRoleService _forumRoleService;

    public ForumController(IForumPostService postService, IForumThreadService threadService, IForumRoleService forumRoleService) {
        _postService = postService;
        _threadService = threadService;
        _forumRoleService = forumRoleService;
    }

    [SwaggerOperation("wątki pogrupowane przez sekcje")]
    [HttpGet("threads/sections")]
    public async Task<IActionResult> GetThreadsGroupBySections(int pageNumber = 1, int threadsInSectionSize = 10) {
        return Ok(await _threadService.GetThreadsGroupBySectionsAsync(pageNumber, threadsInSectionSize));
    }

    [SwaggerOperation("zwraca wątek z postami. jesli podamy userId to jeszcze będzie info o interakcji danego uzytkownika z postem")]
    [HttpGet("threads/{id:int}")]
    public async Task<IActionResult> GetThreadWithPosts(int id, string? userAccessToken = null, int pageNumber = 1, int postsSize = 10) {
        object? thread = await _threadService.GetThreadWithPostsAsync(id, userAccessToken, pageNumber, postsSize);

        if (thread == null) {
            return NotFound();
        }

        return Ok(thread);
    }

    [HttpGet("threads/sections/{sectionId:int}")]
    public async Task<IActionResult> GetThreadsInSections(int sectionId, int pageNumber = 1, int threadsInSectionSize = 10) {
        var forumThreads = await _threadService.GetThreadsInSectionAsync(sectionId, pageNumber, threadsInSectionSize);

        if (forumThreads == null) {
            return NotFound();
        }

        return Ok(forumThreads);
    }

    [HttpPost("threads")]
    [Authorize]
    [SwaggerResponse(200, "created")]
    [SwaggerResponse(400, "NoUser or NoSection found")]
    public async Task<IActionResult> CreateThread(NewForumThreadDTO forumThreadDTO) {
        if (ModelState.IsValid) {
            ForumThread? forumThread = await _threadService.AddThreadAsync(forumThreadDTO);

            if (forumThread == null) {
                return BadRequest("NoUser or NoSection found");
            }

            return Ok(forumThread);
        }
        else {
            return BadRequest("Wrong ForumThreadDTO");
        }
    }


    [HttpGet("{userId}/threads")]
    [SwaggerResponse(404, "no threads belong to user or no user")]
    [SwaggerResponse(200, "found")]
    public async Task<ActionResult<IEnumerable<object>>> GetUserThreads(string userId, int pageNumber = 1, int threadsSize = 10) {
        var threads = await _threadService.GetThreadsByUserIdAsync(userId, pageNumber, threadsSize);

        if (threads == null)
            return NotFound();

        return Ok(threads);
    }


    [HttpGet("{userId}/posts")]
    [SwaggerResponse(404, "no posts belong to user or no user")]
    [SwaggerResponse(200, "found")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllUserPosts(string userId, int pageNumber = 1, int postsSize = 10) {
        var posts = await _postService.GetPostsByUserIdAsync(userId, pageNumber, postsSize);

        if (posts == null)
            return NotFound();

        return Ok(posts);
    }


    [HttpGet("posts/{id:int}")]
    public async Task<ActionResult<object>> GetPostById(int id) {
        return Ok(await _postService.GetPostByIdAsync(id));
    }

    [HttpPost("posts")]
    [Authorize]
    [SwaggerResponse(200, "created")]
    [SwaggerResponse(400, "Wrong ForumThreadDTO")]
    [SwaggerResponse(404, "no thread or no user")]
    public async Task<ActionResult<Thread>> CreatePost(ForumPostDTO forumPostDTO) {
        if (ModelState.IsValid) {
            ForumPost? forumPost = await _postService.AddForumPostAsync(forumPostDTO);

            if (forumPost == null)
                return NotFound();

            return Ok(forumPost);
        }
        else {
            return BadRequest("Wrong ForumThreadDTO");
        }
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


    [HttpPatch("posts/{postId:int}")]
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

    [HttpDelete("posts/{postId:int}")]
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

    [HttpGet("search-phrase")]
    public async Task<ActionResult> SearchForumByPhrase([Required, MinLength(3)] string phrase, int pageNumber = 1, int resultSize = 10) {
        phrase = phrase.ToLower();

        var result = new {
            threads = await _threadService.GetThreadsByPhraseAsync(phrase, pageNumber, resultSize),
            posts = await _postService.GetPostsByPhraseAsync(phrase, pageNumber, resultSize)
        };

        return Ok(result);
    }


    [HttpGet("thread-section-names")]
    public async Task<ActionResult<IEnumerable<object>>> GetSections() {
        return Ok(await _threadService.GetForumSectionsAsync());
    }

    [HttpGet("forum-roles")]
    public async Task<ActionResult<IEnumerable<object>>> GetForumRoles() {
        return Ok(await _forumRoleService.GetForumRolesAsync());
    }

    [HttpPost("interact/{postId:int}")]
    [Authorize]
    [SwaggerOperation("isLike = true,false,  null - noInteraction(remove Like/Dislike) zwracana tablica postInteractions pokazuje tylko aktualnego uzytkownika")]
    public async Task<ActionResult> InteractPost(int postId, bool? isLike) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized("User not authenticated, claim not found");

        object? post = await _postService.InteractPostAsync(userId, postId, isLike);
        if (post == null) {
            return NotFound();
        }

        return Ok(post);
    }
}
