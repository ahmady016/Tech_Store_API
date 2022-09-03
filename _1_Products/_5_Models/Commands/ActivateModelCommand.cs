using MediatR;

using DB;
using Common;
using Entities;

namespace Models.Commands;

public class ActivateModelCommand : IdInput {}

public class ActivateModelCommandHandler : IRequestHandler<ActivateModelCommand, IResult>
{
    private readonly ICrudService _crudService;
    public ActivateModelCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        ActivateModelCommand command,
        CancellationToken cancellationToken
    )
    {
        _ = _crudService.Activate<Model>(command.Id);
        return await Task.FromResult(Results.Ok($"Model with Id: {command.Id} was activated successfully"));
    }

}
