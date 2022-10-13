using MediatR;

using DB;
using Dtos;
using Entities;

namespace Products.Queries;
public class FindProductsQuery : IRequest<IResult>
{
    public string Ids { get; set; }
}

public class FindProductsQueryHandler : IRequestHandler<FindProductsQuery, IResult> {
    private readonly ICrudService _crudService;
    public FindProductsQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        FindProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        var products = await _crudService.FindListAsync<Product, ProductDto>(request.Ids);
        return Results.Ok(products);
    }

}
