namespace BackendGameVibes.Models.Requests;

public class LoginDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
