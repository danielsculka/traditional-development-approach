namespace ManualProg.Api.Features.Posts.Responses;

public record PostResponse
{
    public required Guid Id { get; set; }
    public required IEnumerable<Guid> ImageIds { get; set; }
    public required string Description { get; set; }
    public required int CommentCount { get; set; }
    public required int LikeCount { get; set; }
    public required ProfileData Profile { get; set; }
    public required DateTime Created { get; set; }

    public record ProfileData
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required bool HasImage { get; set; }
    }
}
