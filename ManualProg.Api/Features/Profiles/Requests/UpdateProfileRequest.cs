namespace ManualProg.Api.Features.Profiles.Requests;

public record UpdateProfileRequest
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}
