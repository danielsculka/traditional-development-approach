namespace ManualProg.Api.Features.Posts.Requests;

public record GetPostsRequest : IPagedRequest
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
