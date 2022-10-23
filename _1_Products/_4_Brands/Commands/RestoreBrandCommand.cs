using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Brands.Commands;

public class RestoreBrandCommand : IdInput {}

public class RestoreBrandCommandHandler : IRequestHandler<RestoreBrandCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public RestoreBrandCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        RestoreBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.RestoreAsync<Brand>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Brand with Id: {command.Id} was restored successfully");
    }

}
