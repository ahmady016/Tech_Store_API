using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Brands.Queries;
public class FindBrandQuery : IdInput {}

public class FindBrandQueryHandler : IRequestHandler<FindBrandQuery, IResult>
{
    private readonly ICrudService _crudService;
    public FindBrandQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        FindBrandQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedBrand = _crudService.Find<Brand, BrandDto>(request.Id);
        return await Task.FromResult(Results.Ok(existedBrand));
    }

}
