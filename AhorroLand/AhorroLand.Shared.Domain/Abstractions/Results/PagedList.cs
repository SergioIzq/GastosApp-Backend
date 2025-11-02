namespace AhorroLand.Shared.Domain.Results;

/// <summary>
/// Contiene el resultado de una consulta paginada.
/// </summary>
/// <typeparam name="T">El tipo de los elementos en la lista.</typeparam>
public record PagedList<T>
{
    public PagedList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IList<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
}