using BackendGameVibes.Data;
using BackendGameVibes.Models;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdministrationController : ControllerBase {
        private readonly ApplicationDbContext _context;


        public AdministrationController(ApplicationDbContext context) {
            _context = context;
        }


        // endpoint przetestowany i dziala tylko dla admina. dane logowania konta admina w RoleController
        [HttpGet()]
        public async Task<IActionResult> GetAllUsers() {
            return Ok(await _context.Users.ToArrayAsync());
        }
    }
}