using MediatR;

using DB;
using Dtos;
using Entities;

namespace Models.Queries;
public class FindModelsQuery : IRequest<IResult>
{
    public string Ids { get; set; }
}

public class FindModelsQueryHandler : IRequestHandler<FindModelsQuery, IResult> {
    private readonly ICrudService _crudService;
    public FindModelsQueryHandler (ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle (
        FindModelsQuery request,
        CancellationToken cancellationToken
    )
    {
        var models = await _crudService.FindListAsync<Model, ModelDto>(request.Ids);
        return Results.Ok(models);
    }

}
