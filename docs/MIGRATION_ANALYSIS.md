# Migration Analysis Report: eShopLegacyWebForms → .NET 8

**Analysis Date:** 2025-01-30  
**Source Framework:** ASP.NET Web Forms 4.7.2  
**Target Framework:** .NET 8  
**Module:** eShopLegacyWebForms  

---

## Executive Summary

The eShopLegacyWebForms application is a catalog management system built on ASP.NET Web Forms 4.7.2. The migration to .NET 8 is classified as **Complex** due to the following key factors:

- **Web Forms is not supported in .NET 8** — the entire UI layer must be rewritten as Razor Pages or MVC
- **Entity Framework 6 (EF6)** is used and must be replaced with EF Core 8
- **log4net** is used for logging and must be replaced with Microsoft.Extensions.Logging / Serilog
- **Autofac** Web Forms integration must be replaced with built-in ASP.NET Core DI
- **System.Web** dependencies are pervasive throughout the codebase
- **Global.asax** application startup must be migrated to Program.cs
- **Web.config** must be migrated to appsettings.json
- **HTTP Modules** (Autofac, ApplicationInsights) must be replaced with Middleware

**Total Issues Found: 42**
- Critical: 12
- High: 14
- Medium: 10
- Low: 6

**Estimated Effort: 80–120 hours**

---

## Issue Details

### CRITICAL Issues

#### ISSUE-001: Web Forms Not Supported in .NET 8
- **File:** All .aspx, .aspx.cs, .ascx, .master files
- **Severity:** Critical
- **Description:** ASP.NET Web Forms (System.Web.UI.Page, MasterPage, UserControl) is not available in .NET 8. The entire UI layer must be rewritten.
- **Recommendation:** Migrate all .aspx pages to Razor Pages (.cshtml + .cshtml.cs) or MVC Controllers/Views.

#### ISSUE-002: Entity Framework 6 Incompatible with .NET 8
- **File:** `Models/CatalogDBContext.cs`, `packages.config`, `eShopLegacyWebForms.csproj`
- **Severity:** Critical
- **Code:** `using System.Data.Entity;` / `EntityFramework 6.2.0`
- **Description:** EF6 targets .NET Framework only and is not compatible with .NET 8.
- **Recommendation:** Replace with Microsoft.EntityFrameworkCore 8.0.0 + Microsoft.EntityFrameworkCore.SqlServer 8.0.0.

#### ISSUE-003: System.Web Dependency — HttpApplication
- **File:** `Global.asax.cs` line 16
- **Severity:** Critical
- **Code:** `public class Global : HttpApplication, IContainerProviderAccessor`
- **Description:** HttpApplication and Global.asax are not available in .NET 8.
- **Recommendation:** Migrate to Program.cs using WebApplication builder pattern.

#### ISSUE-004: System.Web Dependency — HttpContext.Current
- **File:** `Global.asax.cs` line 72, `Site.Master.cs` line 14, `ViewSwitcher.ascx.cs` line 22
- **Severity:** Critical
- **Code:** `HttpContext.Current.Session["MachineName"]`
- **Description:** HttpContext.Current is not available in .NET 8.
- **Recommendation:** Inject IHttpContextAccessor and use HttpContext property.

#### ISSUE-005: System.Web.UI.Page — Page Lifecycle
- **File:** All .aspx.cs code-behind files
- **Severity:** Critical
- **Code:** `public partial class _Default : Page` / `protected void Page_Load(object sender, EventArgs e)`
- **Description:** The Web Forms page lifecycle (Page_Load, Page_PreRender, IsPostBack, DataBind, etc.) does not exist in .NET 8.
- **Recommendation:** Migrate to Razor Pages OnGet/OnPost handlers.

#### ISSUE-006: Autofac Web Forms Integration Not Compatible
- **File:** `Global.asax.cs`, `Modules/ApplicationModule.cs`, `Web.config`
- **Severity:** Critical
- **Code:** `using Autofac.Integration.Web;` / `IContainerProviderAccessor`
- **Description:** Autofac.Integration.Web is specific to ASP.NET Web Forms and is not compatible with .NET 8.
- **Recommendation:** Replace with built-in ASP.NET Core DI (builder.Services) or Autofac.Extensions.DependencyInjection.

