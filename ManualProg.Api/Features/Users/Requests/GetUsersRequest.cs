namespace ManualProg.Api.Features.Users.Requests;

public record GetUsersRequest : IPagedRequest
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
