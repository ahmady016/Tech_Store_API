using MediatR;

using DB;
using Common;
using Entities;

namespace Products.Commands;

public class ActivateProductCommand : IdInput {}

public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, IResult>
{
    private readonly ICrudService _crudService;
    public ActivateProductCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        ActivateProductCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.ActivateAsync<Product>(command.Id);
        return Results.Ok($"Product with Id: {command.Id} was activated successfully");
    }

}
