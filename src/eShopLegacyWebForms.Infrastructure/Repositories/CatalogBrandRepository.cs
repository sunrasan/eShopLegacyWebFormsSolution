using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Repositories;
using eShopLegacyWebForms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for CatalogBrand.
/// </summary>
public class CatalogBrandRepository : ICatalogBrandRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CatalogBrandRepository> _logger;

    public CatalogBrandRepository(CatalogDbContext context, ILogger<CatalogBrandRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CatalogBrand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CatalogBrands
                .AsNoTracking()
                .OrderBy(b => b.Brand)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all catalog brands");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<CatalogBrand?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CatalogBrands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting catalog brand with id {Id}", id);
            throw;
        }
    }
}
