using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Web.Pages.Catalog;

/// <summary>
/// Page model for the catalog index page.
/// </summary>
public class IndexModel : PageModel
{
    private const int DefaultPageSize = 10;
    private readonly ICatalogService _catalogService;
    private readonly ILogger<IndexModel> _logger;

    public CatalogIndexViewModel ViewModel { get; set; } = new();

    public IndexModel(ICatalogService catalogService, ILogger<IndexModel> logger)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGetAsync(int pageIndex = 0, int pageSize = DefaultPageSize)
    {
        try
        {
            _logger.LogInformation("Loading catalog index page {PageIndex}", pageIndex);

            var (items, totalCount) = await _catalogService.GetCatalogItemsPaginatedAsync(pageIndex, pageSize);

            ViewModel = new CatalogIndexViewModel
            {
                ActualPage = pageIndex,
                ItemsPerPage = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize),
                Items = items.Select(item => new CatalogItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    PictureFileName = item.PictureFileName,
                    PictureUri = item.PictureUri,
                    CatalogTypeId = item.CatalogTypeId,
                    CatalogTypeName = item.CatalogType?.Type ?? string.Empty,
                    CatalogBrandId = item.CatalogBrandId,
                    CatalogBrandName = item.CatalogBrand?.Brand ?? string.Empty,
                    AvailableStock = item.AvailableStock,
                    RestockThreshold = item.RestockThreshold,
                    MaxStockThreshold = item.MaxStockThreshold,
                    OnReorder = item.OnReorder
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading catalog index page");
            ViewModel = new CatalogIndexViewModel();
        }
    }
}
