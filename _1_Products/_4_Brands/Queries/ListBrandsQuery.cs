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
        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
            result = Results.Ok(_crudService.ListPage<Brand, BrandDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber));
        else
            result = Results.Ok(_crudService.List<Brand, BrandDto>(request.ListType));

        return await Task.FromResult(result);
    }

}
