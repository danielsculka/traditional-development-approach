namespace ManualProg.Api.Features.Posts.Requests;

public record CreatePostRequest
{
    public required string Description { get; set; }
}
