namespace BackendGameVibes.Models.DTOs.Responses {
    public class LoginResponse {
        public string? AccessToken { get; set; }
        public string[]? UserRoles { get; set; }
    }
}
