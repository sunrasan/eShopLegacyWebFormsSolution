namespace eShopLegacyWebForms.Domain.Entities;

/// <summary>
/// Represents a catalog type (category).
/// </summary>
public class CatalogType
{
    public CatalogType()
    {
        Type = string.Empty;
    }

    public int Id { get; set; }

    public string Type { get; set; }
}
