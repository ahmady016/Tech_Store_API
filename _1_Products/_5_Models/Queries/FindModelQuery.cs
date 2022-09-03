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
        var existedModel = _crudService.Find<Model, ModelDto>(request.Id);
        return await Task.FromResult(Results.Ok(existedModel));
    }

}
