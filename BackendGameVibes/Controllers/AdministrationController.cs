using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Forum;
using System.Data;
using BackendGameVibes.Services.Forum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;


namespace BackendGameVibes.Controllers;

[ApiController]
[Route("api/administration")]
public class AdministrationController : ControllerBase {
    private readonly UserManager<UserGameVibes> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IRoleService _roleService;
    private readonly IForumPostService _forumPostService;
    private readonly IForumThreadService _forumThreadService;
    private readonly IReviewService _reviewService;
    private readonly IGameService _gameService;
    private readonly IForumRoleService _forumRoleService;

    public AdministrationController(ApplicationDbContext context,
        UserManager<UserGameVibes> userManager,
        IHostApplicationLifetime applicationLifetime,
        IMapper mapper,
        IAccountService accountService,
        IRoleService roleService,
        IForumPostService forumPostService,
        IForumThreadService forumThreadService,
        IReviewService reviewService,
        IGameService gameService,
        IForumRoleService forumRoleService) {
        _context = context;
        _mapper = mapper;
        _accountService = accountService;
        _userManager = userManager;
        _applicationLifetime = applicationLifetime;
        _roleService = roleService;
        _forumPostService = forumPostService;
        _forumThreadService = forumThreadService;
        _reviewService = reviewService;
        _gameService = gameService;
        _forumRoleService = forumRoleService;
    }

    // dane logowania konta admina w DbInitializer.cs
    [HttpGet("users")]
    [Authorize("admin")]
    public async Task<IActionResult> GetAllUsersWithRoles() {
        //var userRoles = await _userManager.GetRolesAsync(_userManager.FindByIdAsync());

        var users = await _context.Users
            .Select(u => new {
                u.Id,
                u.Email,
                u.EmailConfirmed,
                u.UserName,
                ForumRoleName = u.ForumRole!.Name,
                u.ExperiencePoints,
                u.PhoneNumber,
                u.PhoneNumberConfirmed,
                u.AccessFailedCount,
                u.Description
            }).ToArrayAsync();


        var usersWithRoles = new List<object>();

        foreach (var user in users) {
            var userGameVibes = await _userManager.FindByIdAsync(user!.Id)!;
            var roles = await _userManager.GetRolesAsync(userGameVibes!);

            usersWithRoles.Add(new {
                user.Id,
                user.Email,
                user.EmailConfirmed,
                user.UserName,
                user.ForumRoleName,
                user.ExperiencePoints,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.AccessFailedCount,
                user.Description,
                roles = roles.ToArray()
            });
        }

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
        UserGameVibes? userGameVibes = await _userManager.FindByIdAsync(userDTO.Id);
        if (userGameVibes == null) {
            return NotFound();
        }

        IList<string> userRoles = await _userManager.GetRolesAsync(userGameVibes);

        if (userDTO.UserName != null)
            userGameVibes.UserName = userDTO.UserName;
        if (userDTO.Description != null)
            userGameVibes.Description = userDTO.Description;
        if (userDTO.ExperiencePoints != null)
            userGameVibes.ExperiencePoints = userDTO.ExperiencePoints;
        if (userDTO.ForumRoleId != null)
            userGameVibes.ForumRoleId = userDTO.ForumRoleId;
        if (userDTO.Email != null) {
            var emailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(userGameVibes, userDTO.Email);

            var changeEmailResult = await _userManager.ChangeEmailAsync(userGameVibes, userDTO.Email, emailChangeToken);

            if (!changeEmailResult.Succeeded) {
                return BadRequest(changeEmailResult.Errors.ToArray());
            }
        }
        if (userDTO.Password != null) {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userGameVibes);

            var resetPasswordResult = await _userManager.ResetPasswordAsync(userGameVibes, resetToken, userDTO.Password);

            if (!resetPasswordResult.Succeeded) {
                return BadRequest(resetPasswordResult.Errors.ToArray());
            }
        }
        if (string.IsNullOrEmpty(userDTO.PhoneNumber) == false) {
            await _userManager.SetPhoneNumberAsync(userGameVibes, userDTO.PhoneNumber);
        }
        if (userDTO.PhoneNumberConfirmed != null) {
            userGameVibes.PhoneNumberConfirmed = userDTO.PhoneNumberConfirmed ?? false;
        }

