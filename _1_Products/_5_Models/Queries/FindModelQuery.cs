using MediatR;

using DB;
using Dtos;
using Entities;
using Common;

namespace Models.Queries;
public class FindModelQuery : IdInput {}

public class FindModelQueryHandler : IRequestHandler<FindModelQuery, IResult>
{
    private readonly ICrudService _crudService;
    public FindModelQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        FindModelQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedModel = await _crudService.FindAsync<Model, ModelDto>(request.Id);
        return Results.Ok(existedModel);
    }

}
