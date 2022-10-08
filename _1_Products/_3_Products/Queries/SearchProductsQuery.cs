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
        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
            result = Results.Ok(
                _crudService.QueryPage<Product, ProductDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber)
            );
        else
            result = Results.Ok(
                _crudService.Query<Product, ProductDto>(request.Where, request.Select, request.OrderBy)
            );

        return await Task.FromResult(result);
    }

}
