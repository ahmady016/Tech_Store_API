using MediatR;

using DB;
using Common;
using Entities;

namespace Products.Commands;

public class RestoreProductCommand : IdInput {}

public class RestoreProductCommandHandler : IRequestHandler<RestoreProductCommand, IResult>
{
    private readonly ICrudService _crudService;
    public RestoreProductCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        RestoreProductCommand command,
        CancellationToken cancellationToken
    )
    {
        _ = _crudService.Restore<Product>(command.Id);
        return await Task.FromResult(Results.Ok($"Product with Id: {command.Id} was restored successfully"));
    }

}
