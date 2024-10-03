using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserGameVibes> _userManager;

        public RoleController(RoleManager<IdentityRole> roleMgr, UserManager<UserGameVibes> userManager) {
            _roleManager = roleMgr;
            _userManager = userManager;
            //CreateRolesAndUsers();
        }

        [HttpPost("init")]
        public async Task CreateRolesAndUsers() {
            // Creating admin role    
            bool x = await _roleManager.RoleExistsAsync("admin");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "admin";
                await _roleManager.CreateAsync(role);

                //Admin           
                var user = new UserGameVibes();
                user.UserName = "admin";
                user.Email = "admin@admin.com";
                user.EmailConfirmed = true;
                string userPWD = "Admin123.";

                IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded) {
                    var result1 = await _userManager.AddToRoleAsync(user, "admin");
                }
            }

            // Creating mod role     
            x = await _roleManager.RoleExistsAsync("mod");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "mod";
                await _roleManager.CreateAsync(role);
            }

            // Creating user role     
            x = await _roleManager.RoleExistsAsync("user");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "user";
                await _roleManager.CreateAsync(role);
            }

            // Creating guest role     
            x = await _roleManager.RoleExistsAsync("guest");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "guest";
                await _roleManager.CreateAsync(role);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateNewRole([Required] string name) {
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
                return Ok(name);
            else
                return BadRequest(result);
        }
    }
}
