using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Products.Commands;

public class ActivateProductCommand : IdInput {}

public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public ActivateProductCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        ActivateProductCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.ActivateAsync<Product>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Product with Id: {command.Id} was activated successfully");
    }

}
