using BackendGameVibes.IServices;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BackendGameVibes.Services;

public class RoleService : IRoleService {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<UserGameVibes> _userManager;

    public RoleService(RoleManager<IdentityRole> roleManager, UserManager<UserGameVibes> userManager) {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateNewRole(string name) {
        return await _roleManager.CreateAsync(new IdentityRole(name));
    }

    public async Task<IEnumerable<object>> GetAllRoles() {
        return await _roleManager.Roles.ToListAsync();
    }

    public void Dispose() {
        _roleManager?.Dispose();
    }
}