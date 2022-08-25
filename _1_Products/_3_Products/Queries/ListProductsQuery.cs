using DB;
using Dtos;
using Entities;
using MediatR;

namespace Products.Queries;
public class ListProductsQuery : IRequest<IResult>
{
    public string ListType { get; set; } = "existed";
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}

public class ListProductQueryHandler : IRequestHandler<ListProductsQuery, IResult> {
    private readonly ICrudService _crudService;
    public ListProductQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        ListProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if(request is null)
            result = Results.Ok(_crudService.List<Product, ProductDto>());
        else if (request.PageSize is not null && request.PageSize is not null)
            result = Results.Ok(_crudService.ListPage<Product, ProductDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber));
        else
            result = Results.Ok(_crudService.List<Product, ProductDto>(request.ListType));

        return await Task.FromResult(result);
    }

}
