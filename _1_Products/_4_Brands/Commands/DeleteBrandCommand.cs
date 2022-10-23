using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Brands.Commands;

public class DeleteBrandCommand : IdInput {}

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public DeleteBrandCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DeleteBrandCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.DeleteAsync<Brand>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Brand with Id: {command.Id} was deleted successfully");
    }

}