#### ISSUE-007: Web.config — Not Supported in .NET 8
- **File:** `Web.config`
- **Severity:** Critical
- **Code:** `<entityFramework>`, `<connectionStrings>`, `<appSettings>`, `<system.web>`
- **Description:** Web.config is not used in .NET 8 ASP.NET Core applications.
- **Recommendation:** Migrate to appsettings.json and configure services in Program.cs.

#### ISSUE-008: HTTP Modules Not Supported
- **File:** `Web.config` lines 47–52, `Global.asax.cs`
- **Severity:** Critical
- **Code:** `<httpModules>` / `ContainerDisposalModule`, `PropertyInjectionModule`, `TelemetryCorrelationHttpModule`
- **Description:** HTTP Modules do not exist in ASP.NET Core / .NET 8.
- **Recommendation:** Replace with ASP.NET Core Middleware registered in Program.cs.

#### ISSUE-009: EF6 DbModelBuilder API Not Available in EF Core
- **File:** `Models/CatalogDBContext.cs` line 19
- **Severity:** Critical
- **Code:** `protected override void OnModelCreating(DbModelBuilder builder)`
- **Description:** DbModelBuilder is EF6-specific. EF Core uses ModelBuilder.
- **Recommendation:** Rewrite OnModelCreating using EF Core ModelBuilder and IEntityTypeConfiguration<T>.

#### ISSUE-010: EF6 Database.SqlQuery Not Available in EF Core
- **File:** `Models/CatalogItemHiLoGenerator.cs` line 17, `Models/Infrastructure/CatalogDBInitializer.cs` line 131
- **Severity:** Critical
- **Code:** `db.Database.SqlQuery<Int64>("SELECT NEXT VALUE FOR catalog_hilo;")`
- **Description:** Database.SqlQuery<T> is EF6-specific. EF Core uses FromSqlRaw or ExecuteSqlRaw.
- **Recommendation:** Replace with `db.Database.ExecuteSqlRaw()` or use EF Core sequences.

#### ISSUE-011: EF6 Database.ExecuteSqlCommand Not Available in EF Core
- **File:** `Models/Infrastructure/CatalogDBInitializer.cs` line 237
- **Severity:** Critical
- **Code:** `context.Database.ExecuteSqlCommand(File.ReadAllText(scriptFilePath))`
- **Description:** ExecuteSqlCommand is EF6-specific. EF Core uses ExecuteSqlRaw.
- **Recommendation:** Replace with `context.Database.ExecuteSqlRaw()`.

#### ISSUE-012: EF6 CreateDatabaseIfNotExists Initializer Not Available in EF Core
- **File:** `Models/Infrastructure/CatalogDBInitializer.cs` line 10
- **Severity:** Critical
- **Code:** `public class CatalogDBInitializer : CreateDatabaseIfNotExists<CatalogDBContext>`
- **Description:** EF6 database initializers (CreateDatabaseIfNotExists, DropCreateDatabaseIfModelChanges) do not exist in EF Core.
- **Recommendation:** Use EF Core Migrations with `context.Database.EnsureCreated()` or `context.Database.Migrate()`.

---

### HIGH Issues

#### ISSUE-013: log4net Not Recommended for .NET 8
- **File:** `Global.asax.cs` line 6, `Default.aspx.cs` line 6, `Catalog/Create.aspx.cs` line 4, `Catalog/Edit.aspx.cs` line 4, `Catalog/Delete.aspx.cs` line 4, `Catalog/Details.aspx.cs` line 4
- **Severity:** High
- **Code:** `using log4net;` / `private static readonly ILog _log = LogManager.GetLogger(...)`
- **Description:** log4net is a legacy logging framework. .NET 8 uses Microsoft.Extensions.Logging.
- **Recommendation:** Replace ILog with ILogger<T> injected via DI. Configure Serilog.AspNetCore 8.0.0 in Program.cs.

