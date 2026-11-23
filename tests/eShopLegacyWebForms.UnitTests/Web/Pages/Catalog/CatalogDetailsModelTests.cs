using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.Pages.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopLegacyWebForms.UnitTests.Web.Pages.Catalog;

/// <summary>
/// Unit tests for Catalog Details page model.
/// </summary>
public class CatalogDetailsModelTests
{
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ILogger<DetailsModel>> _mockLogger;
    private readonly DetailsModel _sut;

    public CatalogDetailsModelTests()
    {
        _mockCatalogService = new Mock<ICatalogService>();
        _mockLogger = new Mock<ILogger<DetailsModel>>();
        _sut = new DetailsModel(_mockCatalogService.Object, _mockLogger.Object);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act
        var model = new DetailsModel(_mockCatalogService.Object, _mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullCatalogService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new DetailsModel(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("catalogService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new DetailsModel(_mockCatalogService.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    // ─── OnGetAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task OnGetAsync_WithExistingItem_ShouldPopulateItemAndReturnPage()
    {
        // Arrange
        var item = new CatalogItem
        {
            Id = 1,
            Name = "Detail Item",
            Description = "Detailed description",
            Price = 14.99m,
            PictureFileName = "detail.png",
            PictureUri = "/img/detail.png",
            CatalogTypeId = 1,
            CatalogType = new CatalogType { Id = 1, Type = "Mug" },
            CatalogBrandId = 2,
            CatalogBrand = new CatalogBrand { Id = 2, Brand = "Azure" },
            AvailableStock = 30,
            RestockThreshold = 3,
            MaxStockThreshold = 60,
            OnReorder = true
        };
        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(1, default)).ReturnsAsync(item);

        // Act
        var result = await _sut.OnGetAsync(1);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Item.Should().NotBeNull();
        _sut.Item!.Id.Should().Be(1);
        _sut.Item.Name.Should().Be("Detail Item");
        _sut.Item.Description.Should().Be("Detailed description");
        _sut.Item.Price.Should().Be(14.99m);
        _sut.Item.PictureFileName.Should().Be("detail.png");
        _sut.Item.PictureUri.Should().Be("/img/detail.png");
        _sut.Item.CatalogTypeId.Should().Be(1);
        _sut.Item.CatalogTypeName.Should().Be("Mug");
        _sut.Item.CatalogBrandId.Should().Be(2);
        _sut.Item.CatalogBrandName.Should().Be("Azure");
        _sut.Item.AvailableStock.Should().Be(30);
        _sut.Item.RestockThreshold.Should().Be(3);
        _sut.Item.MaxStockThreshold.Should().Be(60);
        _sut.Item.OnReorder.Should().BeTrue();
    }

    [Fact]
    public async Task OnGetAsync_WithNonExistentItem_ShouldReturnNotFound()
    {
        // Arrange
        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(999, default)).ReturnsAsync((CatalogItem?)null);

        // Act
        var result = await _sut.OnGetAsync(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _sut.Item.Should().BeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_ShouldReturnStatusCode500()
    {
        // Arrange
        _mockCatalogService
            .Setup(s => s.FindCatalogItemAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnGetAsync(1);

        // Assert
        result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result).StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task OnGetAsync_WithItemHavingNullCatalogTypeAndBrand_ShouldMapEmptyStrings()
    {
        // Arrange
        var item = new CatalogItem
        {
            Id = 1,
            Name = "Item",
            Price = 5m,
            PictureFileName = "i.png",
            PictureUri = "/img/i.png",
            CatalogTypeId = 1,
            CatalogType = null,
            CatalogBrandId = 1,
            CatalogBrand = null,
            AvailableStock = 10,
            RestockThreshold = 2,
            MaxStockThreshold = 50
        };
        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(1, default)).ReturnsAsync(item);

        // Act
        await _sut.OnGetAsync(1);

        // Assert
        _sut.Item!.CatalogTypeName.Should().BeEmpty();
        _sut.Item.CatalogBrandName.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_ShouldCallServiceWithCorrectId()
    {
        // Arrange
        int capturedId = 0;
        _mockCatalogService
            .Setup(s => s.FindCatalogItemAsync(It.IsAny<int>(), default))
            .Callback<int, System.Threading.CancellationToken>((id, _) => capturedId = id)
            .ReturnsAsync((CatalogItem?)null);

        // Act
        await _sut.OnGetAsync(77);

        // Assert
        capturedId.Should().Be(77);
    }

    [Fact]
    public async Task OnGetAsync_WithOnReorderFalse_ShouldMapCorrectly()
    {
        // Arrange
        var item = new CatalogItem
        {
            Id = 1,
            Name = "Item",
            Price = 5m,
            PictureFileName = "i.png",
            PictureUri = "/img/i.png",
            CatalogTypeId = 1,
            CatalogBrandId = 1,
            AvailableStock = 10,
            RestockThreshold = 2,
            MaxStockThreshold = 50,
            OnReorder = false
        };
        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(1, default)).ReturnsAsync(item);

        // Act
        await _sut.OnGetAsync(1);

        // Assert
        _sut.Item!.OnReorder.Should().BeFalse();
    }

    [Fact]
    public void Item_DefaultValue_ShouldBeNull()
    {
        // Assert
        _sut.Item.Should().BeNull();
    }
}
