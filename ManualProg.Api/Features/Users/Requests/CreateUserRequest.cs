using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Features.Users.Requests;

public record CreateUserRequest
{
    public required string Username { get; set; }
    public required UserRole Role { get; set; }
    public required string Password { get; set; }
}
