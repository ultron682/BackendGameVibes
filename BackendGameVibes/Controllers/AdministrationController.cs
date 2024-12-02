using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Forum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Data;
using AutoMapper;
using BackendGameVibes.Services;


namespace BackendGameVibes.Controllers;

[ApiController]
[Route("api/administration")]
public class AdministrationController : ControllerBase {
    private readonly UserManager<UserGameVibes> _userManager;
    private readonly IAccountService _accountService;
    private readonly IRoleService _roleService;
    private readonly IForumPostService _forumPostService;
    private readonly IForumThreadService _forumThreadService;
    private readonly IReviewService _reviewService;
    private readonly IGameService _gameService;
    private readonly IForumRoleService _forumRoleService;
    private readonly IAdministrationService _administrationService;

    public AdministrationController(
        UserManager<UserGameVibes> userManager,
        IHostApplicationLifetime applicationLifetime,
        IMapper mapper,
        IAccountService accountService,
        IRoleService roleService,
        IForumPostService forumPostService,
        IForumThreadService forumThreadService,
        IReviewService reviewService,
        IGameService gameService,
        IForumRoleService forumRoleService,
        IAdministrationService administrationService) {
        _accountService = accountService;
        _userManager = userManager;
        _roleService = roleService;
        _forumPostService = forumPostService;
        _forumThreadService = forumThreadService;
        _reviewService = reviewService;
        _gameService = gameService;
        _forumRoleService = forumRoleService;
        _administrationService = administrationService;
    }

    [HttpGet("users")]
    [Authorize("admin")]
    public async Task<IActionResult> GetAllUsersWithRoles() {
        var usersWithRoles = await _administrationService.GetAllUsersWithRolesAsync();
        return Ok(usersWithRoles);
    }

    [HttpGet("users/{userId}")]
    [Authorize("admin")]
    public async Task<IActionResult> GetUser(string userId) {
        var accountInfo = await _accountService.GetBasicAccountInfoAsync(userId);

        if (accountInfo == null) {
            return NotFound();
        }

        return Ok(accountInfo);
    }

    [HttpPost("users")]
    [Authorize("admin")]
    public async Task<IActionResult> AddUser(RegisterDTO newUserData) {
        if (newUserData == null) {
            return BadRequest();
        }

        IdentityResult identityResult = await _accountService.RegisterUserAsync(newUserData);
        if (identityResult.Succeeded)
            return Ok(identityResult.Succeeded);
        else
            return BadRequest("User exist");
    }

    [HttpDelete("users")]
    [Authorize("admin")]
    public async Task<IActionResult> DeleteUser(string userId) {
        UserGameVibes? userGameVibes = await _userManager.FindByIdAsync(userId);
        if (userGameVibes == null) {
            return NotFound();
        }
        else {
            IdentityResult identityResult = await _userManager.DeleteAsync(userGameVibes);
            if (identityResult.Succeeded)
                return Ok();
            else
                return BadRequest();
        }
    }

