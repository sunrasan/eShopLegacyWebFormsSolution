using eShopLegacyWebForms.Domain.Entities;

namespace eShopLegacyWebForms.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for CatalogBrand entities.
/// </summary>
public interface ICatalogBrandRepository
{
    /// <summary>
    /// Gets all catalog brands.
    /// </summary>
    Task<IEnumerable<CatalogBrand>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a catalog brand by its identifier.
    /// </summary>
    Task<CatalogBrand?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
