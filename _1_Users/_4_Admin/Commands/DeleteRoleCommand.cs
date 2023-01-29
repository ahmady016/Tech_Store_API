using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class DeleteRoleCommand : IdInput {}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IResultService _resultService;
    public DeleteRoleCommandHandler(
        RoleManager<Role> roleManager,
        IResultService resultService
    )
    {
        _roleManager = roleManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var existingRole = await _roleManager.FindByIdAsync(command.Id.ToString());
        if(existingRole is null)
            return _resultService.NotFound(nameof(DeleteRoleCommand), nameof(Role), command.Id);

        var identityResult = await _roleManager.DeleteAsync(existingRole);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(DeleteRoleCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded("Role deleted successfully ...");
    }
}
