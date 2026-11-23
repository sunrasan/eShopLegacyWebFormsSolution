using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Web.Pages.Catalog;

/// <summary>
/// Page model for creating a new catalog item.
/// </summary>
public class CreateModel : PageModel
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public CatalogItemCreateViewModel Input { get; set; } = new();

    public CreateModel(ICatalogService catalogService, ILogger<CreateModel> logger)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Loading create catalog item page");
        await PopulateDropdownsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return Page();
        }

        try
        {
            _logger.LogInformation("Creating new catalog item: {Name}", Input.Name);

            var catalogItem = new CatalogItem
            {
                Name = Input.Name,
                Description = Input.Description,
                Price = Input.Price,
                PictureFileName = string.IsNullOrWhiteSpace(Input.PictureFileName) ? CatalogItem.DefaultPictureName : Input.PictureFileName,
                CatalogBrandId = Input.CatalogBrandId,
                CatalogTypeId = Input.CatalogTypeId,
                AvailableStock = Input.AvailableStock,
                RestockThreshold = Input.RestockThreshold,
                MaxStockThreshold = Input.MaxStockThreshold,
                OnReorder = Input.OnReorder
            };

            await _catalogService.CreateCatalogItemAsync(catalogItem);
            _logger.LogInformation("Catalog item created successfully with id {Id}", catalogItem.Id);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating catalog item: {Name}", Input.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the item. Please try again.");
            await PopulateDropdownsAsync();
            return Page();
        }
    }

    private async Task PopulateDropdownsAsync()
    {
        var brands = await _catalogService.GetCatalogBrandsAsync();
        var types = await _catalogService.GetCatalogTypesAsync();

        Input.Brands = brands.Select(b => new SelectListItemViewModel
        {
            Value = b.Id.ToString(),
            Text = b.Brand
        }).ToList();

        Input.Types = types.Select(t => new SelectListItemViewModel
        {
            Value = t.Id.ToString(),
            Text = t.Type
        }).ToList();
    }
}
