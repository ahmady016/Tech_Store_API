using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class RemoveUserFromRolesCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Roles is required")]
    public List<string> Roles { get; set; }
}

public class RemoveUserFromRolesCommandHandler : IRequestHandler<RemoveUserFromRolesCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IResultService _resultService;
    public RemoveUserFromRolesCommandHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        RemoveUserFromRolesCommand command,
        CancellationToken cancellationToken
    )
    {
        // get User from db
        var existingUser = await _userManager.FindByIdAsync(command.UserId.ToString());
        // if User not found
        if (existingUser is null)
            return _resultService.NotFound(nameof(RemoveUserFromRolesCommand), nameof(User), command.UserId);

        // if any role not found
        var foundRoles =  await _roleManager.Roles.CountAsync(e => command.Roles.Contains(e.Name));
        if(foundRoles < command.Roles.Count)
            return _resultService.NotFound(nameof(RemoveUserFromRolesCommand), "one or more Role(s) not found!!!");

        // remove User From Roles
        var identityResult = await _userManager.RemoveFromRolesAsync(existingUser, command.Roles);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(RemoveUserFromRolesCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded($"User with Id: {command.UserId} has been removed from the given Roles successfully ...");
    }
}
