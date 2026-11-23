namespace eShopLegacyWebForms.Application.DTOs;

/// <summary>
/// Generic paginated items view model.
/// </summary>
public class PaginatedItemsDto<TDto> where TDto : class
{
    public int ActualPage { get; set; }
    public int ItemsPerPage { get; set; }
    public long TotalItems { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<TDto> Data { get; set; } = Enumerable.Empty<TDto>();

    public PaginatedItemsDto(int pageIndex, int pageSize, long count, IEnumerable<TDto> data)
    {
        ActualPage = pageIndex;
        ItemsPerPage = pageSize;
        TotalItems = count;
        TotalPages = (int)Math.Ceiling((decimal)count / pageSize);
        Data = data;
    }

    public PaginatedItemsDto()
    {
    }
}
