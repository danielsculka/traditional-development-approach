namespace ManualProg.Api.Endpoints.Auth.Requests;

public record RefreshTokenRequest
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}
