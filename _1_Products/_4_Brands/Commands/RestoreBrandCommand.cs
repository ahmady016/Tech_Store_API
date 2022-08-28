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
        _ = _crudService.Restore<Brand>(command.Id);
        return await Task.FromResult(Results.Ok($"Brand with Id: {command.Id} was restored successfully"));
    }

}