#### ISSUE-014: ConfigurationManager Not Available in .NET 8
- **File:** `Global.asax.cs` line 7, `Models/Infrastructure/CatalogDBInitializer.cs` line 6
- **Severity:** High
- **Code:** `using System.Configuration;` / `ConfigurationManager.AppSettings["UseMockData"]`
- **Description:** System.Configuration.ConfigurationManager reads from Web.config which is not used in .NET 8.
- **Recommendation:** Use IConfiguration with appsettings.json and the Options pattern.

#### ISSUE-015: HostingEnvironment.ApplicationPhysicalPath Not Available
- **File:** `Models/Infrastructure/CatalogDBInitializer.cs` lines 87, 107, 127
- **Severity:** High
- **Code:** `var contentRootPath = HostingEnvironment.ApplicationPhysicalPath;`
- **Description:** System.Web.Hosting.HostingEnvironment is not available in .NET 8.
- **Recommendation:** Inject IWebHostEnvironment and use ContentRootPath or WebRootPath.

#### ISSUE-016: Response.Redirect Not Available in .NET 8
- **File:** `Catalog/Create.aspx.cs` line 38, `Catalog/Edit.aspx.cs` line 52, `Catalog/Delete.aspx.cs` line 30
- **Severity:** High
- **Code:** `Response.Redirect("~");`
- **Description:** System.Web.HttpResponse.Redirect is not available in .NET 8.
- **Recommendation:** Use RedirectToPage() in Razor Pages or Redirect() in MVC controllers.

#### ISSUE-017: System.Web.Routing.RouteCollection Not Available
- **File:** `App_Start/RouteConfig.cs` line 1
- **Severity:** High
- **Code:** `using System.Web.Routing;` / `routes.MapPageRoute(...)`
- **Description:** System.Web.Routing and MapPageRoute are not available in .NET 8.
- **Recommendation:** Use ASP.NET Core endpoint routing configured in Program.cs.

#### ISSUE-018: System.Web.Optimization (Bundling) Not Available
- **File:** `App_Start/BundleConfig.cs`, `Site.Master`
- **Severity:** High
- **Code:** `using System.Web.Optimization;` / `BundleTable.Bundles`, `ScriptBundle`, `StyleBundle`
- **Description:** Microsoft.AspNet.Web.Optimization is not available in .NET 8.
- **Recommendation:** Use ASP.NET Core bundling (WebOptimizer) or reference CDN/static files directly.

#### ISSUE-019: ScriptManager / MsAjax Not Available in .NET 8
- **File:** `Site.Master` lines 16–35
- **Severity:** High
- **Code:** `<asp:ScriptManager runat="server">` / `<asp:ScriptReference Name="MsAjaxBundle" />`
- **Description:** ASP.NET Web Forms ScriptManager and MS Ajax are not available in .NET 8.
- **Recommendation:** Use standard HTML script tags or a modern JavaScript bundler.

#### ISSUE-020: ViewState and Postback Pattern Not Available
- **File:** `Catalog/Edit.aspx.cs` line 18
- **Severity:** High
- **Code:** `if (!Page.IsPostBack)`
- **Description:** ViewState and the postback model do not exist in .NET 8.
- **Recommendation:** Use Razor Pages model binding with OnGet/OnPost handlers.

#### ISSUE-021: Server Controls (asp:ListView, asp:HyperLink, etc.) Not Available
- **File:** `Default.aspx`
- **Severity:** High
- **Code:** `<asp:ListView ID="productList" ...>`, `<asp:HyperLink ...>`, `<asp:PlaceHolder ...>`
- **Description:** ASP.NET Web Forms server controls are not available in .NET 8.
- **Recommendation:** Replace with Razor syntax (@foreach, <a asp-page>, Tag Helpers).

#### ISSUE-022: EF6 EntityTypeConfiguration<T> API Not Available in EF Core
- **File:** `Models/CatalogDBContext.cs` lines 30–75
- **Severity:** High
- **Code:** `void ConfigureCatalogItem(EntityTypeConfiguration<CatalogItem> builder)` / `builder.HasRequired<CatalogBrand>(...).WithMany().HasForeignKey(...)`
- **Description:** EntityTypeConfiguration<T> and HasRequired/WithMany fluent API differ in EF Core.
- **Recommendation:** Implement IEntityTypeConfiguration<T> with EF Core fluent API (HasOne/WithMany/IsRequired).

