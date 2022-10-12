using MediatR;

using DB;
using Common;
using Entities;

namespace Brands.Commands;

public class ActivateBrandCommand : IdInput {}

public class ActivateBrandCommandHandler : IRequestHandler<ActivateBrandCommand, IResult>
{
    private readonly ICrudService _crudService;
    public ActivateBrandCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        ActivateBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.ActivateAsync<Brand>(command.Id);
        return Results.Ok($"Brand with Id: {command.Id} was activated successfully");
    }

}
