namespace ManualProg.Api.Features.Profiles.Responses;

public record ProfilePostResponse
{
    public required Guid Id { get; set; }
    public required IEnumerable<Guid> ImageIds { get; set; }
    public required string Description { get; set; }
    public required int CommentCount { get; set; }
    public required int LikeCount { get; set; }
    public required DateTime Created { get; set; }
}
