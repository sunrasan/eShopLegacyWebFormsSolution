using eShopLegacyWebForms.Domain.Entities;

namespace eShopLegacyWebForms.Domain.Interfaces.Services;

/// <summary>
/// Service interface for catalog operations.
/// </summary>
public interface ICatalogService
{
    /// <summary>
    /// Gets paginated catalog items.
    /// </summary>
    Task<(IEnumerable<CatalogItem> Items, long TotalCount)> GetCatalogItemsPaginatedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a catalog item by its identifier.
    /// </summary>
    Task<CatalogItem?> FindCatalogItemAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all catalog types.
    /// </summary>
    Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all catalog brands.
    /// </summary>
    Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new catalog item.
    /// </summary>
    Task CreateCatalogItemAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing catalog item.
    /// </summary>
    Task UpdateCatalogItemAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a catalog item.
    /// </summary>
    Task RemoveCatalogItemAsync(int id, CancellationToken cancellationToken = default);
}
