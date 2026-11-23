using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShopLegacyWebForms.Web.Pages;

/// <summary>
/// Page model for the About page.
/// </summary>
public class AboutModel : PageModel
{
    private readonly ILogger<AboutModel> _logger;

    public AboutModel(ILogger<AboutModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("About page loaded");
    }
}
