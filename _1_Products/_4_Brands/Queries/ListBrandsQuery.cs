using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Brands.Queries;
public class ListBrandsQuery : ListQuery {}

public class ListBrandsQueryHandler : IRequestHandler<ListBrandsQuery, IResult> {
    private readonly ICrudService _crudService;
    public ListBrandsQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        ListBrandsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _crudService.ListPageAsync<Brand, BrandDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await _crudService.ListAsync<Brand, BrandDto>(request.ListType);
            return Results.Ok(list);
        }
    }

}