#### ISSUE-023: EF6 DatabaseGeneratedOption.None via HasDatabaseGeneratedOption
- **File:** `Models/CatalogDBContext.cs` line 55
- **Severity:** High
- **Code:** `.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)`
- **Description:** HasDatabaseGeneratedOption is EF6-specific.
- **Recommendation:** Use `.ValueGeneratedNever()` in EF Core.

#### ISSUE-024: Autofac.Integration.Web.Forms.PropertyInjectionModule
- **File:** `Web.config` line 48, `Global.asax.cs`
- **Severity:** High
- **Code:** `type="Autofac.Integration.Web.Forms.PropertyInjectionModule, Autofac.Integration.Web"`
- **Description:** Property injection via HTTP modules is a Web Forms pattern not available in .NET 8.
- **Recommendation:** Use constructor injection with built-in ASP.NET Core DI.

#### ISSUE-025: Microsoft.AspNet.FriendlyUrls Not Available in .NET 8
- **File:** `ViewSwitcher.ascx.cs` line 8
- **Severity:** High
- **Code:** `using Microsoft.AspNet.FriendlyUrls.Resolvers;` / `WebFormsFriendlyUrlResolver.IsMobileView(...)`
- **Description:** Microsoft.AspNet.FriendlyUrls is a Web Forms-specific package not available in .NET 8.
- **Recommendation:** Remove ViewSwitcher user control; use responsive design (Bootstrap) instead.

#### ISSUE-026: Non-SDK-Style Project File
- **File:** `eShopLegacyWebForms.csproj`
- **Severity:** High
- **Code:** `<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="...">`
- **Description:** The project uses the old non-SDK-style format which is not compatible with .NET 8.
- **Recommendation:** Convert to SDK-style project: `<Project Sdk="Microsoft.NET.Sdk.Web">`.

---

### MEDIUM Issues

#### ISSUE-027: Session State Usage
- **File:** `Global.asax.cs` lines 37–40, `Site.Master.cs` line 14
- **Severity:** Medium
- **Code:** `HttpContext.Current.Session["MachineName"] = Environment.MachineName;`
- **Description:** Session state works differently in ASP.NET Core. InProc session requires explicit configuration.
- **Recommendation:** Configure session in Program.cs with `builder.Services.AddSession()` and use ISession via IHttpContextAccessor.

#### ISSUE-028: EF6 DbContext Constructor with Connection String Name
- **File:** `Models/CatalogDBContext.cs` line 8
- **Severity:** Medium
- **Code:** `public CatalogDBContext() : base("name=CatalogDBContext")`
- **Description:** EF Core DbContext does not accept connection string names in the constructor.
- **Recommendation:** Use DbContextOptions<T> with connection string from IConfiguration.

#### ISSUE-029: EF6 DbContext.Entry().State = EntityState.Modified
- **File:** `Services/CatalogService.cs` line 52
- **Severity:** Medium
- **Code:** `db.Entry(catalogItem).State = EntityState.Modified;`
- **Description:** While EF Core supports this pattern, it's better to use Update() or track changes properly.
- **Recommendation:** Use `db.CatalogItems.Update(catalogItem)` in EF Core.

#### ISSUE-030: packages.config Format Not Supported
- **File:** `packages.config`
- **Severity:** Medium
- **Code:** `<packages>` XML format
- **Description:** packages.config is the old NuGet format. .NET 8 SDK-style projects use PackageReference in .csproj.
- **Recommendation:** Migrate all package references to `<PackageReference>` elements in the .csproj file.

#### ISSUE-031: ApplicationInsights Web SDK Not Compatible
- **File:** `packages.config`, `ApplicationInsights.config`, `Web.config`
- **Severity:** Medium
- **Code:** `Microsoft.ApplicationInsights.Web 2.9.1`
- **Description:** The Web Forms ApplicationInsights SDK is not compatible with .NET 8.
- **Recommendation:** Use Microsoft.ApplicationInsights.AspNetCore 2.22.0 for .NET 8.

