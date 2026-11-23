using System.ComponentModel.DataAnnotations;

namespace eShopLegacyWebForms.Application.DTOs;

/// <summary>
/// DTO for creating a new catalog item.
/// </summary>
public class CatalogItemCreateDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0, 9999999999999999.99)]
    public decimal Price { get; set; }

    public string PictureFileName { get; set; } = "dummy.png";

    [Required]
    public int CatalogTypeId { get; set; }

    [Required]
    public int CatalogBrandId { get; set; }

    [Range(0, 10000000)]
    public int AvailableStock { get; set; }

    [Range(0, 10000000)]
    public int RestockThreshold { get; set; }

    [Range(0, 10000000)]
    public int MaxStockThreshold { get; set; }

    public bool OnReorder { get; set; }
}
