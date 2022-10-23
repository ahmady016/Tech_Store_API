using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Products.Commands;

public class RestoreProductCommand : IdInput {}

public class RestoreProductCommandHandler : IRequestHandler<RestoreProductCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public RestoreProductCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        RestoreProductCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.RestoreAsync<Product>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Product with Id: {command.Id} was restored successfully");
    }

}
