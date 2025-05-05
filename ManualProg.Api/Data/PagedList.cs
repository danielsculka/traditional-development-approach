namespace ManualProg.Api.Data;

public record PagedList<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalItems)
{
    public bool HasNextPage => Page * PageSize < TotalItems;
    public bool HasPreviousPage => Page > 1;
}
