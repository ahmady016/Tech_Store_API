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
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _crudService.QueryPageAsync<Brand, BrandDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await _crudService.QueryAsync<Brand, BrandDto>(request.Where, request.Select, request.OrderBy);
            return Results.Ok(list);
        }
    }

}
