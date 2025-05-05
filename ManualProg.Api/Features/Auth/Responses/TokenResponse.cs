using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Features.Auth.Responses;

public class TokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public Guid UserId { get; set; }
    public required string UserName { get; set; }
    public UserRole UserRole { get; set; }
}
