using Xunit;
using eShopLegacyWebForms.Application.Services;
using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Exceptions;
using eShopLegacyWebForms.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using eShopLegacyWebForms.Application.Mappings;

namespace eShopLegacyWebForms.UnitTests.Services;

/// <summary>
/// Unit tests for CatalogService.
/// </summary>
public class CatalogServiceTests
{
    private readonly Mock<ICatalogItemRepository> _mockItemRepository;
    private readonly Mock<ICatalogBrandRepository> _mockBrandRepository;
    private readonly Mock<ICatalogTypeRepository> _mockTypeRepository;
    private readonly Mock<ILogger<CatalogService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly CatalogService _sut;

    public CatalogServiceTests()
    {
        _mockItemRepository = new Mock<ICatalogItemRepository>();
        _mockBrandRepository = new Mock<ICatalogBrandRepository>();
        _mockTypeRepository = new Mock<ICatalogTypeRepository>();
        _mockLogger = new Mock<ILogger<CatalogService>>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _sut = new CatalogService(
            _mockItemRepository.Object,
            _mockBrandRepository.Object,
            _mockTypeRepository.Object,
            _mapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetCatalogItemsPaginatedAsync_ShouldReturnPaginatedItems()
    {
        // Arrange
        var items = new List<CatalogItem>
        {
            new() { Id = 1, Name = "Item 1", Price = 10.0m, PictureFileName = "1.png" },
            new() { Id = 2, Name = "Item 2", Price = 20.0m, PictureFileName = "2.png" }
        };
        _mockItemRepository.Setup(r => r.GetPaginatedAsync(0, 10, default))
            .ReturnsAsync((items, 2L));

        // Act
        var (resultItems, totalCount) = await _sut.GetCatalogItemsPaginatedAsync(0, 10);

        // Assert
        resultItems.Should().HaveCount(2);
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task FindCatalogItemAsync_WhenItemExists_ShouldReturnItem()
    {
        // Arrange
        var item = new CatalogItem { Id = 1, Name = "Test Item", Price = 10.0m, PictureFileName = "1.png" };
        _mockItemRepository.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(item);

        // Act
        var result = await _sut.FindCatalogItemAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Item");
    }

    [Fact]
    public async Task FindCatalogItemAsync_WhenItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _mockItemRepository.Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((CatalogItem?)null);

        // Act
        var result = await _sut.FindCatalogItemAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCatalogItemAsync_ShouldCallRepository()
    {
        // Arrange
        var item = new CatalogItem { Name = "New Item", Price = 15.0m, PictureFileName = "new.png" };
        _mockItemRepository.Setup(r => r.AddAsync(item, default))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.CreateCatalogItemAsync(item);

        // Assert
        _mockItemRepository.Verify(r => r.AddAsync(item, default), Times.Once);
    }

    [Fact]
    public async Task UpdateCatalogItemAsync_WhenItemExists_ShouldCallRepository()
    {
        // Arrange
        var item = new CatalogItem { Id = 1, Name = "Updated Item", Price = 25.0m, PictureFileName = "1.png" };
        _mockItemRepository.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _mockItemRepository.Setup(r => r.UpdateAsync(item, default)).Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateCatalogItemAsync(item);

        // Assert
        _mockItemRepository.Verify(r => r.UpdateAsync(item, default), Times.Once);
    }

    [Fact]
    public async Task UpdateCatalogItemAsync_WhenItemDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var item = new CatalogItem { Id = 999, Name = "Non-existent", Price = 10.0m, PictureFileName = "x.png" };
        _mockItemRepository.Setup(r => r.ExistsAsync(999, default)).ReturnsAsync(false);

        // Act & Assert
        await _sut.Invoking(s => s.UpdateCatalogItemAsync(item))
            .Should().ThrowAsync<CatalogItemNotFoundException>();
    }

    [Fact]
    public async Task RemoveCatalogItemAsync_WhenItemExists_ShouldCallRepository()
    {
        // Arrange
        var item = new CatalogItem { Id = 1, Name = "Item to Delete", Price = 10.0m, PictureFileName = "1.png" };
        _mockItemRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(item);
        _mockItemRepository.Setup(r => r.DeleteAsync(item, default)).Returns(Task.CompletedTask);

        // Act
        await _sut.RemoveCatalogItemAsync(1);

        // Assert
        _mockItemRepository.Verify(r => r.DeleteAsync(item, default), Times.Once);
    }

    [Fact]
    public async Task RemoveCatalogItemAsync_WhenItemDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockItemRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((CatalogItem?)null);

        // Act & Assert
        await _sut.Invoking(s => s.RemoveCatalogItemAsync(999))
            .Should().ThrowAsync<CatalogItemNotFoundException>();
    }

    [Fact]
    public async Task GetCatalogBrandsAsync_ShouldReturnAllBrands()
    {
        // Arrange
        var brands = new List<CatalogBrand>
        {
            new() { Id = 1, Brand = "Azure" },
            new() { Id = 2, Brand = ".NET" }
        };
        _mockBrandRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(brands);

        // Act
        var result = await _sut.GetCatalogBrandsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCatalogTypesAsync_ShouldReturnAllTypes()
    {
        // Arrange
        var types = new List<CatalogType>
        {
            new() { Id = 1, Type = "Mug" },
            new() { Id = 2, Type = "T-Shirt" }
        };
        _mockTypeRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(types);

        // Act
        var result = await _sut.GetCatalogTypesAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
