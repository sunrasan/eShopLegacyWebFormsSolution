namespace eShopLegacyWebForms.Application.DTOs;

/// <summary>
/// DTO for catalog item data.
/// </summary>
public class CatalogItemDto
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
