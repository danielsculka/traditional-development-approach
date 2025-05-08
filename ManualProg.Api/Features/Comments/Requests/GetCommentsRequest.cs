namespace ManualProg.Api.Features.Comments.Requests;

public record GetCommentsRequest : IPagedRequest
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
