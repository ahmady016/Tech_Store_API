using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Products.Queries;
public class FindProductQuery : IdInput {}

public class FindProductQueryHandler : IRequestHandler<FindProductQuery, IResult>
{
    private readonly ICrudService _crudService;
    public FindProductQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        FindProductQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedProduct = await _crudService.FindAsync<Product, ProductDto>(request.Id);
        return Results.Ok(existedProduct);
    }

}
