using MediatR;

using DB;
using Dtos;
using Entities;

namespace Brands.Queries;
public class FindBrandsQuery : IRequest<IResult>
{
    public string Ids { get; set; }
}

public class FindBrandsQueryHandler : IRequestHandler<FindBrandsQuery, IResult> {
    private readonly ICrudService _crudService;
    public FindBrandsQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        FindBrandsQuery request,
        CancellationToken cancellationToken
    )
    {
        var brands = await _crudService.FindListAsync<Brand, BrandDto>(request.Ids);
        return Results.Ok(brands);
    }

}
