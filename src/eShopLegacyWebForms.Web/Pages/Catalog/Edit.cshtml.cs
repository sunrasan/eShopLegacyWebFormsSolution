using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Web.Pages.Catalog;

/// <summary>
/// Page model for editing an existing catalog item.
/// </summary>
public class EditModel : PageModel
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public CatalogItemEditViewModel Input { get; set; } = new();

    public EditModel(ICatalogService catalogService, ILogger<EditModel> logger)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            _logger.LogInformation("Loading edit page for catalog item id {Id}", id);

            var item = await _catalogService.FindCatalogItemAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Catalog item with id {Id} not found", id);
                return NotFound();
            }

            // Manually map entity to view model
            Input = new CatalogItemEditViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                PictureFileName = item.PictureFileName,
                CatalogTypeId = item.CatalogTypeId,
                CatalogBrandId = item.CatalogBrandId,
                AvailableStock = item.AvailableStock,
                RestockThreshold = item.RestockThreshold,
                MaxStockThreshold = item.MaxStockThreshold,
                OnReorder = item.OnReorder
            };

            await PopulateDropdownsAsync(item.CatalogBrandId, item.CatalogTypeId);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit page for catalog item id {Id}", id);
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(Input.CatalogBrandId, Input.CatalogTypeId);
            return Page();
        }

        try
        {
            _logger.LogInformation("Saving catalog item with id {Id}", Input.Id);

            var catalogItem = new CatalogItem
            {
                Id = Input.Id,
                Name = Input.Name,
                Description = Input.Description,
                Price = Input.Price,
                PictureFileName = Input.PictureFileName,
                CatalogBrandId = Input.CatalogBrandId,
                CatalogTypeId = Input.CatalogTypeId,
                AvailableStock = Input.AvailableStock,
                RestockThreshold = Input.RestockThreshold,
                MaxStockThreshold = Input.MaxStockThreshold,
                OnReorder = Input.OnReorder
            };

            await _catalogService.UpdateCatalogItemAsync(catalogItem);
            _logger.LogInformation("Catalog item with id {Id} updated successfully", Input.Id);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving catalog item with id {Id}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while saving the item. Please try again.");
            await PopulateDropdownsAsync(Input.CatalogBrandId, Input.CatalogTypeId);
            return Page();
        }
    }

    private async Task PopulateDropdownsAsync(int selectedBrandId = 0, int selectedTypeId = 0)
    {
        var brands = await _catalogService.GetCatalogBrandsAsync();
        var types = await _catalogService.GetCatalogTypesAsync();

        Input.Brands = brands.Select(b => new SelectListItemViewModel
        {
            Value = b.Id.ToString(),
            Text = b.Brand,
            Selected = b.Id == selectedBrandId
        }).ToList();

        Input.Types = types.Select(t => new SelectListItemViewModel
        {
            Value = t.Id.ToString(),
            Text = t.Type,
            Selected = t.Id == selectedTypeId
        }).ToList();
    }
}
