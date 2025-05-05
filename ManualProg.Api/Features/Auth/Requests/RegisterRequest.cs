namespace ManualProg.Api.Features.Auth.Requests;

public record RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required ProfileData Profile { get; set; }

    public record ProfileData
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
