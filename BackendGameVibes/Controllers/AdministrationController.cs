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


namespace BackendGameVibes.Controllers {
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


        public AdministrationController(ApplicationDbContext context,
            UserManager<UserGameVibes> userManager,
            IHostApplicationLifetime applicationLifetime,
            IMapper mapper,
            IAccountService accountService,
            IRoleService roleService,
            IForumPostService forumPostService,
            IForumThreadService forumThreadService,
            IReviewService reviewService) {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _userManager = userManager;
            _applicationLifetime = applicationLifetime;
            _roleService = roleService;
            _forumPostService = forumPostService;
            _forumThreadService = forumThreadService;
            _reviewService = reviewService;
        }

        [SwaggerOperation("Usuwa baze i tworzy plik bazy od początku. Zamyka aplikację którą trzeba ręcznie ponownie uruchomic")]
        [HttpGet("DROP_DATABASE")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetDatabase() {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
            _applicationLifetime.StopApplication();
            return Ok("Database deleted");
        }


        // dane logowania konta admina w DbInitializer.cs
        [HttpGet("user")]
        [Authorize("admin")]
        public async Task<IActionResult> GetAllUsers() {
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
                    u.Description,
                    roles = _userManager.GetRolesAsync(u).Result.ToArray()
                }).ToArrayAsync();

            return Ok(users);
        }

        [HttpGet("user/{userId}")]
        [Authorize("admin")]
        public async Task<IActionResult> GetUser(string userId) {
            UserGameVibes? userGameVibes = await _userManager.FindByIdAsync(userId);

            if (userGameVibes == null) {
                return NotFound();
            }

            var accountInfo = _accountService.GetBasicAccountInfoAsync(userGameVibes.Id);

            return Ok(accountInfo);
        }

        [HttpPost("user")]
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

        [HttpDelete("user")]
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

        [HttpDelete("review/{id}")]
        [Authorize(Policy = "modOrAdmin")]
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
        public async Task<IActionResult> DeletePost(int id) {
            var post = await _context.ForumPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) {
                return NotFound();
            }

            _context.ForumPosts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok("removed");
        }

        [HttpPatch("user")]
        [Authorize(Policy = "modOrAdmin")]
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
        public async Task<IActionResult> FinishReportedPost([Required] int id, [Required] bool toRemovePost) {
            var reportedPost = await _forumPostService.FinishReportPostAsync(id, toRemovePost);
            if (reportedPost == null) {
                return NotFound();
            }
            return Ok(reportedPost);
        }

        [HttpPost("review/finish")]
        [Authorize(Policy = "modOrAdmin")]
        public async Task<IActionResult> FinishReportedReview([Required] int id, [Required] bool toRemoveReview) {
            var reportedReview = await _reviewService.FinishReportReviewAsync(id, toRemoveReview);
            if (reportedReview == null) {
                return NotFound();
            }
            return Ok(reportedReview);
        }

        [HttpPost("send-email-to/{userId}")]
        [Authorize(Policy = "modOrAdmin")]
        public async Task<IActionResult> SendEmailToUser(string userId, EmailSendDTO emailSendDTO) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return NotFound();
            }

            bool isSuccess = await _accountService.SendGeneralEmailToUserAsync(user, emailSendDTO.Subject!, emailSendDTO.Message!);
            return Ok(isSuccess);
        }

        [HttpPost("thread-section-names")]
        public async Task<ActionResult<IEnumerable<object>>> AddSection(AddSectionDTO addSectionDTO) {
            return Ok(await _forumThreadService.AddSection(addSectionDTO));
        }

        [HttpPatch("thread-section-name/{sectionId:int}")]
        public async Task<ActionResult<IEnumerable<object>>> UpdateSection(int sectionId, AddSectionDTO sectionDTO) {
            return Ok(await _forumThreadService.UpdateSection(sectionId, sectionDTO));
        }

        [HttpDelete("thread-section-name/{sectionId:int}")]
        public async Task<ActionResult<IEnumerable<object>>> RemoveSection(int sectionId) {
            return Ok(await _forumThreadService.RemoveSection(sectionId));
        }
    }
}