        if (string.IsNullOrEmpty(userDTO.RoleName) == false) {
            foreach (string role in userRoles) {
                await _userManager.RemoveFromRoleAsync(userGameVibes, role);
            }

            await _userManager.AddToRoleAsync(userGameVibes, userDTO.RoleName!);
        }
        if (userDTO.LockoutEnabled != null) {
            await _userManager.SetLockoutEnabledAsync(userGameVibes, (userDTO.LockoutEnabled != null && userDTO.LockoutEnabled == true));
        }

        if (userDTO.LockoutEnd != null) {
            await _userManager.SetLockoutEndDateAsync(userGameVibes, userDTO.LockoutEnd);
        }

        if (userDTO.AccessFailedCount != null) {
            userGameVibes.AccessFailedCount = (int)userDTO.AccessFailedCount;
        }

        await _userManager.UpdateAsync(userGameVibes);

        var currentRoles = await _userManager.GetRolesAsync(userGameVibes);

        return Ok(new {
            userGameVibes,
            roles = currentRoles.ToArray()
        });
    }


    [HttpPost("user/change-profile-picture")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> UpdateUserProfilePicture(string userId, IFormFile profilePicture) {
        if (userId.IsNullOrEmpty())
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
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (review == null) {
            return NotFound();
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return Ok("removed");
    }

    [HttpDelete("post/{id}")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> DeletePost(int id) {
        var post = await _context.ForumPosts.FirstOrDefaultAsync(p => p.Id == id);
        if (post == null) {
            return NotFound();
        }

        _context.ForumPosts.Remove(post);
        await _context.SaveChangesAsync();
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
        return Ok(await _context.ReportedReviews
            .Include(r => r.ReporterUser)
            .Include(r => r.Review)
            .Where(r => r.IsFinished == false)
            .Select(r => new {
                r.Id,
                r.ReporterUserId,
                ReporterUserName = r.ReporterUser!.UserName,
                r.ReviewId,
                r.Review!.Comment,
                r.Reason,
                r.IsFinished
            })
            .ToArrayAsync());
    }

    [HttpGet("posts/reported")]
    [Authorize(Policy = "modOrAdmin")]
    [SwaggerOperation("modOrAdmin")]
    public async Task<IActionResult> GetReportedPosts() {
        return Ok(await _context.ReportedForumPosts
            .Include(p => p.ReporterUser)
            .Include(p => p.ForumPost)
            .Where(r => r.IsFinished == false)
            .Select(p => new {
                p.Id,
                p.ReporterUserId,
                ReporterUserName = p.ReporterUser!.UserName,
                p.ForumPostId,
                p.ForumPost!.Content,
                p.Reason,
                p.IsFinished
            })
            .ToArrayAsync());
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
        var game = await _gameService.UpdateGame(gameId, gameUpdateDTO);
        if (game == null)
            return NotFound("Game not found");

        return Ok(game);
    }

    [HttpDelete("games")]
    [Authorize(Roles = "admin,mod")]
    [SwaggerOperation("Require authorization admin or mod")]
    public async Task<ActionResult> RemoveGame(int gameId) {
        var isRemoved = await _gameService.RemoveGame(gameId);
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
        _context.Genres.Add(new Models.Games.Genre { Name = genreModel.Value });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch("games/genres/{genreId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> UpdateGameGenre(int genreId, ValueModel genreModel) {
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
        if (genre == null) {
            return NotFound();
        }

        genre.Name = genreModel.Value;
        _context.Genres.Update(genre);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("games/genres/{genreId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> RemoveGameGenre(int genreId) {
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
        if (genre == null) {
            return NotFound();
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("games/platforms")]
    [Authorize("admin")]
    public async Task<IActionResult> AddGamePlatform(ValueModel platformModel) {
        _context.Platforms.Add(new Models.Games.Platform { Name = platformModel.Value });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch("games/platforms/{platformId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> UpdateGamePlatform(int platformId, ValueModel platformModel) {
        var platform = await _context.Platforms.FirstOrDefaultAsync(g => g.Id == platformId);
        if (platform == null) {
            return NotFound();
        }

        platform.Name = platformModel.Value;
        _context.Platforms.Update(platform);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("games/platforms/{platformId:int}")]
    [Authorize("admin")]
    public async Task<IActionResult> RemoveGamePlatform(int platformId) {
        var platform = await _context.Platforms.FirstOrDefaultAsync(g => g.Id == platformId);
        if (platform == null) {
            return NotFound();
        }

        _context.Platforms.Remove(platform);
        await _context.SaveChangesAsync();
        return Ok();
    }
}