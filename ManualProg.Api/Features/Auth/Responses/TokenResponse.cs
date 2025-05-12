using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Features.Auth.Responses;

public class TokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required Guid UserId { get; set; }
    public Guid? ProfileId { get; set; }
    public required string Username { get; set; }
    public required UserRole UserRole { get; set; }
}
