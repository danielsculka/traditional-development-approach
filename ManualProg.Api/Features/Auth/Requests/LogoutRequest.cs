namespace ManualProg.Api.Features.Auth.Requests;

public record LogoutRequest
{
    public required string RefreshToken { get; set; }
}
