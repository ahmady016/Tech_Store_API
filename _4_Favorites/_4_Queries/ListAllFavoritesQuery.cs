using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Favorites.Queries;

public class ListAllFavoritesQuery : IRequest<IResult>
{
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}

public class ListAllFavoritesQueryHandler : IRequestHandler<ListAllFavoritesQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    public ListAllFavoritesQueryHandler(IDBQueryService dbQueryService)
    {
        _dbQueryService = dbQueryService;
    }

    public async Task<IResult> Handle(
        ListAllFavoritesQuery query,
        CancellationToken cancellationToken
    )
    {
        var dbQuery = _dbQueryService
            .GetQuery<CustomerFavoriteModel>()
            .Include("Customer")
            .Include("Model")
            .Include("Model.Product")
            .Include("Model.Brand")
            .Select(e => new CustomerFavoriteModelDto()
            {
                FullName = $"{e.Customer.FirstName} {e.Customer.LastName}",
                Email = e.Customer.Email,
                BirthDate = e.Customer.BirthDate,
                Gender = e.Customer.Gender,
                Title = e.Model.Title,
                Description = e.Model.Description,
                ThumbUrl = e.Model.ThumbUrl,
                Category = e.Model.Category,
                ProductId = e.Model.ProductId,
                ProductTitle = e.Model.Product.Title,
                BrandId = e.Model.BrandId,
                BrandTitle = e.Model.Brand.Title
            });

        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<CustomerFavoriteModelDto>(dbQuery, (int)query.PageSize, (int)query.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await dbQuery.ToListAsync();
            return Results.Ok(list);
        }
    }
}
