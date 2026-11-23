using AutoMapper;
using eShopLegacyWebForms.Application.DTOs;
using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Exceptions;
using eShopLegacyWebForms.Domain.Interfaces.Repositories;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace eShopLegacyWebForms.Application.Services;

/// <summary>
/// Service implementation for catalog operations using EF Core repositories.
/// </summary>
public class CatalogService : ICatalogService
{
    private readonly ICatalogItemRepository _catalogItemRepository;
    private readonly ICatalogBrandRepository _catalogBrandRepository;
    private readonly ICatalogTypeRepository _catalogTypeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(
        ICatalogItemRepository catalogItemRepository,
        ICatalogBrandRepository catalogBrandRepository,
        ICatalogTypeRepository catalogTypeRepository,
        IMapper mapper,
        ILogger<CatalogService> logger)
    {
        _catalogItemRepository = catalogItemRepository ?? throw new ArgumentNullException(nameof(catalogItemRepository));
        _catalogBrandRepository = catalogBrandRepository ?? throw new ArgumentNullException(nameof(catalogBrandRepository));
        _catalogTypeRepository = catalogTypeRepository ?? throw new ArgumentNullException(nameof(catalogTypeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<CatalogItem> Items, long TotalCount)> GetCatalogItemsPaginatedAsync(
        int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting paginated catalog items: page {PageIndex}, size {PageSize}", pageIndex, pageSize);
            return await _catalogItemRepository.GetPaginatedAsync(pageIndex, pageSize, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated catalog items");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<CatalogItem?> FindCatalogItemAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Finding catalog item with id {Id}", id);
            return await _catalogItemRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding catalog item with id {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all catalog types");
            return await _catalogTypeRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting catalog types");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all catalog brands");
            return await _catalogBrandRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting catalog brands");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task CreateCatalogItemAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating catalog item: {Name}", catalogItem.Name);
            await _catalogItemRepository.AddAsync(catalogItem, cancellationToken);
            _logger.LogInformation("Catalog item created with id {Id}", catalogItem.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating catalog item: {Name}", catalogItem.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateCatalogItemAsync(CatalogItem catalogItem, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating catalog item with id {Id}", catalogItem.Id);
            var exists = await _catalogItemRepository.ExistsAsync(catalogItem.Id, cancellationToken);
            if (!exists)
            {
                throw new CatalogItemNotFoundException(catalogItem.Id);
            }
            await _catalogItemRepository.UpdateAsync(catalogItem, cancellationToken);
            _logger.LogInformation("Catalog item with id {Id} updated successfully", catalogItem.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item with id {Id}", catalogItem.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveCatalogItemAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Removing catalog item with id {Id}", id);
            var item = await _catalogItemRepository.GetByIdAsync(id, cancellationToken);
            if (item == null)
            {
                throw new CatalogItemNotFoundException(id);
            }
            await _catalogItemRepository.DeleteAsync(item, cancellationToken);
            _logger.LogInformation("Catalog item with id {Id} removed successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing catalog item with id {Id}", id);
            throw;
        }
    }
}
