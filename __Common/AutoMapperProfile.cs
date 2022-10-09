using System.Reflection.Metadata;
using AutoMapper;

using Dtos;
using Entities;

using Auth.Commands;
using Products.Commands;
using Brands.Commands;
using Models.Commands;
using Comments.Commands;
using Replies.Commands;
using Purchases.Commands;
using Sales.Commands;

namespace Common;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, SignupCommand>().ReverseMap();
        CreateMap<dynamic, UserDto>().ReverseMap();

        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Role, RoleWithUsersDto>().ReverseMap();
        CreateMap<dynamic, RoleDto>().ReverseMap();

        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, AddProductCommand>().ReverseMap();

        CreateMap<Brand, BrandDto>().ReverseMap();
        CreateMap<Brand, AddBrandCommand>().ReverseMap();

        CreateMap<Model, ModelDto>().ReverseMap();
        CreateMap<Model, AddModelCommand>().ReverseMap();

        CreateMap<Purchase, PurchaseDto>().ReverseMap();
        CreateMap<Purchase, CreatePurchaseWithItemsCommand>().ReverseMap();

        CreateMap<PurchaseItem, PurchaseItemDto>().ReverseMap();
        CreateMap<PurchaseItem, CreatePurchaseItemCommand>().ReverseMap();
        CreateMap<PurchaseItem, DetailedPurchaseItemDto>()
            .ForMember(dest => dest.EmployeeId, action => action.MapFrom(src => src.Purchase != null ? src.Purchase.EmployeeId : String.Empty))
            .ForMember(dest => dest.PurchasedAt, action => action.MapFrom(src => src.Purchase != null ? src.Purchase.PurchasedAt : DateTime.MinValue))
            .ForMember(dest => dest.ModelTitle, action => action.MapFrom(src => src.Model != null ? src.Model.Title : String.Empty))
            .ReverseMap();

        CreateMap<Sale, SaleDto>().ReverseMap();
        CreateMap<Sale, CreateSaleWithItemsCommand>().ReverseMap();

        CreateMap<SaleItem, SaleItemDto>().ReverseMap();
        CreateMap<SaleItem, CreateSaleItemCommand>().ReverseMap();
        CreateMap<SaleItem, DetailedSaleItemDto>()
            .ForMember(dest => dest.EmployeeId, action => action.MapFrom(src => src.Sale != null ? src.Sale.EmployeeId : String.Empty))
            .ForMember(dest => dest.SoldAt, action => action.MapFrom(src => src.Sale != null ? src.Sale.SoldAt : DateTime.MinValue))
            .ForMember(dest => dest.ModelTitle, action => action.MapFrom(src => src.Model != null ? src.Model.Title : String.Empty))
            .ReverseMap();

        CreateMap<Stock, StockDto>().ReverseMap();
        CreateMap<Stock, DetailedStockDto>()
            .ForMember(dest => dest.ModelTitle, action => action.MapFrom(src => src.Model != null ? src.Model.Title : String.Empty))
            .ForMember(dest => dest.ModelDescription, action => action.MapFrom(src => src.Model != null ? src.Model.Description : String.Empty))
            .ForMember(dest => dest.ModelThumbUrl, action => action.MapFrom(src => src.Model != null ? src.Model.ThumbUrl : null))
            .ForMember(dest => dest.ProductId, action => action.MapFrom(src => src.Model != null ? src.Model.ProductId : Guid.NewGuid()))
            .ForMember(dest => dest.ProductTitle, action => action.MapFrom(src => src.Model != null && src.Model.Product != null ? src.Model.Product.Title : String.Empty))
            .ForMember(dest => dest.ProductCategory, action => action.MapFrom(src => src.Model != null && src.Model.Product != null ? src.Model.Product.Category : Category.PCs))
            .ForMember(dest => dest.BrandId, action => action.MapFrom(src => src.Model != null ? src.Model.BrandId : Guid.NewGuid()))
            .ForMember(dest => dest.BrandTitle, action => action.MapFrom(src => src.Model != null && src.Model.Brand != null ? src.Model.Brand.Title : String.Empty))
            .ReverseMap();

        CreateMap<Comment, CommentDto>().ReverseMap();
        CreateMap<Comment, AddCommentCommand>().ReverseMap();

        CreateMap<Reply, ReplyDto>().ReverseMap();
        CreateMap<Reply, AddReplyCommand>().ReverseMap();

        CreateMap<CustomerFavoriteModel, CustomerFavoriteModelDto>()
            .ForMember(dest => dest.FullName, action => action.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : String.Empty))
            .ForMember(dest => dest.Email, action => action.MapFrom(src => src.Customer != null ? src.Customer.Email : String.Empty))
            .ForMember(dest => dest.BirthDate, action => action.MapFrom(src => src.Customer != null ? src.Customer.BirthDate : DateTime.MinValue))
            .ForMember(dest => dest.Gender, action => action.MapFrom(src => src.Customer != null ? src.Customer.Gender : Gender.Male))
            .ForMember(dest => dest.Title, action => action.MapFrom(src => src.Model != null ? src.Model.Title : String.Empty))
            .ForMember(dest => dest.Description, action => action.MapFrom(src => src.Model != null ? src.Model.Description : String.Empty))
            .ForMember(dest => dest.ThumbUrl, action => action.MapFrom(src => src.Model != null ? src.Model.ThumbUrl : null))
            .ForMember(dest => dest.Category, action => action.MapFrom(src => src.Model != null ? src.Model.Category : Category.PCs))
            .ForMember(dest => dest.ProductId, action => action.MapFrom(src => src.Model != null ? src.Model.ProductId : Guid.NewGuid()))
            .ForMember(dest => dest.ProductTitle, action => action.MapFrom(src => src.Model != null && src.Model.Product != null ? src.Model.Product.Title : String.Empty))
            .ForMember(dest => dest.BrandId, action => action.MapFrom(src => src.Model != null ? src.Model.BrandId : Guid.NewGuid()))
            .ForMember(dest => dest.BrandTitle, action => action.MapFrom(src => src.Model != null && src.Model.Brand != null ? src.Model.Brand.Title : String.Empty))
            .ReverseMap();

    }
}
