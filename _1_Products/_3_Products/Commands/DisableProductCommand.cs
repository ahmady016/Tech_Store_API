using MediatR;

using DB;
using Common;
using Entities;

namespace Products.Commands;

public class DisableProductCommand : IdInput {}

public class DisableProductCommandHandler : IRequestHandler<DisableProductCommand, IResult>
{
    private readonly ICrudService _crudService;
    public DisableProductCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DisableProductCommand command,
        CancellationToken cancellationToken
    )
    {
        _ = _crudService.Disable<Product>(command.Id);
        return await Task.FromResult(Results.Ok($"Product with Id: {command.Id} was disabled successfully"));
    }

}
