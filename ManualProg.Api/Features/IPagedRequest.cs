namespace ManualProg.Api.Features;

public interface IPagedRequest
{
    public const int MaxPageSize = 100;
    int? Page { get; }
    int? PageSize { get; }
}

