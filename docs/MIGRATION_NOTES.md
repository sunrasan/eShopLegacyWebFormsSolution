# Migration Notes

## What Was Migrated

### Pages
| Web Forms Page | Razor Page |
|---------------|------------|
| Default.aspx | Pages/Index.cshtml |
| Catalog/Create.aspx | Pages/Catalog/Create.cshtml |
| Catalog/Details.aspx | Pages/Catalog/Details.cshtml |
| Catalog/Edit.aspx | Pages/Catalog/Edit.cshtml |
| Catalog/Delete.aspx | Pages/Catalog/Delete.cshtml |
| About.aspx | Pages/About.cshtml |
| Contact.aspx | Pages/Contact.cshtml |

### Infrastructure
| Old | New |
|-----|-----|
| Global.asax | Program.cs |
| Web.config | appsettings.json |
| packages.config | SDK-style PackageReference |
| Site.Master | Pages/Shared/_Layout.cshtml |

### Dependencies
| Old Package | New Package |
|------------|-------------|
| EntityFramework 6.2.0 | Microsoft.EntityFrameworkCore 8.0.0 |
| log4net 2.0.10 | Serilog.AspNetCore 8.0.0 |
| Autofac 4.9.1 | Built-in Microsoft.Extensions.DependencyInjection |
| Autofac.Web 4.0.0 | (removed - not needed) |
| System.Web | Microsoft.AspNetCore |

## Key Differences from Web Forms

1. **No ViewState** - State is managed via form posts and TempData
2. **No Code-Behind** - Page models replace code-behind files
3. **Async by Default** - All I/O operations use async/await
4. **Dependency Injection** - Constructor injection throughout
5. **Tag Helpers** - Replace Web Forms server controls
6. **Middleware Pipeline** - Replaces HTTP modules/handlers

## Breaking Changes

1. `HttpContext.Current` → Inject `IHttpContextAccessor`
2. `Server.MapPath` → Inject `IWebHostEnvironment`
3. `Session` → Configure distributed session
4. `Response.Redirect` → `return RedirectToPage()`
5. `Page.RouteData.Values` → Route parameters in page handler methods

## Configuration Changes

- Connection strings moved from `<connectionStrings>` in Web.config to `ConnectionStrings` section in appsettings.json
- App settings moved from `<appSettings>` to root level in appsettings.json
- Entity Framework configuration moved to `DbContextOptions` in Program.cs

## Known Issues

- HiLo sequence generator replaced with simple max+1 strategy (works for mock data; for production, consider using database sequences)
- Application Insights integration removed (can be re-added with `Microsoft.ApplicationInsights.AspNetCore`)

## Future Improvements

1. Add Application Insights telemetry
2. Implement proper database migrations
3. Add authentication/authorization
4. Implement caching strategies
5. Add API endpoints for SPA integration
