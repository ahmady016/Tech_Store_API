using AutoMapper;

using Dtos;
using Entities;

using Products.Commands;
using Brands.Commands;
using Models.Commands;

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

        CreateMap<Model, ModelDto>().ReverseMap();
        CreateMap<Brand, AddModelCommand>().ReverseMap();
        CreateMap<Brand, UpdateModelCommand>().ReverseMap();

    }
}
