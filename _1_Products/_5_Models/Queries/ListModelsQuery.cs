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
        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
            result = Results.Ok(_crudService.ListPage<Model, ModelDto>(request.ListType, (int)request.PageSize, (int)request.PageNumber));
        else
            result = Results.Ok(_crudService.List<Model, ModelDto>(request.ListType));

        return await Task.FromResult(result);
    }

}
