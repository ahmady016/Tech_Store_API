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
        IResult result;
        if (request.PageSize is not null && request.PageSize is not null)
            result = Results.Ok(_crudService.ListPage<Product, ProductDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber));
        else
            result = Results.Ok(_crudService.List<Product, ProductDto>(request.ListType));

        return await Task.FromResult(result);
    }

}
