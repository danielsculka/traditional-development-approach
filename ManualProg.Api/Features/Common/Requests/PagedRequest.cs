using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Common.Requests;

public interface IPagedRequest
{
    public const int MaxPageSize = 100;
    int? Page { get; }
    int? PageSize { get; }
}

public record PagedList<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalItems)
{
    public bool HasNextPage => Page * PageSize < TotalItems;
    public bool HasPreviousPage => Page > 1;
}

public static class PaginationDatabaseExtensions
{
    public static async Task<PagedList<TResponse>> ToPagedListAsync<TRequest, TResponse>(this IQueryable<TResponse> query, TRequest request, CancellationToken cancellationToken = default) where TRequest : IPagedRequest
    {
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, IPagedRequest.MaxPageSize);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return new PagedList<TResponse>(items, page, pageSize, totalItems);
    }
}
