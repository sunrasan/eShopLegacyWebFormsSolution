using System.ComponentModel.DataAnnotations;

namespace eShopLegacyWebForms.Domain.Entities;

/// <summary>
/// Represents a catalog item (product) in the eShop.
/// </summary>
public class CatalogItem
{
    public const string DefaultPictureName = "dummy.png";

    public CatalogItem()
    {
        PictureFileName = DefaultPictureName;
        Name = string.Empty;
        Description = string.Empty;
        PictureUri = string.Empty;
    }

    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public string Description { get; set; }

    [RegularExpression(@"^\d+(\.\d{0,2})*$", ErrorMessage = "The field Price must be a positive number with maximum two decimals.")]
    [Range(0, 9999999999999999.99)]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Picture name")]
    public string PictureFileName { get; set; }

    public string PictureUri { get; set; }

    [Display(Name = "Type")]
    public int CatalogTypeId { get; set; }

    [Display(Name = "Type")]
    public CatalogType? CatalogType { get; set; }

    [Display(Name = "Brand")]
    public int CatalogBrandId { get; set; }

    [Display(Name = "Brand")]
    public CatalogBrand? CatalogBrand { get; set; }

    [Range(0, 10000000, ErrorMessage = "The field Stock must be between 0 and 10 million.")]
    [Display(Name = "Stock")]
    public int AvailableStock { get; set; }

    [Range(0, 10000000, ErrorMessage = "The field Stock must be between 0 and 10 million.")]
    [Display(Name = "Restock")]
    public int RestockThreshold { get; set; }

    [Range(0, 10000000, ErrorMessage = "The field Stock must be between 0 and 10 million.")]
    [Display(Name = "Max stock")]
    public int MaxStockThreshold { get; set; }

    /// <summary>
    /// True if item is on reorder.
    /// </summary>
    public bool OnReorder { get; set; }
}
