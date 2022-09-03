using MediatR;

using DB;
using Common;
using Entities;

namespace Models.Commands;

public class RestoreModelCommand : IdInput {}

public class RestoreModelCommandHandler : IRequestHandler<RestoreModelCommand, IResult>
{
    private readonly ICrudService _crudService;
    public RestoreModelCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        RestoreModelCommand command,
        CancellationToken cancellationToken
    )
    {
        _ = _crudService.Restore<Model>(command.Id);
        return await Task.FromResult(Results.Ok($"Model with Id: {command.Id} was restored successfully"));
    }

}
