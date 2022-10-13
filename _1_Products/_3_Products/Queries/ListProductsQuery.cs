using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Products.Queries;
public class ListProductsQuery : ListQuery {}

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, IResult> {
    private readonly ICrudService _crudService;
    public ListProductsQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        ListProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _crudService.ListPageAsync<Product, ProductDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await _crudService.ListAsync<Product, ProductDto>(request.ListType);
            return Results.Ok(list);
        }
    }

}
