namespace ManualProg.Api.Features.Profiles.Responses;

public record ProfileResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool HasImage { get; set; }
    public required int PostCount { get; set; }
}
