using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Requests.Account;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/administration")]
    [SwaggerTag("Require authorization admin")]
    public class AdministrationController : ControllerBase {
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IRoleService _roleService;

        public AdministrationController(ApplicationDbContext context,
            UserManager<UserGameVibes> userManager,
            IHostApplicationLifetime applicationLifetime,
            IMapper mapper,
            IAccountService accountService,
            IRoleService roleService) {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _userManager = userManager;
            _applicationLifetime = applicationLifetime;
            _roleService = roleService;
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
            return Ok(await _context.Users.ToArrayAsync());
        }

        [HttpGet("user/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> GetUser(string id) {
            UserGameVibes? userGameVibes = await _userManager.FindByIdAsync(id);
            if (userGameVibes == null) {
                return NotFound();
            }
            return Ok(userGameVibes);
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
            //if (userDTO.ProfilePicture != null)
            //    userGameVibes.ProfilePicture = userDTO.ProfilePicture;
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
                .Select(r => new {
                    r.Id,
                    r.ReporterUserId,
                    ReporterUserName = r.ReporterUser!.UserName,
                    r.ReviewId,
                    r.Review!.Comment,
                    r.Reason
                })
                .ToArrayAsync());
        }

        [HttpGet("posts/reported")]
        [Authorize(Policy = "modOrAdmin")]
        public async Task<IActionResult> GetReportedPosts() {
            return Ok(await _context.ReportedPosts
                .Include(p => p.ReporterUser)
                .Include(p => p.ForumPost)
                .Select(p => new {
                    p.Id,
                    p.ReporterUserId,
                    ReporterUserName = p.ReporterUser!.UserName,
                    p.ForumPostId,
                    p.ForumPost!.Content,
                    p.Reason
                })
                .ToArrayAsync());
        }
    }
}