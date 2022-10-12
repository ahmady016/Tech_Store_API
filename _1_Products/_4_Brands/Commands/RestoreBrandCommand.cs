using MediatR;

using DB;
using Common;
using Entities;

namespace Brands.Commands;

public class RestoreBrandCommand : IdInput {}

public class RestoreBrandCommandHandler : IRequestHandler<RestoreBrandCommand, IResult>
{
    private readonly ICrudService _crudService;
    public RestoreBrandCommandHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        RestoreBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        await _crudService.RestoreAsync<Brand>(command.Id);
        return Results.Ok($"Brand with Id: {command.Id} was restored successfully");
    }

}
