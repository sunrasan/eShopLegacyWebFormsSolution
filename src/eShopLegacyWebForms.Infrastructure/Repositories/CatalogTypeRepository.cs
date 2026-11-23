using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Repositories;
using eShopLegacyWebForms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for CatalogType.
/// </summary>
public class CatalogTypeRepository : ICatalogTypeRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CatalogTypeRepository> _logger;

    public CatalogTypeRepository(CatalogDbContext context, ILogger<CatalogTypeRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CatalogType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CatalogTypes
                .AsNoTracking()
                .OrderBy(t => t.Type)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all catalog types");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<CatalogType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CatalogTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting catalog type with id {Id}", id);
            throw;
        }
    }
}
