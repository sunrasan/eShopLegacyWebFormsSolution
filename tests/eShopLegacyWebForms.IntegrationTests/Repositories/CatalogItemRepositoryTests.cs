using Xunit;
using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Infrastructure.Data;
using eShopLegacyWebForms.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace eShopLegacyWebForms.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for CatalogItemRepository using in-memory database.
/// </summary>
public class CatalogItemRepositoryTests : IDisposable
{
    private readonly CatalogDbContext _context;
    private readonly CatalogItemRepository _sut;

    public CatalogItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CatalogDbContext(options);
        var mockLogger = new Mock<ILogger<CatalogItemRepository>>();
        _sut = new CatalogItemRepository(_context, mockLogger.Object);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var brands = new List<CatalogBrand>
        {
            new() { Id = 1, Brand = ".NET" },
            new() { Id = 2, Brand = "Azure" }
        };
        var types = new List<CatalogType>
        {
            new() { Id = 1, Type = "Mug" },
            new() { Id = 2, Type = "T-Shirt" }
        };
        var items = new List<CatalogItem>
        {
            new() { Id = 1, Name = "Item 1", Price = 10.0m, PictureFileName = "1.png", CatalogBrandId = 1, CatalogTypeId = 1 },
            new() { Id = 2, Name = "Item 2", Price = 20.0m, PictureFileName = "2.png", CatalogBrandId = 2, CatalogTypeId = 2 }
        };

        _context.CatalogBrands.AddRange(brands);
        _context.CatalogTypes.AddRange(types);
        _context.CatalogItems.AddRange(items);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnCorrectItems()
    {
        // Act
        var (items, totalCount) = await _sut.GetPaginatedAsync(0, 10);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ShouldReturnItem()
    {
        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Item 1");
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddItemToDatabase()
    {
        // Arrange
        var newItem = new CatalogItem
        {
            Name = "New Item",
            Price = 15.0m,
            PictureFileName = "new.png",
            CatalogBrandId = 1,
            CatalogTypeId = 1
        };

        // Act
        await _sut.AddAsync(newItem);

        // Assert
        var savedItem = await _context.CatalogItems.FindAsync(newItem.Id);
        savedItem.Should().NotBeNull();
        savedItem!.Name.Should().Be("New Item");
    }

    [Fact]
    public async Task ExistsAsync_WhenItemExists_ShouldReturnTrue()
    {
        // Act
        var result = await _sut.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenItemDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.ExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
