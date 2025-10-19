namespace FitApi.Core.Domain.Common;

public class PaginationResponse<T>(
    IReadOnlyList<T> data,
    int pageIndex,
    int pageSize,
    int totalPages,
    long totalCount
)
{
    public IReadOnlyList<T> Data { get; private init; } = data;
    public int PageIndex { get; private init; } = pageIndex;
    public int PageSize { get; private init; } = pageSize;
    public int TotalPages { get; private init; } = totalPages;
    public long TotalCount { get; private init; } = totalCount;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}