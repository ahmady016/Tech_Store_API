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
        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
            result = Results.Ok(
                _crudService.QueryPage<Model, ModelDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber)
            );
        else
            result = Results.Ok(
                _crudService.Query<Model, ModelDto>(request.Where, request.Select, request.OrderBy)
            );

        return await Task.FromResult(result);
    }

}
