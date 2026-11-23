using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Web.Pages.Catalog;

/// <summary>
/// Page model for the catalog item details page.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<DetailsModel> _logger;

    public CatalogItemViewModel? Item { get; set; }

    public DetailsModel(ICatalogService catalogService, ILogger<DetailsModel> logger)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            _logger.LogInformation("Loading catalog item details for id {Id}", id);

            var item = await _catalogService.FindCatalogItemAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Catalog item with id {Id} not found", id);
                return NotFound();
            }

            Item = new CatalogItemViewModel
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
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading catalog item details for id {Id}", id);
            return StatusCode(500);
        }
    }
}
