using MediatR;

using DB;
using Dtos;
using Entities;

namespace Products.Queries;
public class FindProductQuery : IRequest<IResult>
{
    public Guid Id { get; set; }
}

public class FindProductQueryHandler : IRequestHandler<FindProductQuery, IResult> {
    private readonly ICrudService _crudService;
    public FindProductQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        FindProductQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = Results.Ok(_crudService.Find<Product, ProductDto>(request.Id));
        return await Task.FromResult(result);
    }

}