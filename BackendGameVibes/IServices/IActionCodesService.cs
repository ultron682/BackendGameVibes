using BackendGameVibes.Models;

namespace BackendGameVibes.IServices {
    public interface IActionCodesService {
        Task<(ActionCode, bool)> GenerateUniqueActionCode(string userId);
        Task<bool> RemoveActionCode(string actionCode);
    }
}
