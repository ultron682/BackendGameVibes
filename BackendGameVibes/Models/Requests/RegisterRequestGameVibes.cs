namespace BackendGameVibes.Models.Requests;

public class RegisterRequestGameVibes
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string UserName { get; init; }
}

