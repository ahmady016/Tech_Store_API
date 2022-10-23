using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Models.Commands;

public class ActivateModelCommand : IdInput {}

public class ActivateModelCommandHandler : IRequestHandler<ActivateModelCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public ActivateModelCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        ActivateModelCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.ActivateAsync<Model>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Model with Id: {command.Id} was activated successfully");
    }

}
