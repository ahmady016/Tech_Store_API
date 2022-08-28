using AutoMapper;

using Dtos;
using Entities;

using Brands.Commands;
using Products.Commands;
namespace Common;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, AddProductCommand>().ReverseMap();
        CreateMap<Product, UpdateProductCommand>().ReverseMap();

        CreateMap<Brand, BrandDto>().ReverseMap();
        CreateMap<Brand, AddBrandCommand>().ReverseMap();
        CreateMap<Brand, UpdateBrandCommand>().ReverseMap();

    }
}
