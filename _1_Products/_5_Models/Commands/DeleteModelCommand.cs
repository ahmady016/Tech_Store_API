using MediatR;

using DB;
using Common;
using Entities;

namespace Models.Commands;

public class DeleteModelCommand : IdInput {}

public class DeleteModelCommandHandler : IRequestHandler<DeleteModelCommand, IResult>
{
    private readonly ICrudService _crudService;
    public DeleteModelCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DeleteModelCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.DeleteAsync<Model>(command.Id);
        return Results.Ok($"Model with Id: {command.Id} was deleted successfully");
    }

}
