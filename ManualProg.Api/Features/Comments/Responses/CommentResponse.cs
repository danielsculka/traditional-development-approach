namespace ManualProg.Api.Features.Comments.Responses;

public record CommentResponse
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public required int RepliesCount { get; set; }
    public required int LikeCount { get; set; }
    public required bool HasLike { get; set; }
    public required ProfileData Profile { get; set; }
    public required DateTime Created { get; set; }

    public record ProfileData
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required bool HasImage { get; set; }
    }
}
