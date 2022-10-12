using MediatR;

using DB;
using Common;
using Entities;

namespace Brands.Commands;

public class DeleteBrandCommand : IdInput {}

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, IResult>
{
    private readonly ICrudService _crudService;
    public DeleteBrandCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DeleteBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.DeleteAsync<Brand>(command.Id);
        return Results.Ok($"Brand with Id: {command.Id} was deleted successfully");
    }

}
