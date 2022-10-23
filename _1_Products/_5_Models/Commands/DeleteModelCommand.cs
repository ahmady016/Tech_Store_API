using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Models.Commands;

public class DeleteModelCommand : IdInput {}

public class DeleteModelCommandHandler : IRequestHandler<DeleteModelCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public DeleteModelCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DeleteModelCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.DeleteAsync<Model>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Model with Id: {command.Id} was deleted successfully");
    }

}
