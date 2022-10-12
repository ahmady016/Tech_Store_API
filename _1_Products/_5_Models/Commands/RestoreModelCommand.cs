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
        await _crudService.RestoreAsync<Model>(command.Id);
        return Results.Ok($"Model with Id: {command.Id} was restored successfully");
    }

}
