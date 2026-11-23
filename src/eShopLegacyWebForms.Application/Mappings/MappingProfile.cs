using AutoMapper;
using eShopLegacyWebForms.Application.DTOs;
using eShopLegacyWebForms.Domain.Entities;

namespace eShopLegacyWebForms.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // CatalogItem mappings
        CreateMap<CatalogItem, CatalogItemDto>()
            .ForMember(dest => dest.CatalogTypeName, opt => opt.MapFrom(src => src.CatalogType != null ? src.CatalogType.Type : string.Empty))
            .ForMember(dest => dest.CatalogBrandName, opt => opt.MapFrom(src => src.CatalogBrand != null ? src.CatalogBrand.Brand : string.Empty));

        CreateMap<CatalogItemCreateDto, CatalogItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CatalogType, opt => opt.Ignore())
            .ForMember(dest => dest.CatalogBrand, opt => opt.Ignore())
            .ForMember(dest => dest.PictureUri, opt => opt.Ignore());

        CreateMap<CatalogItemUpdateDto, CatalogItem>()
            .ForMember(dest => dest.CatalogType, opt => opt.Ignore())
            .ForMember(dest => dest.CatalogBrand, opt => opt.Ignore())
            .ForMember(dest => dest.PictureUri, opt => opt.Ignore());

        // CatalogBrand mappings
        CreateMap<CatalogBrand, CatalogBrandDto>();
        CreateMap<CatalogBrandDto, CatalogBrand>();

        // CatalogType mappings
        CreateMap<CatalogType, CatalogTypeDto>();
        CreateMap<CatalogTypeDto, CatalogType>();
    }
}
