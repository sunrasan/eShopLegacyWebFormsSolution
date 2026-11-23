using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShopLegacyWebForms.Web.Pages;

/// <summary>
/// Page model for the Contact page.
/// </summary>
public class ContactModel : PageModel
{
    private readonly ILogger<ContactModel> _logger;

    public ContactModel(ILogger<ContactModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Contact page loaded");
    }
}
