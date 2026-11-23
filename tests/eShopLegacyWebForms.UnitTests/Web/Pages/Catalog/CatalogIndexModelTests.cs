using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.Pages.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopLegacyWebForms.UnitTests.Web.Pages.Catalog;

/// <summary>
/// Unit tests for Catalog Index page model.
/// </summary>
public class CatalogIndexModelTests
{
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _sut;

    public CatalogIndexModelTests()
    {
        _mockCatalogService = new Mock<ICatalogService>();
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _sut = new IndexModel(_mockCatalogService.Object, _mockLogger.Object);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act
        var model = new IndexModel(_mockCatalogService.Object, _mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullCatalogService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new IndexModel(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("catalogService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new IndexModel(_mockCatalogService.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    // ─── OnGetAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task OnGetAsync_WithDefaultParameters_ShouldPopulateViewModel()
    {
        // Arrange
        var catalogItems = new List<CatalogItem>
        {
            new() { Id = 1, Name = "Item 1", Price = 10.0m, PictureFileName = "1.png", PictureUri = "/img/1.png",
                    CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock = 10, RestockThreshold = 2, MaxStockThreshold = 50 },
            new() { Id = 2, Name = "Item 2", Price = 20.0m, PictureFileName = "2.png", PictureUri = "/img/2.png",
                    CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 5, RestockThreshold = 1, MaxStockThreshold = 25 }
        };
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(0, 10, default))
            .ReturnsAsync((catalogItems, 2L));

        // Act
        await _sut.OnGetAsync();

        // Assert
        _sut.ViewModel.Should().NotBeNull();
        _sut.ViewModel.Items.Should().HaveCount(2);
        _sut.ViewModel.ActualPage.Should().Be(0);
        _sut.ViewModel.ItemsPerPage.Should().Be(10);
        _sut.ViewModel.TotalItems.Should().Be(2);
        _sut.ViewModel.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task OnGetAsync_WithCustomPageParameters_ShouldUseProvidedValues()
    {
        // Arrange
        var catalogItems = new List<CatalogItem>
        {
            new() { Id = 3, Name = "Item 3", Price = 30.0m, PictureFileName = "3.png", PictureUri = "/img/3.png",
                    CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock = 20, RestockThreshold = 5, MaxStockThreshold = 100 }
        };
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(1, 5, default))
            .ReturnsAsync((catalogItems, 11L));

        // Act
        await _sut.OnGetAsync(pageIndex: 1, pageSize: 5);

        // Assert
        _sut.ViewModel.ActualPage.Should().Be(1);
        _sut.ViewModel.ItemsPerPage.Should().Be(5);
        _sut.ViewModel.TotalItems.Should().Be(11);
        _sut.ViewModel.TotalPages.Should().Be(3); // ceil(11/5) = 3
    }

    [Fact]
    public async Task OnGetAsync_WithEmptyItems_ShouldReturnEmptyViewModel()
    {
        // Arrange
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(0, 10, default))
            .ReturnsAsync((new List<CatalogItem>(), 0L));

        // Act
        await _sut.OnGetAsync();

        // Assert
        _sut.ViewModel.Items.Should().BeEmpty();
        _sut.ViewModel.TotalItems.Should().Be(0);
        _sut.ViewModel.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ShouldReturnEmptyViewModel()
    {
        // Arrange
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _sut.OnGetAsync();

        // Assert
        _sut.ViewModel.Should().NotBeNull();
        _sut.ViewModel.Items.Should().BeEmpty();
        _sut.ViewModel.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task OnGetAsync_WithItemHavingCatalogTypeAndBrand_ShouldMapNames()
    {
        // Arrange
        var catalogItems = new List<CatalogItem>
        {
            new()
            {
                Id = 1,
                Name = "Mug",
                Price = 5.0m,
                PictureFileName = "mug.png",
                PictureUri = "/img/mug.png",
                CatalogTypeId = 1,
                CatalogType = new CatalogType { Id = 1, Type = "Mug" },
                CatalogBrandId = 1,
                CatalogBrand = new CatalogBrand { Id = 1, Brand = "Azure" },
                AvailableStock = 10,
                RestockThreshold = 2,
                MaxStockThreshold = 50,
                OnReorder = false
            }
        };
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(0, 10, default))
            .ReturnsAsync((catalogItems, 1L));

        // Act
        await _sut.OnGetAsync();

        // Assert
        var item = _sut.ViewModel.Items.First();
        item.CatalogTypeName.Should().Be("Mug");
        item.CatalogBrandName.Should().Be("Azure");
    }

    [Fact]
    public async Task OnGetAsync_WithItemHavingNullCatalogTypeAndBrand_ShouldMapEmptyStrings()
    {
        // Arrange
        var catalogItems = new List<CatalogItem>
        {
            new()
            {
                Id = 1,
                Name = "Item",
                Price = 5.0m,
                PictureFileName = "item.png",
                PictureUri = "/img/item.png",
                CatalogTypeId = 1,
                CatalogType = null,
                CatalogBrandId = 1,
                CatalogBrand = null,
                AvailableStock = 10,
                RestockThreshold = 2,
                MaxStockThreshold = 50
            }
        };
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(0, 10, default))
            .ReturnsAsync((catalogItems, 1L));

        // Act
        await _sut.OnGetAsync();

        // Assert
        var item = _sut.ViewModel.Items.First();
        item.CatalogTypeName.Should().BeEmpty();
        item.CatalogBrandName.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_TotalPages_ShouldCalculateCorrectly()
    {
        // Arrange - 25 items, 10 per page = 3 pages
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(0, 10, default))
            .ReturnsAsync((new List<CatalogItem>(), 25L));

        // Act
        await _sut.OnGetAsync(pageIndex: 0, pageSize: 10);

        // Assert
        _sut.ViewModel.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task OnGetAsync_TotalPages_ExactDivision_ShouldNotAddExtraPage()
    {
        // Arrange - 20 items, 10 per page = exactly 2 pages
        _mockCatalogService
            .Setup(s => s.GetCatalogItemsPaginatedAsync(0, 10, default))
            .ReturnsAsync((new List<CatalogItem>(), 20L));

        // Act
        await _sut.OnGetAsync(pageIndex: 0, pageSize: 10);

        // Assert
        _sut.ViewModel.TotalPages.Should().Be(2);
    }

    [Fact]
    public void ViewModel_DefaultValue_ShouldBeInitialized()
    {
        // Assert
        _sut.ViewModel.Should().NotBeNull();
        _sut.ViewModel.Items.Should().BeEmpty();
    }
}
