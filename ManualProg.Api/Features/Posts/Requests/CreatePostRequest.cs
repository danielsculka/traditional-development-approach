namespace ManualProg.Api.Features.Posts.Requests;

public record CreatePostRequest
{
    public required bool IsPublic { get; set; }
    public required string Description { get; set; }
    public required IEnumerable<IFormFile> Images { get; set; }
}
