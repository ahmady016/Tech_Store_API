using MediatR;

using DB;
using Common;
using Entities;

namespace Products.Commands;

public class DeleteProductCommand : IdInput {}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, IResult>
{
    private readonly ICrudService _crudService;
    public DeleteProductCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.DeleteAsync<Product>(command.Id);
        return Results.Ok($"Product with Id: {command.Id} was deleted successfully");
    }

}
