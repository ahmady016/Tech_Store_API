using AutoMapper;

using Dtos;
using Entities;

using Auth.Commands;
using Products.Commands;
using Brands.Commands;
using Models.Commands;

namespace Common;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, SignupCommand>().ReverseMap();

        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Role, RoleWithUsersDto>().ReverseMap();

        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, AddProductCommand>().ReverseMap();
        CreateMap<Product, UpdateProductCommand>().ReverseMap();

        CreateMap<Brand, BrandDto>().ReverseMap();
        CreateMap<Brand, AddBrandCommand>().ReverseMap();
        CreateMap<Brand, UpdateBrandCommand>().ReverseMap();

        CreateMap<Model, ModelDto>().ReverseMap();
        CreateMap<Model, AddModelCommand>().ReverseMap();
        CreateMap<Model, UpdateModelCommand>().ReverseMap();

    }
}
