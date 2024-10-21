using BackendGameVibes.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService) {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles() {
            return Ok(await _roleService.GetAllRoles());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewRole([Required] string name) {
            IdentityResult result = await _roleService.CreateNewRole(name);
            if (result.Succeeded)
                return Ok(name);
            else
                return BadRequest(result);
        }
    }
}
