using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Models.Queries;
public class SearchModelsQuery : SearchQuery {}

public class SearchModelsQueryHandler : IRequestHandler<SearchModelsQuery, IResult>
{
    private readonly ICrudService _crudService;
    public SearchModelsQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        SearchModelsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _crudService.QueryPageAsync<Model, ModelDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await _crudService.QueryAsync<Model, ModelDto>(request.Where, request.Select, request.OrderBy);
            return Results.Ok(list);
        }
    }

}
