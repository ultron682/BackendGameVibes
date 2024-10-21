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

namespace BackendGameVibes.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdministrationController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public AdministrationController(ApplicationDbContext context, IMapper mapper, IAccountService accountService) {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
        }


        // dane logowania konta admina w DbInitializer.cs
        [HttpGet("user")]
        [Authorize("admin")]
        public async Task<IActionResult> GetAllUsers() {
            return Ok(await _context.Users.ToArrayAsync());
        }

        [HttpPost("user")]
        [Authorize("admin")]
        public async Task<IActionResult> AddUser(RegisterRequest newUserData) {
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("review")]
        [Authorize(Policy ="modOrAdmin")]
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