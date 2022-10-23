using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Models.Commands;

public class DisableModelCommand : IdInput {}

public class DisableModelCommandHandler : IRequestHandler<DisableModelCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public DisableModelCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DisableModelCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.DisableAsync<Model>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Model with Id: {command.Id} was disabled successfully");
    }

}
