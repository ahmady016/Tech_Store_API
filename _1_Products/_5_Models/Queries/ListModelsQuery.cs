using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Models.Queries;
public class ListModelsQuery : ListQuery {}

public class ListModelsQueryHandler : IRequestHandler<ListModelsQuery, IResult> {
    private readonly ICrudService _crudService;
    public ListModelsQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        ListModelsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _crudService.ListPageAsync<Model, ModelDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await _crudService.ListAsync<Model, ModelDto>(request.ListType);
            return Results.Ok(list);
        }
    }

}
