using MediatR;

using DB;
using Common;
using Entities;

namespace Models.Commands;

public class DisableModelCommand : IdInput {}

public class DisableModelCommandHandler : IRequestHandler<DisableModelCommand, IResult>
{
    private readonly ICrudService _crudService;
    public DisableModelCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DisableModelCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.DisableAsync<Model>(command.Id);
        return Results.Ok($"Model with Id: {command.Id} was disabled successfully");
    }

}
