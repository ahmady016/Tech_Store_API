
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class DeleteUserCommand : IdInput {}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IResultService _resultService;
    public DeleteUserCommandHandler(
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await _userManager.FindByIdAsync(command.Id.ToString());
        if (existingUser is null)
            return _resultService.NotFound(nameof(DeleteUserCommand), nameof(User), command.Id);

        var identityResult = await _userManager.DeleteAsync(existingUser);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(DeleteUserCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded($"User with Id: [{command.Id}] has been deleted successfully ...");
    }
}
