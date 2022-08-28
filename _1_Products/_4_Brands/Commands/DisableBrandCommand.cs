using MediatR;

using DB;
using Common;
using Entities;

namespace Brands.Commands;

public class DisableBrandCommand : IdInput {}

public class DisableBrandCommandHandler : IRequestHandler<DisableBrandCommand, IResult>
{
    private readonly ICrudService _crudService;
    public DisableBrandCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DisableBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        _ = _crudService.Delete<Brand>(command.Id);
        return await Task.FromResult(Results.Ok($"Brand with Id: {command.Id} was disabled successfully"));
    }

}
