using MediatR;

using DB;
using Common;
using Entities;
using Auth;

namespace Products.Commands;

public class DeleteProductCommand : IdInput {}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly ICrudService _crudService;
    public DeleteProductCommandHandler(
        IAuthService authService,
        ICrudService crudService
    )
    {
        _authService = authService;
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken
    )
    {
        var loggedUserEmail = _authService.GetCurrentUserEmail();
        await _crudService.DeleteAsync<Product>(command.Id, loggedUserEmail ?? "app_dev");
        return Results.Ok($"Product with Id: {command.Id} was deleted successfully");
    }

}