#### ISSUE-032: WebGrease / Antlr3 Legacy Dependencies
- **File:** `packages.config`, `eShopLegacyWebForms.csproj`
- **Severity:** Medium
- **Code:** `WebGrease 1.6.0`, `Antlr 3.5.0.2`
- **Description:** WebGrease and Antlr3 are dependencies of the Web Optimization framework and are not needed in .NET 8.
- **Recommendation:** Remove these packages entirely.

#### ISSUE-033: Master Page Pattern Must Be Replaced
- **File:** `Site.Master`, `Site.Mobile.Master`
- **Severity:** Medium
- **Code:** `<%@ master language="C#" ... inherits="eShopLegacyWebForms.SiteMaster" %>`
- **Description:** Master pages are a Web Forms concept. .NET 8 Razor Pages use Layout pages.
- **Recommendation:** Convert to `_Layout.cshtml` in Pages/Shared folder.

#### ISSUE-034: User Control (ViewSwitcher.ascx) Must Be Replaced
- **File:** `ViewSwitcher.ascx`, `ViewSwitcher.ascx.cs`
- **Severity:** Medium
- **Code:** `<%@ Control Language="C#" ... Inherits="eShopLegacyWebForms.ViewSwitcher" %>`
- **Description:** ASP.NET Web Forms user controls (.ascx) are not available in .NET 8.
- **Recommendation:** Replace with Razor partial views or View Components.

#### ISSUE-035: Designer Files Must Be Removed
- **File:** All `.aspx.designer.cs`, `.master.designer.cs`, `.ascx.designer.cs` files
- **Severity:** Medium
- **Code:** Auto-generated designer files
- **Description:** Designer files are Web Forms-specific and have no equivalent in .NET 8.
- **Recommendation:** Remove all designer files; Razor Pages use model binding instead.

#### ISSUE-036: AppDomain.CurrentDomain.BaseDirectory
- **File:** `Models/Infrastructure/CatalogDBInitializer.cs` line 237
- **Severity:** Medium
- **Code:** `var scriptFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptFile);`
- **Description:** While AppDomain exists in .NET 8, using IWebHostEnvironment.ContentRootPath is the recommended approach.
- **Recommendation:** Inject IWebHostEnvironment and use ContentRootPath.

---

### LOW Issues

#### ISSUE-037: Modernizr 2.8.3 Outdated
- **File:** `packages.config`, `App_Start/BundleConfig.cs`
- **Severity:** Low
- **Code:** `Modernizr 2.8.3`
- **Description:** Modernizr 2.8.3 is outdated. Modern browsers have good HTML5 support.
- **Recommendation:** Remove Modernizr or update to latest version via npm/CDN.

#### ISSUE-038: Respond.js Not Needed for Modern Browsers
- **File:** `App_Start/BundleConfig.cs`, `Site.Master`
- **Severity:** Low
- **Code:** `respond.min.js`
- **Description:** Respond.js is a polyfill for IE8 CSS3 media queries. Not needed for modern browsers.
- **Recommendation:** Remove Respond.js.

#### ISSUE-039: jQuery 3.3.1 Should Be Updated
- **File:** `packages.config`
- **Severity:** Low
- **Code:** `jQuery 3.5.0`
- **Description:** jQuery 3.3.1/3.5.0 has known security vulnerabilities. Should be updated.
- **Recommendation:** Update to jQuery 3.7.x via CDN or npm.

#### ISSUE-040: Newtonsoft.Json Should Be Replaced with System.Text.Json
- **File:** `packages.config`
- **Severity:** Low
- **Code:** `Newtonsoft.Json 12.0.1`
- **Description:** .NET 8 includes System.Text.Json as the default JSON serializer.
- **Recommendation:** Replace with System.Text.Json or update Newtonsoft.Json to 13.x if needed.

#### ISSUE-041: AssemblyInfo.cs Attributes Redundant in SDK-Style Projects
- **File:** `Properties/AssemblyInfo.cs`
- **Severity:** Low
- **Code:** Assembly attributes
- **Description:** SDK-style projects auto-generate assembly attributes. Manual AssemblyInfo.cs may cause duplicates.
- **Recommendation:** Remove or simplify AssemblyInfo.cs when migrating to SDK-style project.

