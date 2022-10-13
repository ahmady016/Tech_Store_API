using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Products.Queries;
public class SearchProductsQuery : SearchQuery {}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IResult>
{
    private readonly ICrudService _crudService;
    public SearchProductsQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _crudService.QueryPageAsync<Product, ProductDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await _crudService.QueryAsync<Product, ProductDto>(request.Where, request.Select, request.OrderBy);
            return Results.Ok(list);
        }
    }

}
