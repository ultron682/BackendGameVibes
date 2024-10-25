using BackendGameVibes.IServices;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService) {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize("admin")]
        public async Task<IActionResult> GetAllRoles() {
            return Ok(await _roleService.GetAllRoles());
        }

        [HttpPost]
        [Authorize("admin")]
        public async Task<IActionResult> CreateNewRole([Required] string name) {
            IdentityResult result = await _roleService.CreateNewRole(name);
            if (result.Succeeded)
                return Ok(name);
            else
                return BadRequest(result);
        }
    }
}