#### ISSUE-042: TypeScript Tools Version Outdated
- **File:** `eShopLegacyWebForms.csproj`
- **Severity:** Low
- **Code:** `<TypeScriptToolsVersion>3.2</TypeScriptToolsVersion>`
- **Description:** TypeScript 3.2 is outdated. Current version is 5.x.
- **Recommendation:** Update TypeScript or remove if not actively used.

---

## Migration Roadmap

### Phase 1: Project Structure (Week 1)
1. Create new SDK-style solution with clean architecture layers
2. Set up Domain, Application, Infrastructure, Web projects targeting net8.0
3. Migrate entity models to Domain layer

### Phase 2: Data Access (Week 1-2)
1. Replace EF6 with EF Core 8.0.0
2. Rewrite CatalogDBContext using EF Core ModelBuilder
3. Implement IEntityTypeConfiguration<T> for each entity
4. Replace HiLo generator with EF Core sequences
5. Migrate database initializer to EF Core migrations

### Phase 3: Business Logic (Week 2)
1. Replace log4net with ILogger<T>
2. Replace ConfigurationManager with IConfiguration
3. Migrate CatalogService to async/await pattern
4. Replace Autofac with built-in DI

### Phase 4: Web Layer (Week 3-4)
1. Create Razor Pages for each .aspx page
2. Migrate code-behind logic to PageModel handlers
3. Replace Master Pages with _Layout.cshtml
4. Replace server controls with Tag Helpers
5. Configure routing in Program.cs
6. Migrate Web.config to appsettings.json
7. Replace HTTP Modules with Middleware

### Phase 5: Testing & Verification (Week 5)
1. Write unit tests for services
2. Write integration tests for repositories
3. Verify all CRUD operations
4. Performance testing

---

## File Migration Mapping

| Legacy File | Target File | Complexity |
|---|---|---|
| Default.aspx + .cs | Pages/Index.cshtml + .cs | Medium |
| Catalog/Create.aspx + .cs | Pages/Catalog/Create.cshtml + .cs | Medium |
| Catalog/Edit.aspx + .cs | Pages/Catalog/Edit.cshtml + .cs | Medium |
| Catalog/Details.aspx + .cs | Pages/Catalog/Details.cshtml + .cs | Simple |
| Catalog/Delete.aspx + .cs | Pages/Catalog/Delete.cshtml + .cs | Simple |
| About.aspx + .cs | Pages/About.cshtml + .cs | Simple |
| Contact.aspx + .cs | Pages/Contact.cshtml + .cs | Simple |
| Site.Master + .cs | Pages/Shared/_Layout.cshtml | Medium |
| Site.Mobile.Master | Remove (use responsive CSS) | Simple |
| ViewSwitcher.ascx + .cs | Remove (use Bootstrap responsive) | Simple |
| Global.asax + .cs | Program.cs | Complex |
| Web.config | appsettings.json | Medium |
| App_Start/RouteConfig.cs | Program.cs (endpoint routing) | Medium |
| App_Start/BundleConfig.cs | Program.cs (static files) | Medium |
| Models/CatalogDBContext.cs | Infrastructure/Data/CatalogDbContext.cs | Complex |
| Models/CatalogItem.cs | Domain/Entities/CatalogItem.cs | Simple |
| Models/CatalogBrand.cs | Domain/Entities/CatalogBrand.cs | Simple |
| Models/CatalogType.cs | Domain/Entities/CatalogType.cs | Simple |
| Models/Infrastructure/CatalogDBInitializer.cs | Infrastructure/Data/CatalogDbInitializer.cs | Complex |
| Services/CatalogService.cs | Application/Services/CatalogService.cs | Medium |
| Services/ICatalogService.cs | Domain/Interfaces/Services/ICatalogService.cs | Simple |
| Modules/ApplicationModule.cs | Web/Program.cs (DI registration) | Medium |
| ViewModel/PaginatedItemsViewModel.cs | Application/DTOs/PaginatedItemsDto.cs | Simple |
