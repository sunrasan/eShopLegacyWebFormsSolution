using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using eShopLegacyWebForms.Domain.Entities;
using eShopLegacyWebForms.Domain.Interfaces.Services;
using eShopLegacyWebForms.Web.Pages.Catalog;
using eShopLegacyWebForms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopLegacyWebForms.UnitTests.Web.Pages.Catalog;

/// <summary>
/// Unit tests for Catalog Edit page model.
/// </summary>
public class CatalogEditModelTests
{
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ILogger<EditModel>> _mockLogger;
    private readonly EditModel _sut;

    public CatalogEditModelTests()
    {
        _mockCatalogService = new Mock<ICatalogService>();
        _mockLogger = new Mock<ILogger<EditModel>>();
        _sut = new EditModel(_mockCatalogService.Object, _mockLogger.Object);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act
        var model = new EditModel(_mockCatalogService.Object, _mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullCatalogService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new EditModel(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("catalogService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new EditModel(_mockCatalogService.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    // ─── OnGetAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task OnGetAsync_WithExistingItem_ShouldPopulateInputAndReturnPage()
    {
        // Arrange
        var item = new CatalogItem
        {
            Id = 1,
            Name = "Test Item",
            Description = "Test Description",
            Price = 25.99m,
            PictureFileName = "test.png",
            CatalogTypeId = 2,
            CatalogBrandId = 3,
            AvailableStock = 50,
            RestockThreshold = 5,
            MaxStockThreshold = 100,
            OnReorder = false
        };
        var brands = new List<CatalogBrand> { new() { Id = 3, Brand = "Azure" } };
        var types = new List<CatalogType> { new() { Id = 2, Type = "Mug" } };

        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(1, default)).ReturnsAsync(item);
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(brands);
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(types);

        // Act
        var result = await _sut.OnGetAsync(1);

        // Assert
        result.Should().BeOfType<PageResult>();
        _sut.Input.Id.Should().Be(1);
        _sut.Input.Name.Should().Be("Test Item");
        _sut.Input.Description.Should().Be("Test Description");
        _sut.Input.Price.Should().Be(25.99m);
        _sut.Input.PictureFileName.Should().Be("test.png");
        _sut.Input.CatalogTypeId.Should().Be(2);
        _sut.Input.CatalogBrandId.Should().Be(3);
        _sut.Input.AvailableStock.Should().Be(50);
        _sut.Input.RestockThreshold.Should().Be(5);
        _sut.Input.MaxStockThreshold.Should().Be(100);
        _sut.Input.OnReorder.Should().BeFalse();
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
    public async Task OnGetAsync_ShouldPopulateDropdownsWithSelectedValues()
    {
        // Arrange
        var item = new CatalogItem
        {
            Id = 1, Name = "Item", Price = 10m, PictureFileName = "i.png",
            CatalogTypeId = 2, CatalogBrandId = 3,
            AvailableStock = 10, RestockThreshold = 1, MaxStockThreshold = 50
        };
        var brands = new List<CatalogBrand>
        {
            new() { Id = 3, Brand = "Azure" },
            new() { Id = 4, Brand = ".NET" }
        };
        var types = new List<CatalogType>
        {
            new() { Id = 1, Type = "Mug" },
            new() { Id = 2, Type = "T-Shirt" }
        };

        _mockCatalogService.Setup(s => s.FindCatalogItemAsync(1, default)).ReturnsAsync(item);
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(brands);
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(types);

        // Act
        await _sut.OnGetAsync(1);

        // Assert
        var selectedBrand = _sut.Input.Brands.FirstOrDefault(b => b.Selected);
        selectedBrand.Should().NotBeNull();
        selectedBrand!.Value.Should().Be("3");

        var selectedType = _sut.Input.Types.FirstOrDefault(t => t.Selected);
        selectedType.Should().NotBeNull();
        selectedType!.Value.Should().Be("2");
    }

    // ─── OnPostAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task OnPostAsync_WithValidModel_ShouldUpdateItemAndRedirect()
    {
        // Arrange
        _sut.Input = new CatalogItemEditViewModel
        {
            Id = 1,
            Name = "Updated Product",
            Description = "Updated description",
            Price = 35.99m,
            PictureFileName = "updated.png",
            CatalogTypeId = 1,
            CatalogBrandId = 2,
            AvailableStock = 75,
            RestockThreshold = 10,
            MaxStockThreshold = 200,
            OnReorder = true
        };
        _mockCatalogService
            .Setup(s => s.UpdateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("./Index");
        _mockCatalogService.Verify(s => s.UpdateCatalogItemAsync(It.IsAny<CatalogItem>(), default), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_ShouldMapAllFieldsToEntity()
    {
        // Arrange
        CatalogItem? capturedItem = null;
        _sut.Input = new CatalogItemEditViewModel
        {
            Id = 5,
            Name = "Mapped Product",
            Description = "Mapped Description",
            Price = 49.99m,
            PictureFileName = "mapped.png",
            CatalogTypeId = 3,
            CatalogBrandId = 4,
            AvailableStock = 100,
            RestockThreshold = 15,
            MaxStockThreshold = 300,
            OnReorder = false
        };
        _mockCatalogService
            .Setup(s => s.UpdateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Callback<CatalogItem, System.Threading.CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.OnPostAsync();

        // Assert
        capturedItem.Should().NotBeNull();
        capturedItem!.Id.Should().Be(5);
        capturedItem.Name.Should().Be("Mapped Product");
        capturedItem.Description.Should().Be("Mapped Description");
        capturedItem.Price.Should().Be(49.99m);
        capturedItem.PictureFileName.Should().Be("mapped.png");
        capturedItem.CatalogTypeId.Should().Be(3);
        capturedItem.CatalogBrandId.Should().Be(4);
        capturedItem.AvailableStock.Should().Be(100);
        capturedItem.RestockThreshold.Should().Be(15);
        capturedItem.MaxStockThreshold.Should().Be(300);
        capturedItem.OnReorder.Should().BeFalse();
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ShouldReturnPageWithError()
    {
        // Arrange
        _sut.Input = new CatalogItemEditViewModel
        {
            Id = 1,
            Name = "Product",
            Price = 10m,
            PictureFileName = "test.png",
            CatalogTypeId = 1,
            CatalogBrandId = 1
        };
        _mockCatalogService
            .Setup(s => s.UpdateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .ThrowsAsync(new Exception("DB error"));
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(new List<CatalogBrand>());
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(new List<CatalogType>());

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void Input_DefaultValue_ShouldBeInitialized()
    {
        // Assert
        _sut.Input.Should().NotBeNull();
    }
}
