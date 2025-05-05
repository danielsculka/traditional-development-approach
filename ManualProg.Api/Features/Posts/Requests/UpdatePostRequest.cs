namespace ManualProg.Api.Features.Posts.Requests;

public record UpdatePostRequest
{
    public required string Description { get; set; }
}
