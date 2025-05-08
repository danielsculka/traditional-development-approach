using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Features.Users.Requests;

public record UpdateUserRequest
{
    public required UserRole Role { get; set; }
}
