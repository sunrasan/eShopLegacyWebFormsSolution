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
/// Unit tests for Catalog Delete page model.
/// </summary>
public class CatalogDeleteModelTests
{
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ILogger<DeleteModel>> _mockLogger;
    private readonly DeleteModel _sut;

    public CatalogDeleteModelTests()
    {
        _mockCatalogService = new Mock<ICatalogService>();
        _mockLogger = new Mock<ILogger<DeleteModel>>();
        _sut = new DeleteModel(_mockCatalogService.Object, _mockLogger.Object);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act
        var model = new DeleteModel(_mockCatalogService.Object, _mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullCatalogService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new DeleteModel(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("catalogService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new DeleteModel(_mockCatalogService.Object, null!);

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
            Name = "Delete Me",
            Description = "To be deleted",
            Price = 9.99m,
            PictureFileName = "delete.png",
            PictureUri = "/img/delete.png",
            CatalogTypeId = 1,
            CatalogType = new CatalogType { Id = 1, Type = "Mug" },
            CatalogBrandId = 2,
            CatalogBrand = new CatalogBrand { Id = 2, Brand = "Azure" },
            AvailableStock = 5,
            RestockThreshold = 1,
            MaxStockThreshold = 20,
            OnReorder = false
        };
        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(1, default)).ReturnsAsync(item);

        // Act
        var result = await _sut.OnGetAsync(1);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Item.Should().NotBeNull();
        _sut.Item!.Id.Should().Be(1);
        _sut.Item.Name.Should().Be("Delete Me");
        _sut.Item.Description.Should().Be("To be deleted");
        _sut.Item.Price.Should().Be(9.99m);
        _sut.Item.PictureFileName.Should().Be("delete.png");
        _sut.Item.PictureUri.Should().Be("/img/delete.png");
        _sut.Item.CatalogTypeId.Should().Be(1);
        _sut.Item.CatalogTypeName.Should().Be("Mug");
        _sut.Item.CatalogBrandId.Should().Be(2);
        _sut.Item.CatalogBrandName.Should().Be("Azure");
        _sut.Item.AvailableStock.Should().Be(5);
        _sut.Item.RestockThreshold.Should().Be(1);
        _sut.Item.MaxStockThreshold.Should().Be(20);
        _sut.Item.OnReorder.Should().BeFalse();
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

    // ─── OnPostAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task OnPostAsync_WithExistingItem_ShouldDeleteAndRedirect()
    {
        // Arrange
        _mockCatalogService
            .Setup(s => s.RemoveCatalogItemAsync(1, default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.OnPostAsync(1);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("./Index");
        _mockCatalogService.Verify(s => s.RemoveCatalogItemAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ShouldReturnStatusCode500()
    {
        // Arrange
        _mockCatalogService
            .Setup(s => s.RemoveCatalogItemAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.OnPostAsync(1);

        // Assert
        result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result).StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task OnPostAsync_ShouldCallRemoveWithCorrectId()
    {
        // Arrange
        int capturedId = 0;
        _mockCatalogService
            .Setup(s => s.RemoveCatalogItemAsync(It.IsAny<int>(), default))
            .Callback<int, System.Threading.CancellationToken>((id, _) => capturedId = id)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.OnPostAsync(42);

        // Assert
        capturedId.Should().Be(42);
    }

    [Fact]
    public void Item_DefaultValue_ShouldBeNull()
    {
        // Assert
        _sut.Item.Should().BeNull();
    }
}
