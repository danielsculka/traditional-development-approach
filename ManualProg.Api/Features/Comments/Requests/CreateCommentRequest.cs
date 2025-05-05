namespace ManualProg.Api.Features.Comments.Requests;

public record CreateCommentRequest
{
    public required Guid PostId { get; set; }
    public required string Content { get; set; }
    public Guid? ReplyToCommentId { get; set; }
}
