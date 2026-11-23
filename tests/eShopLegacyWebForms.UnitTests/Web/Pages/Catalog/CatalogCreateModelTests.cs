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
/// Unit tests for Catalog Create page model.
/// </summary>
public class CatalogCreateModelTests
{
    private readonly Mock<ICatalogService> _mockCatalogService;
    private readonly Mock<ILogger<CreateModel>> _mockLogger;
    private readonly CreateModel _sut;

    public CatalogCreateModelTests()
    {
        _mockCatalogService = new Mock<ICatalogService>();
        _mockLogger = new Mock<ILogger<CreateModel>>();
        _sut = new CreateModel(_mockCatalogService.Object, _mockLogger.Object);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act
        var model = new CreateModel(_mockCatalogService.Object, _mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullCatalogService_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new CreateModel(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("catalogService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new CreateModel(_mockCatalogService.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    // ─── OnGetAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task OnGetAsync_ShouldPopulateDropdowns()
    {
        // Arrange
        var brands = new List<CatalogBrand>
        {
            new() { Id = 1, Brand = "Azure" },
            new() { Id = 2, Brand = ".NET" }
        };
        var types = new List<CatalogType>
        {
            new() { Id = 1, Type = "Mug" },
            new() { Id = 2, Type = "T-Shirt" }
        };
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(brands);
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(types);

        // Act
        await _sut.OnGetAsync();

        // Assert
        _sut.Input.Brands.Should().HaveCount(2);
        _sut.Input.Types.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnGetAsync_ShouldMapBrandsCorrectly()
    {
        // Arrange
        var brands = new List<CatalogBrand>
        {
            new() { Id = 5, Brand = "TestBrand" }
        };
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(brands);
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(new List<CatalogType>());

        // Act
        await _sut.OnGetAsync();

        // Assert
        var brand = _sut.Input.Brands.First();
        brand.Value.Should().Be("5");
        brand.Text.Should().Be("TestBrand");
    }

    [Fact]
    public async Task OnGetAsync_ShouldMapTypesCorrectly()
    {
        // Arrange
        var types = new List<CatalogType>
        {
            new() { Id = 3, Type = "TestType" }
        };
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(new List<CatalogBrand>());
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(types);

        // Act
        await _sut.OnGetAsync();

        // Assert
        var type = _sut.Input.Types.First();
        type.Value.Should().Be("3");
        type.Text.Should().Be("TestType");
    }

    // ─── OnPostAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task OnPostAsync_WithValidModel_ShouldCreateItemAndRedirect()
    {
        // Arrange
        _sut.Input = new CatalogItemCreateViewModel
        {
            Name = "New Product",
            Description = "A new product",
            Price = 15.99m,
            PictureFileName = "new.png",
            CatalogTypeId = 1,
            CatalogBrandId = 2,
            AvailableStock = 50,
            RestockThreshold = 5,
            MaxStockThreshold = 100,
            OnReorder = false
        };
        _mockCatalogService
            .Setup(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = (RedirectToPageResult)result;
        redirect.PageName.Should().Be("./Index");
        _mockCatalogService.Verify(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyPictureFileName_ShouldUseDefaultPictureName()
    {
        // Arrange
        CatalogItem? capturedItem = null;
        _sut.Input = new CatalogItemCreateViewModel
        {
            Name = "Product",
            Price = 10m,
            PictureFileName = "",
            CatalogTypeId = 1,
            CatalogBrandId = 1
        };
        _mockCatalogService
            .Setup(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Callback<CatalogItem, System.Threading.CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.OnPostAsync();

        // Assert
        capturedItem.Should().NotBeNull();
        capturedItem!.PictureFileName.Should().Be(CatalogItem.DefaultPictureName);
    }

    [Fact]
    public async Task OnPostAsync_WithWhitespacePictureFileName_ShouldUseDefaultPictureName()
    {
        // Arrange
        CatalogItem? capturedItem = null;
        _sut.Input = new CatalogItemCreateViewModel
        {
            Name = "Product",
            Price = 10m,
            PictureFileName = "   ",
            CatalogTypeId = 1,
            CatalogBrandId = 1
        };
        _mockCatalogService
            .Setup(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Callback<CatalogItem, System.Threading.CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.OnPostAsync();

        // Assert
        capturedItem!.PictureFileName.Should().Be(CatalogItem.DefaultPictureName);
    }

    [Fact]
    public async Task OnPostAsync_WithProvidedPictureFileName_ShouldUseProvidedName()
    {
        // Arrange
        CatalogItem? capturedItem = null;
        _sut.Input = new CatalogItemCreateViewModel
        {
            Name = "Product",
            Price = 10m,
            PictureFileName = "custom.png",
            CatalogTypeId = 1,
            CatalogBrandId = 1
        };
        _mockCatalogService
            .Setup(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Callback<CatalogItem, System.Threading.CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.OnPostAsync();

        // Assert
        capturedItem!.PictureFileName.Should().Be("custom.png");
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ShouldReturnPageWithError()
    {
        // Arrange
        _sut.Input = new CatalogItemCreateViewModel
        {
            Name = "Product",
            Price = 10m,
            PictureFileName = "test.png",
            CatalogTypeId = 1,
            CatalogBrandId = 1
        };
        _mockCatalogService
            .Setup(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .ThrowsAsync(new Exception("DB error"));
        _mockCatalogService.Setup(s => s.GetCatalogBrandsAsync(default)).ReturnsAsync(new List<CatalogBrand>());
        _mockCatalogService.Setup(s => s.GetCatalogTypesAsync(default)).ReturnsAsync(new List<CatalogType>());

        // Act
        var result = await _sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_ShouldMapAllFieldsToEntity()
    {
        // Arrange
        CatalogItem? capturedItem = null;
        _sut.Input = new CatalogItemCreateViewModel
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            PictureFileName = "test.png",
            CatalogTypeId = 3,
            CatalogBrandId = 4,
            AvailableStock = 200,
            RestockThreshold = 20,
            MaxStockThreshold = 500,
            OnReorder = true
        };
        _mockCatalogService
            .Setup(s => s.CreateCatalogItemAsync(It.IsAny<CatalogItem>(), default))
            .Callback<CatalogItem, System.Threading.CancellationToken>((item, _) => capturedItem = item)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.OnPostAsync();

        // Assert
        capturedItem.Should().NotBeNull();
        capturedItem!.Name.Should().Be("Test Product");
        capturedItem.Description.Should().Be("Test Description");
        capturedItem.Price.Should().Be(99.99m);
        capturedItem.CatalogTypeId.Should().Be(3);
        capturedItem.CatalogBrandId.Should().Be(4);
        capturedItem.AvailableStock.Should().Be(200);
        capturedItem.RestockThreshold.Should().Be(20);
        capturedItem.MaxStockThreshold.Should().Be(500);
        capturedItem.OnReorder.Should().BeTrue();
    }

    [Fact]
    public void Input_DefaultValue_ShouldBeInitialized()
    {
        // Assert
        _sut.Input.Should().NotBeNull();
    }
}
