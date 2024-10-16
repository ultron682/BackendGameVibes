using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendGameVibes.Services {
    public class RoleService : IDisposable {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserGameVibes> _userManager;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<UserGameVibes> userManager) {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task InitRolesAndUsers() {
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

                //Mod         
                var user = new UserGameVibes();
                user.UserName = "mod";
                user.Email = "mod@mod.com";
                user.EmailConfirmed = true;
                string userPWD = "Mod123.";

                IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

                if (chkUser.Succeeded) {
                    var result1 = await _userManager.AddToRoleAsync(user, "mod");
                }
            }

            // Creating user role     
            x = await _roleManager.RoleExistsAsync("user");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "user";
                await _roleManager.CreateAsync(role);

                //Normal user      
                var newUser = new UserGameVibes();
                newUser.UserName = "test";
                newUser.Email = "test@test.com";
                newUser.EmailConfirmed = true;
                string userPWD = "Test123.";

                IdentityResult chkUser = await _userManager.CreateAsync(newUser, userPWD);

                if (chkUser.Succeeded) {
                    await _userManager.AddToRoleAsync(newUser, "user");
                }
            }

            // Creating guest role     
            x = await _roleManager.RoleExistsAsync("guest");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "guest";
                await _roleManager.CreateAsync(role);
            }
        }

        public async Task<IdentityResult> CreateNewRole(string name) {
            return await _roleManager.CreateAsync(new IdentityRole(name));
        }

        public void Dispose() {
            _roleManager?.Dispose();
        }
    }
}