using System.ComponentModel.DataAnnotations;

namespace eShopLegacyWebForms.Web.ViewModels;

/// <summary>
/// ViewModel for catalog item list/index page.
/// </summary>
public class CatalogIndexViewModel
{
    public IEnumerable<CatalogItemViewModel> Items { get; set; } = Enumerable.Empty<CatalogItemViewModel>();
    public int ActualPage { get; set; }
    public int ItemsPerPage { get; set; }
    public long TotalItems { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// ViewModel for a single catalog item display.
/// </summary>
public class CatalogItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureFileName { get; set; } = string.Empty;
    public string PictureUri { get; set; } = string.Empty;
    public int CatalogTypeId { get; set; }
    public string CatalogTypeName { get; set; } = string.Empty;
    public int CatalogBrandId { get; set; }
    public string CatalogBrandName { get; set; } = string.Empty;
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}

/// <summary>
/// ViewModel for creating a new catalog item.
/// </summary>
public class CatalogItemCreateViewModel
{
    [Required]
    [MaxLength(50)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 9999999999999999.99)]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Picture name")]
    public string PictureFileName { get; set; } = "dummy.png";

    [Required]
    [Display(Name = "Type")]
    public int CatalogTypeId { get; set; }

    [Required]
    [Display(Name = "Brand")]
    public int CatalogBrandId { get; set; }

    [Range(0, 10000000)]
    [Display(Name = "Stock")]
    public int AvailableStock { get; set; }

    [Range(0, 10000000)]
    [Display(Name = "Restock")]
    public int RestockThreshold { get; set; }

    [Range(0, 10000000)]
    [Display(Name = "Max stock")]
    public int MaxStockThreshold { get; set; }

    public bool OnReorder { get; set; }

    public IEnumerable<SelectListItemViewModel> Brands { get; set; } = Enumerable.Empty<SelectListItemViewModel>();
    public IEnumerable<SelectListItemViewModel> Types { get; set; } = Enumerable.Empty<SelectListItemViewModel>();
}

/// <summary>
/// ViewModel for editing an existing catalog item.
/// </summary>
public class CatalogItemEditViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 9999999999999999.99)]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Picture name")]
    public string PictureFileName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Type")]
    public int CatalogTypeId { get; set; }

    [Required]
    [Display(Name = "Brand")]
    public int CatalogBrandId { get; set; }

    [Range(0, 10000000)]
    [Display(Name = "Stock")]
    public int AvailableStock { get; set; }

    [Range(0, 10000000)]
    [Display(Name = "Restock")]
    public int RestockThreshold { get; set; }

    [Range(0, 10000000)]
    [Display(Name = "Max stock")]
    public int MaxStockThreshold { get; set; }

    public bool OnReorder { get; set; }

    public IEnumerable<SelectListItemViewModel> Brands { get; set; } = Enumerable.Empty<SelectListItemViewModel>();
    public IEnumerable<SelectListItemViewModel> Types { get; set; } = Enumerable.Empty<SelectListItemViewModel>();
}

/// <summary>
/// Simple select list item view model.
/// </summary>
public class SelectListItemViewModel
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
}
