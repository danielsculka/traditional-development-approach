using ManualProg.Api.Features;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Data.Extensions;

public static class PaginationExtensions
{
    public static async Task<PagedList<TResponse>> ToPagedArrayAsync<TRequest, TResponse>(this IQueryable<TResponse> query, TRequest request, CancellationToken cancellationToken = default) where TRequest : IPagedRequest
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
