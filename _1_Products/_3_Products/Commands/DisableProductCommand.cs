using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Products.Commands;

public class DisableProductCommand : IdInput {}

public class DisableProductCommandHandler : IRequestHandler<DisableProductCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public DisableProductCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DisableProductCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.DisableAsync<Product>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Product with Id: {command.Id} was disabled successfully");
    }

}
