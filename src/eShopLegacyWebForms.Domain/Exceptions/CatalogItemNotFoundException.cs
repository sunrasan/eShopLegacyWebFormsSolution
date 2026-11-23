namespace eShopLegacyWebForms.Domain.Exceptions;

/// <summary>
/// Exception thrown when a catalog item is not found.
/// </summary>
public class CatalogItemNotFoundException : Exception
{
    public CatalogItemNotFoundException(int id)
        : base($"Catalog item with id {id} was not found.")
    {
    }

    public CatalogItemNotFoundException(string message) : base(message)
    {
    }

    public CatalogItemNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
