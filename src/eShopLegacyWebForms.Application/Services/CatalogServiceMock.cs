using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Application.Services;

/// <summary>
/// Mock implementation of ICatalogService for development/testing.
/// </summary>
public class CatalogServiceMock : ICatalogService
{
    private readonly List<CatalogItem> _catalogItems;
    private readonly ILogger<CatalogServiceMock> _logger;
    private int _nextId;

    public CatalogServiceMock(ILogger<CatalogServiceMock> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _catalogItems = new List<CatalogItem>(GetPreconfiguredCatalogItems());
        _nextId = _catalogItems.Max(i => i.Id) + 1;
        ComposeCatalogItems(_catalogItems);
    }

    public Task<(IEnumerable<CatalogItem> Items, long TotalCount)> GetCatalogItemsPaginatedAsync(
        int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting paginated catalog items");
        var items = _catalogItems
            .OrderBy(c => c.Id)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<(IEnumerable<CatalogItem>, long)>((items, _catalogItems.Count));
    }

    public Task<CatalogItem?> FindCatalogItemAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Finding catalog item with id {Id}", id);
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(item);
    }

    public Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting catalog types");
        return Task.FromResult(GetPreconfiguredCatalogTypes());
    }

    public Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting catalog brands");
        return Task.FromResult(GetPreconfiguredCatalogBrands());
    }

    public Task CreateCatalogItemAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Creating catalog item: {Name}", catalogItem.Name);
        catalogItem.Id = _nextId++;
        _catalogItems.Add(catalogItem);
        return Task.CompletedTask;
    }

    public Task UpdateCatalogItemAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Updating catalog item with id {Id}", catalogItem.Id);
        var originalItem = _catalogItems.FirstOrDefault(x => x.Id == catalogItem.Id);
        if (originalItem == null)
        {
            throw new CatalogItemNotFoundException(catalogItem.Id);
        }
        _catalogItems[_catalogItems.IndexOf(originalItem)] = catalogItem;
        return Task.CompletedTask;
    }

    public Task RemoveCatalogItemAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Removing catalog item with id {Id}", id);
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            throw new CatalogItemNotFoundException(id);
        }
        _catalogItems.Remove(item);
        return Task.CompletedTask;
    }

    private void ComposeCatalogItems(List<CatalogItem> items)
    {
        var catalogTypes = GetPreconfiguredCatalogTypes().ToList();
        var catalogBrands = GetPreconfiguredCatalogBrands().ToList();
        items.ForEach(i => i.CatalogBrand = catalogBrands.First(b => b.Id == i.CatalogBrandId));
        items.ForEach(i => i.CatalogType = catalogTypes.First(b => b.Id == i.CatalogTypeId));
    }

    private static List<CatalogItem> GetPreconfiguredCatalogItems()
    {
        return new List<CatalogItem>
        {
            new() { Id = 1, CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureFileName = "1.png" },
            new() { Id = 2, CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price = 8.50M, PictureFileName = "2.png" },
            new() { Id = 3, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureFileName = "3.png" },
            new() { Id = 4, CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureFileName = "4.png" },
            new() { Id = 5, CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureFileName = "5.png" },
            new() { Id = 6, CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureFileName = "6.png" },
            new() { Id = 7, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureFileName = "7.png" },
            new() { Id = 8, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureFileName = "8.png" },
            new() { Id = 9, CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureFileName = "9.png" },
            new() { Id = 10, CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureFileName = "10.png" },
            new() { Id = 11, CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureFileName = "11.png" },
            new() { Id = 12, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureFileName = "12.png" },
        };
    }

    private static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
    {
        return new List<CatalogBrand>
        {
            new() { Id = 1, Brand = "Azure" },
            new() { Id = 2, Brand = ".NET" },
            new() { Id = 3, Brand = "Visual Studio" },
            new() { Id = 4, Brand = "SQL Server" },
            new() { Id = 5, Brand = "Other" }
        };
    }

    private static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
    {
        return new List<CatalogType>
        {
            new() { Id = 1, Type = "Mug" },
            new() { Id = 2, Type = "T-Shirt" },
            new() { Id = 3, Type = "Sheet" },
            new() { Id = 4, Type = "USB Memory Stick" }
        };
    }
}
