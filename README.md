# eShopLegacyWebForms - Migrated to .NET 8

## Overview

This application has been migrated from **ASP.NET Web Forms 4.7.2** to **.NET 8** using clean architecture principles.

## Architecture

The solution follows **Clean Architecture** with four layers:

```
eShopLegacyWebForms/
├── src/
│   ├── eShopLegacyWebForms.Domain/          # Domain entities, interfaces
│   ├── eShopLegacyWebForms.Application/     # Business logic, services, DTOs
│   ├── eShopLegacyWebForms.Infrastructure/  # EF Core, repositories
│   └── eShopLegacyWebForms.Web/             # Razor Pages, ViewModels
├── tests/
│   ├── eShopLegacyWebForms.UnitTests/       # Unit tests
│   └── eShopLegacyWebForms.IntegrationTests/ # Integration tests
└── docs/                                     # Documentation
```

## Prerequisites

- .NET 8 SDK
- SQL Server (optional - mock data mode available)

## Setup

1. Clone the repository
2. Navigate to the solution directory
3. Run `dotnet restore`
4. Configure `appsettings.json` (or use mock data mode)
5. Run `dotnet run --project src/eShopLegacyWebForms.Web`

## Configuration

### Mock Data Mode (Default)
Set `"UseMockData": true` in `appsettings.json` to use in-memory mock data.

### Database Mode
Set `"UseMockData": false` and configure the connection string:
```json
{
  "UseMockData": false,
  "ConnectionStrings": {
    "CatalogDbContext": "Your SQL Server connection string"
  }
}
```

## Running the Application

```bash
cd src/eShopLegacyWebForms.Web
dotnet run
```

## Running Tests

```bash
dotnet test
```

## Migration Summary

| Old (Web Forms 4.7.2) | New (.NET 8) |
|----------------------|--------------|
| ASP.NET Web Forms | Razor Pages |
| Entity Framework 6 | EF Core 8 |
| log4net | Serilog + Microsoft.Extensions.Logging |
| Autofac | Built-in .NET DI |
| Web.config | appsettings.json |
| Global.asax | Program.cs |
| packages.config | SDK-style .csproj |
| System.Web | ASP.NET Core |
