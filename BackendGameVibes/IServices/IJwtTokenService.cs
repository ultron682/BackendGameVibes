namespace BackendGameVibes.IServices {
    public interface IJwtTokenService {
        string GenerateToken(string email, string username, string userId, string role);
        (string? email, string? username, string? userId, string? role) GetTokenClaims(string token);
    }
}
