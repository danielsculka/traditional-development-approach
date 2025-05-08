using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Features.Users.Responses;

public record UserResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required UserRole Role { get; set; }
    public ProfileData? Profile { get; set; }
    public required DateTime Created { get; set; }

    public record ProfileData
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
