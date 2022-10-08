using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Brands.Queries;
public class SearchBrandsQuery : SearchQuery {}

public class SearchBrandsQueryHandler : IRequestHandler<SearchBrandsQuery, IResult>
{
    private readonly ICrudService _crudService;
    public SearchBrandsQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        SearchBrandsQuery request,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
            result = Results.Ok(
                _crudService.QueryPage<Brand, BrandDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber)
            );
        else
            result = Results.Ok(
                _crudService.Query<Brand, BrandDto>(request.Where, request.Select, request.OrderBy)
            );

        return await Task.FromResult(result);
    }

}
