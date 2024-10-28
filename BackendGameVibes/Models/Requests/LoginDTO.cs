using System.ComponentModel;

namespace BackendGameVibes.Models.Requests;

public class LoginDTO {
    [DefaultValue("test@test.com")]
    public required string Email { get; init; }
    [DefaultValue("Test123.")]
    public required string Password { get; init; }
}
