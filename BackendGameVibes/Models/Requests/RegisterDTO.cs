namespace BackendGameVibes.Models.Requests;

public class RegisterDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string UserName { get; init; }
}

