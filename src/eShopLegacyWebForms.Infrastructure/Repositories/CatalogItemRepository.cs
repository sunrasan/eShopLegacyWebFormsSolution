using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Repositories;
using eShopLegacyWebForms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for CatalogItem.
/// </summary>
public class CatalogItemRepository : ICatalogItemRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<CatalogItemRepository> _logger;

    public CatalogItemRepository(CatalogDbContext context, ILogger<CatalogItemRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<CatalogItem> Items, long TotalCount)> GetPaginatedAsync(
        int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var totalCount = await _context.CatalogItems.LongCountAsync(cancellationToken);
            var items = await _context.CatalogItems
                .AsNoTracking()
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated catalog items");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<CatalogItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CatalogItems
                .AsNoTracking()
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .FirstOrDefaultAsync(ci => ci.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting catalog item with id {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task AddAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        try
        {
            // Auto-generate ID using max + 1 strategy (replaces HiLo generator)
            var maxId = await _context.CatalogItems.AnyAsync(cancellationToken)
                ? await _context.CatalogItems.MaxAsync(i => i.Id, cancellationToken)
                : 0;
            catalogItem.Id = maxId + 1;

            await _context.CatalogItems.AddAsync(catalogItem, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding catalog item: {Name}", catalogItem.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.CatalogItems.Update(catalogItem);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item with id {Id}", catalogItem.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.CatalogItems.Remove(catalogItem);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting catalog item with id {Id}", catalogItem.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.CatalogItems.AnyAsync(ci => ci.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of catalog item with id {Id}", id);
            throw;
        }
    }
}
