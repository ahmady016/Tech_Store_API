using AutoMapper;

using Dtos;
using Entities;
using Products.Commands;
namespace Common;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, AddProductCommand>().ReverseMap();
        CreateMap<Product, UpdateProductCommand>().ReverseMap();

    }
}
