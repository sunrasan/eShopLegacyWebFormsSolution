using eShopLegacyWebForms.Domain.Entities;

namespace eShopLegacyWebForms.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for CatalogItem entities.
/// </summary>
public interface ICatalogItemRepository
{
    /// <summary>
    /// Gets all catalog items with pagination.
    /// </summary>
    Task<(IEnumerable<CatalogItem> Items, long TotalCount)> GetPaginatedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a catalog item by its identifier.
    /// </summary>
    Task<CatalogItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new catalog item.
    /// </summary>
    Task AddAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing catalog item.
    /// </summary>
    Task UpdateAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a catalog item.
    /// </summary>
    Task DeleteAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a catalog item exists.
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
