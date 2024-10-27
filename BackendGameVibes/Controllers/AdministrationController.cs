using AutoMapper;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Requests;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace BackendGameVibes.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Require authorization admin")]
    public class AdministrationController : ControllerBase {
        private readonly UserManager<UserGameVibes> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public AdministrationController(ApplicationDbContext context,
            UserManager<UserGameVibes> userManager,
            IHostApplicationLifetime applicationLifetime,
            IMapper mapper,
            IAccountService accountService) {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _userManager = userManager;
            _applicationLifetime = applicationLifetime;
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

        [HttpDelete("review")]
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

    }
}