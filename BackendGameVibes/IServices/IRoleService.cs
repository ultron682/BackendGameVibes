using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.IServices {
    public interface IRoleService : IDisposable {
        Task<IdentityResult> CreateNewRole(string name);
        Task<IEnumerable<object>> GetAllRoles();
    }
}