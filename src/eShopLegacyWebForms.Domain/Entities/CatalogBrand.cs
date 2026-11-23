namespace eShopLegacyWebForms.Domain.Entities;

/// <summary>
/// Represents a catalog brand.
/// </summary>
public class CatalogBrand
{
    public CatalogBrand()
    {
        Brand = string.Empty;
    }

    public int Id { get; set; }

    public string Brand { get; set; }
}
