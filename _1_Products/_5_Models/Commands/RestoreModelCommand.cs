using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Models.Commands;

public class RestoreModelCommand : IdInput {}

public class RestoreModelCommandHandler : IRequestHandler<RestoreModelCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public RestoreModelCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        RestoreModelCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.RestoreAsync<Model>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Model with Id: {command.Id} was restored successfully");
    }

}
