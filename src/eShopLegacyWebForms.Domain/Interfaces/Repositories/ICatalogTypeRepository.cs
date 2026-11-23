using eShopLegacyWebForms.Domain.Entities;

namespace eShopLegacyWebForms.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for CatalogType entities.
/// </summary>
public interface ICatalogTypeRepository
{
    /// <summary>
    /// Gets all catalog types.
    /// </summary>
    Task<IEnumerable<CatalogType>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a catalog type by its identifier.
    /// </summary>
    Task<CatalogType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
