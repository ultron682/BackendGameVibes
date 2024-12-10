using BackendGameVibes.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BackendGameVibes.Services;

public class RoleService : IRoleService {
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(RoleManager<IdentityRole> roleManager) {
        _roleManager = roleManager;
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