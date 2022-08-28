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
        _ = _crudService.Delete<Brand>(command.Id);
        return await Task.FromResult(Results.Ok($"Brand with Id: {command.Id} was deleted successfully"));
    }

}
