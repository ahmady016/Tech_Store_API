using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Brands.Commands;

public class ActivateBrandCommand : IdInput {}

public class ActivateBrandCommandHandler : IRequestHandler<ActivateBrandCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public ActivateBrandCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        ActivateBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.ActivateAsync<Brand>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Brand with Id: {command.Id} was activated successfully");
    }

}
