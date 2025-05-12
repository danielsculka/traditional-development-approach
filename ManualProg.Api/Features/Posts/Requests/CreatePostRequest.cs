namespace ManualProg.Api.Features.Posts.Requests;

public record CreatePostRequest
{
    public required bool IsPublic { get; set; }
    public int Price { get; set; } = 0;
    public required string Description { get; set; }
    public required IEnumerable<IFormFile> Images { get; set; }
}
