using Xunit;
using FluentAssertions;
using eShopLegacyWebForms.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace eShopLegacyWebForms.UnitTests.Web.ViewModels;

/// <summary>
/// Unit tests for CatalogViewModels.
/// </summary>
public class CatalogViewModelsTests
{
    // ─── CatalogIndexViewModel ───────────────────────────────────────────────

    [Fact]
    public void CatalogIndexViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new CatalogIndexViewModel();

        // Assert
        vm.Items.Should().NotBeNull();
        vm.Items.Should().BeEmpty();
        vm.ActualPage.Should().Be(0);
        vm.ItemsPerPage.Should().Be(0);
        vm.TotalItems.Should().Be(0);
        vm.TotalPages.Should().Be(0);
    }

    [Fact]
    public void CatalogIndexViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var items = new List<CatalogItemViewModel>
        {
            new() { Id = 1, Name = "Item A" },
            new() { Id = 2, Name = "Item B" }
        };

        // Act
        var vm = new CatalogIndexViewModel
        {
            Items = items,
            ActualPage = 2,
            ItemsPerPage = 10,
            TotalItems = 25,
            TotalPages = 3
        };

        // Assert
        vm.Items.Should().HaveCount(2);
        vm.ActualPage.Should().Be(2);
        vm.ItemsPerPage.Should().Be(10);
        vm.TotalItems.Should().Be(25);
        vm.TotalPages.Should().Be(3);
    }

    // ─── CatalogItemViewModel ────────────────────────────────────────────────

    [Fact]
    public void CatalogItemViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new CatalogItemViewModel();

        // Assert
        vm.Id.Should().Be(0);
        vm.Name.Should().BeEmpty();
        vm.Description.Should().BeEmpty();
        vm.Price.Should().Be(0m);
        vm.PictureFileName.Should().BeEmpty();
        vm.PictureUri.Should().BeEmpty();
        vm.CatalogTypeId.Should().Be(0);
        vm.CatalogTypeName.Should().BeEmpty();
        vm.CatalogBrandId.Should().Be(0);
        vm.CatalogBrandName.Should().BeEmpty();
        vm.AvailableStock.Should().Be(0);
        vm.RestockThreshold.Should().Be(0);
        vm.MaxStockThreshold.Should().Be(0);
        vm.OnReorder.Should().BeFalse();
    }

    [Fact]
    public void CatalogItemViewModel_SetAllProperties_ShouldRetainValues()
    {
        // Act
        var vm = new CatalogItemViewModel
        {
            Id = 42,
            Name = "Test Product",
            Description = "A test product",
            Price = 19.99m,
            PictureFileName = "test.png",
            PictureUri = "/images/test.png",
            CatalogTypeId = 1,
            CatalogTypeName = "Mug",
            CatalogBrandId = 2,
            CatalogBrandName = "Azure",
            AvailableStock = 100,
            RestockThreshold = 10,
            MaxStockThreshold = 200,
            OnReorder = true
        };

        // Assert
        vm.Id.Should().Be(42);
        vm.Name.Should().Be("Test Product");
        vm.Description.Should().Be("A test product");
        vm.Price.Should().Be(19.99m);
        vm.PictureFileName.Should().Be("test.png");
        vm.PictureUri.Should().Be("/images/test.png");
        vm.CatalogTypeId.Should().Be(1);
        vm.CatalogTypeName.Should().Be("Mug");
        vm.CatalogBrandId.Should().Be(2);
        vm.CatalogBrandName.Should().Be("Azure");
        vm.AvailableStock.Should().Be(100);
        vm.RestockThreshold.Should().Be(10);
        vm.MaxStockThreshold.Should().Be(200);
        vm.OnReorder.Should().BeTrue();
    }

    // ─── CatalogItemCreateViewModel ──────────────────────────────────────────

    [Fact]
    public void CatalogItemCreateViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new CatalogItemCreateViewModel();

        // Assert
        vm.Name.Should().BeEmpty();
        vm.Description.Should().BeEmpty();
        vm.Price.Should().Be(0m);
        vm.PictureFileName.Should().Be("dummy.png");
        vm.CatalogTypeId.Should().Be(0);
        vm.CatalogBrandId.Should().Be(0);
        vm.AvailableStock.Should().Be(0);
        vm.RestockThreshold.Should().Be(0);
        vm.MaxStockThreshold.Should().Be(0);
        vm.OnReorder.Should().BeFalse();
        vm.Brands.Should().NotBeNull();
        vm.Brands.Should().BeEmpty();
        vm.Types.Should().NotBeNull();
        vm.Types.Should().BeEmpty();
    }

    [Fact]
    public void CatalogItemCreateViewModel_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var brands = new List<SelectListItemViewModel>
        {
            new() { Value = "1", Text = "Azure", Selected = true }
        };
        var types = new List<SelectListItemViewModel>
        {
            new() { Value = "2", Text = "Mug", Selected = false }
        };

        // Act
        var vm = new CatalogItemCreateViewModel
        {
            Name = "New Item",
            Description = "Description",
            Price = 9.99m,
            PictureFileName = "new.png",
            CatalogTypeId = 2,
            CatalogBrandId = 1,
            AvailableStock = 50,
            RestockThreshold = 5,
            MaxStockThreshold = 100,
            OnReorder = false,
            Brands = brands,
            Types = types
        };

        // Assert
        vm.Name.Should().Be("New Item");
        vm.Description.Should().Be("Description");
        vm.Price.Should().Be(9.99m);
        vm.PictureFileName.Should().Be("new.png");
        vm.CatalogTypeId.Should().Be(2);
        vm.CatalogBrandId.Should().Be(1);
        vm.AvailableStock.Should().Be(50);
        vm.RestockThreshold.Should().Be(5);
        vm.MaxStockThreshold.Should().Be(100);
        vm.OnReorder.Should().BeFalse();
        vm.Brands.Should().HaveCount(1);
        vm.Types.Should().HaveCount(1);
    }

    // ─── CatalogItemEditViewModel ────────────────────────────────────────────

    [Fact]
    public void CatalogItemEditViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new CatalogItemEditViewModel();

        // Assert
        vm.Id.Should().Be(0);
        vm.Name.Should().BeEmpty();
        vm.Description.Should().BeEmpty();
        vm.Price.Should().Be(0m);
        vm.PictureFileName.Should().BeEmpty();
        vm.CatalogTypeId.Should().Be(0);
        vm.CatalogBrandId.Should().Be(0);
        vm.AvailableStock.Should().Be(0);
        vm.RestockThreshold.Should().Be(0);
        vm.MaxStockThreshold.Should().Be(0);
        vm.OnReorder.Should().BeFalse();
        vm.Brands.Should().NotBeNull();
        vm.Brands.Should().BeEmpty();
        vm.Types.Should().NotBeNull();
        vm.Types.Should().BeEmpty();
    }

    [Fact]
    public void CatalogItemEditViewModel_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var brands = new List<SelectListItemViewModel>
        {
            new() { Value = "1", Text = "Azure", Selected = true },
            new() { Value = "2", Text = ".NET", Selected = false }
        };
        var types = new List<SelectListItemViewModel>
        {
            new() { Value = "1", Text = "Mug", Selected = false },
            new() { Value = "2", Text = "T-Shirt", Selected = true }
        };

        // Act
        var vm = new CatalogItemEditViewModel
        {
            Id = 10,
            Name = "Edited Item",
            Description = "Edited description",
            Price = 29.99m,
            PictureFileName = "edited.png",
            CatalogTypeId = 2,
            CatalogBrandId = 1,
            AvailableStock = 75,
            RestockThreshold = 15,
            MaxStockThreshold = 150,
            OnReorder = true,
            Brands = brands,
            Types = types
        };

        // Assert
        vm.Id.Should().Be(10);
        vm.Name.Should().Be("Edited Item");
        vm.Description.Should().Be("Edited description");
        vm.Price.Should().Be(29.99m);
        vm.PictureFileName.Should().Be("edited.png");
        vm.CatalogTypeId.Should().Be(2);
        vm.CatalogBrandId.Should().Be(1);
        vm.AvailableStock.Should().Be(75);
        vm.RestockThreshold.Should().Be(15);
        vm.MaxStockThreshold.Should().Be(150);
        vm.OnReorder.Should().BeTrue();
        vm.Brands.Should().HaveCount(2);
        vm.Types.Should().HaveCount(2);
    }

    // ─── SelectListItemViewModel ─────────────────────────────────────────────

    [Fact]
    public void SelectListItemViewModel_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var vm = new SelectListItemViewModel();

        // Assert
        vm.Value.Should().BeEmpty();
        vm.Text.Should().BeEmpty();
        vm.Selected.Should().BeFalse();
    }

    [Fact]
    public void SelectListItemViewModel_SetProperties_ShouldRetainValues()
    {
        // Act
        var vm = new SelectListItemViewModel
        {
            Value = "42",
            Text = "Option 42",
            Selected = true
        };

        // Assert
        vm.Value.Should().Be("42");
        vm.Text.Should().Be("Option 42");
        vm.Selected.Should().BeTrue();
    }

    [Fact]
    public void SelectListItemViewModel_Selected_DefaultIsFalse()
    {
        // Act
        var vm = new SelectListItemViewModel { Value = "1", Text = "One" };

        // Assert
        vm.Selected.Should().BeFalse();
    }

    [Fact]
    public void CatalogIndexViewModel_Items_CanBeSetToNonEmptyCollection()
    {
        // Arrange
        var items = Enumerable.Range(1, 5).Select(i => new CatalogItemViewModel
        {
            Id = i,
            Name = $"Item {i}",
            Price = i * 10m
        }).ToList();

        // Act
        var vm = new CatalogIndexViewModel { Items = items };

        // Assert
        vm.Items.Should().HaveCount(5);
        vm.Items.First().Id.Should().Be(1);
        vm.Items.Last().Id.Should().Be(5);
    }

    [Fact]
    public void CatalogItemCreateViewModel_PictureFileName_DefaultIsDummyPng()
    {
        // Act
        var vm = new CatalogItemCreateViewModel();

        // Assert
        vm.PictureFileName.Should().Be("dummy.png");
    }

    [Fact]
    public void CatalogItemEditViewModel_Brands_CanContainSelectedItem()
    {
        // Arrange
        var brands = new List<SelectListItemViewModel>
        {
            new() { Value = "1", Text = "Azure", Selected = false },
            new() { Value = "2", Text = ".NET", Selected = true },
            new() { Value = "3", Text = "Other", Selected = false }
        };

        // Act
        var vm = new CatalogItemEditViewModel { Brands = brands };
        var selectedBrand = vm.Brands.FirstOrDefault(b => b.Selected);

        // Assert
        selectedBrand.Should().NotBeNull();
        selectedBrand!.Value.Should().Be("2");
        selectedBrand.Text.Should().Be(".NET");
    }
}