    [HttpPatch("users")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> UpdateUser(UserGameVibesDTO userDTO) {
        try {
            var (user, roles) = await _administrationService.UpdateUserAsync(userDTO);
            return Ok(new { user, roles });
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("user/change-profile-picture")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> UpdateUserProfilePicture(string userId, IFormFile profilePicture) {
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not authenticated");

        if (profilePicture == null || profilePicture.Length == 0)
            return BadRequest("InvalidProfilePicture");

        using (var ms = new MemoryStream()) {
            await profilePicture.CopyToAsync(ms);
            var imageData = ms.ToArray();

            var result = await _accountService.UpdateProfilePictureAsync(userId, imageData);
            return result ? Ok("ProfilePictureUpdated") : BadRequest("FailedToUpdateProfilePicture");
        }
    }

    [HttpPost("user/set-forum-role")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> SetForumRoleForUser([Required] string userId, [Required] int forumRoleId) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) {
            return NotFound("NoUser");
        }

        user.ForumRoleId = forumRoleId;
        await _userManager.UpdateAsync(user);

        return Ok("ForumRoleUpdated");
    }

    [HttpDelete("reviews/{id}")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> DeleteReview(int id) {
        var success = await _administrationService.DeleteReviewAsync(id);
        if (!success) {
            return NotFound();
        }

        return Ok("removed");
    }

    [HttpDelete("post/{id}")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> DeletePost(int id) {
        var success = await _administrationService.DeletePostAsync(id);
        if (!success) {
            return NotFound();
        }

        return Ok("removed");
    }

    [HttpGet("role")]
    [Authorize("admin")]
    [SwaggerOperation("")]
    public async Task<IActionResult> GetAllRoles() {
        return Ok(await _roleService.GetAllRoles());
    }

    [HttpPost("role")]
    [Authorize("admin")]
    [SwaggerOperation("")]
    public async Task<IActionResult> CreateNewRole([Required] string name) {
        IdentityResult result = await _roleService.CreateNewRole(name);
        if (result.Succeeded)
            return Ok(name);
        else
            return BadRequest(result);
    }

    [HttpGet("reviews/reported")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> GetReportedReviews() {
        var reportedReviews = await _administrationService.GetReportedReviewsAsync();
        return Ok(reportedReviews);
    }

    [HttpGet("posts/reported")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> GetReportedPosts() {
        var reportedPosts = await _administrationService.GetReportedPostsAsync();
        return Ok(reportedPosts);
    }

    [HttpPost("post/finish")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> FinishReportedPost([Required] int id, [Required] bool toRemovePost) {
        var reportedPost = await _forumPostService.FinishReportPostAsync(id, toRemovePost);
        if (reportedPost == null) {
            return NotFound();
        }
        return Ok(reportedPost);
    }

    [HttpPost("review/finish")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> FinishReportedReview([Required] int id, [Required] bool toRemoveReview) {
        var reportedReview = await _reviewService.FinishReportReviewAsync(id, toRemoveReview);
        if (reportedReview == null) {
            return NotFound();
        }
        return Ok(reportedReview);
    }

    [HttpPost("send-email-to/{userId}")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> SendEmailToUser(string userId, EmailSendDTO emailSendDTO) {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) {
            return NotFound();
        }

        bool isSuccess = await _accountService.SendGeneralEmailToUserAsync(user, emailSendDTO.Subject!, emailSendDTO.Message!);
        return Ok(isSuccess);
    }

    [HttpPost("thread-section-names")]
    [Authorize("admin")]
    public async Task<ActionResult<IEnumerable<object>>> AddSection(AddSectionDTO addSectionDTO) {
        return Ok(await _forumThreadService.AddSectionAsync(addSectionDTO));
    }

    [HttpPatch("thread-section-name/{sectionId:int}")]
    [Authorize("admin")]
    public async Task<ActionResult<IEnumerable<object>>> UpdateSection(int sectionId, AddSectionDTO sectionDTO) {
        return Ok(await _forumThreadService.UpdateSectionAsync(sectionId, sectionDTO));
    }

    [HttpDelete("thread-section-name/{sectionId:int}")]
    [Authorize("admin")]
    public async Task<ActionResult<IEnumerable<object>>> RemoveSection(int sectionId) {
        return Ok(await _forumThreadService.RemoveSectionAsync(sectionId));
    }

    [HttpPatch("games")]
    [Authorize(Roles = "admin,mod")]
    [SwaggerOperation("Require authorization admin or mod. Dajesz tylko te wlasciwosci ktore chcesz zmienic. nadpisuje się cała kolekcja wiec np. imagesUrls musisz podac wszystkie wraz z nowymi")]
    public async Task<ActionResult> UpdateGame([FromBody] GameUpdateDTO gameUpdateDTO, int gameId) {
        var game = await _gameService.UpdateGameAsync(gameId, gameUpdateDTO);
        if (game == null)
            return NotFound("Game not found");

        return Ok(game);
    }

    [HttpDelete("games")]
    [Authorize(Roles = "admin,mod")]
    [SwaggerOperation("Require authorization admin or mod")]
    public async Task<ActionResult> RemoveGame(int gameId) {
        var isRemoved = await _gameService.RemoveGameAsync(gameId);
        if (isRemoved == null)
            return NotFound("Game not found or error");

        return Ok(isRemoved);
    }

    [HttpPost("forum-roles")]
    [Authorize("admin")]
    public async Task<ActionResult<IEnumerable<object>>> AddForumRole(ForumRoleDTO addForumRoleDTO) {
        return Ok(await _forumRoleService.AddForumRoleAsync(addForumRoleDTO));
    }

    [HttpPatch("forum-roles/{forumRoleId:int}")]
    [Authorize("admin")]
    public async Task<ActionResult<IEnumerable<object>>> UpdateForumRole(int forumRoleId, ForumRoleDTO addForumRoleDTO) {
        return Ok(await _forumRoleService.UpdateForumRoleAsync(forumRoleId, addForumRoleDTO));
    }

    [HttpDelete("forum-roles/{forumRoleId:int}")]
    [Authorize("admin")]
    public async Task<ActionResult<IEnumerable<object>>> RemoveForumRole(int forumRoleId) {
        return Ok(await _forumRoleService.RemoveForumRoleAsync(forumRoleId));
    }

    [HttpPost("games/genres")]
    [Authorize("admin")]
    public async Task<IActionResult> AddGameGenre(ValueModel genreModel) {
        await _administrationService.AddGameGenreAsync(genreModel);
        return Ok();
    }

    [HttpPatch("games/genres/{genreId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> UpdateGameGenre(int genreId, ValueModel genreModel) {
        var success = await _administrationService.UpdateGameGenreAsync(genreId, genreModel);
        if (!success) {
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("games/genres/{genreId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> RemoveGameGenre(int genreId) {
        var success = await _administrationService.RemoveGameGenreAsync(genreId);
        if (!success) {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost("games/platforms")]
    [Authorize("admin")]
    public async Task<IActionResult> AddGamePlatform(ValueModel platformModel) {
        await _administrationService.AddGamePlatformAsync(platformModel);
        return Ok();
    }

    [HttpPatch("games/platforms/{platformId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> UpdateGamePlatform(int platformId, ValueModel platformModel) {
        var success = await _administrationService.UpdateGamePlatformAsync(platformId, platformModel);
        if (!success) {
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("games/platforms/{platformId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> RemoveGamePlatform(int platformId) {
        var success = await _administrationService.RemoveGamePlatformAsync(platformId);
        if (!success) {
            return NotFound();
        }

        return Ok();
    }
}