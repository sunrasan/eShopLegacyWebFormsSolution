# Architecture Documentation

## Clean Architecture Overview

The solution follows Clean Architecture principles with clear separation of concerns.

## Layer Responsibilities

### Domain Layer (`eShopLegacyWebForms.Domain`)
- **Entities**: `CatalogItem`, `CatalogBrand`, `CatalogType`
- **Interfaces**: Repository and service contracts
- **Exceptions**: Domain-specific exceptions
- **No dependencies** on other layers

### Application Layer (`eShopLegacyWebForms.Application`)
- **Services**: Business logic implementation (`CatalogService`, `CatalogServiceMock`)
- **DTOs**: Data transfer objects for cross-layer communication
- **Mappings**: AutoMapper profiles
- **Extensions**: DI registration
- **Depends on**: Domain layer only

### Infrastructure Layer (`eShopLegacyWebForms.Infrastructure`)
- **DbContext**: `CatalogDbContext` (EF Core)
- **Configurations**: Entity type configurations
- **Repositories**: EF Core repository implementations
- **Extensions**: DI registration
- **Depends on**: Domain and Application layers

### Web Layer (`eShopLegacyWebForms.Web`)
- **Pages**: Razor Pages (Index, Create, Details, Edit, Delete)
- **ViewModels**: UI-specific view models (manually mapped from DTOs)
- **Program.cs**: Application startup and DI configuration
- **Depends on**: Application and Infrastructure layers

## Dependency Flow

```
Web → Application → Domain
Web → Infrastructure → Domain
Infrastructure → Application → Domain
```

## Key Design Decisions

1. **ViewModels are manually mapped** - No AutoMapper in Web layer
2. **Mock service** - `CatalogServiceMock` for development without database
3. **Repository pattern** - Abstracts data access from business logic
4. **Async throughout** - All I/O operations are async
