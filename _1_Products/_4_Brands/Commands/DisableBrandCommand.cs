using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Brands.Commands;

public class DisableBrandCommand : IdInput {}

public class DisableBrandCommandHandler : IRequestHandler<DisableBrandCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public DisableBrandCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DisableBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.DisableAsync<Brand>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Brand with Id: {command.Id} was disabled successfully");
    }

}
