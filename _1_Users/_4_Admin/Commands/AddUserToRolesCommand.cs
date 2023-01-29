using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class AddUserToRolesCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Roles is required")]
    public List<string> Roles { get; set; }
}

public class AddUserToRolesCommandHandler : IRequestHandler<AddUserToRolesCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IResultService _resultService;
    public AddUserToRolesCommandHandler(
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
        AddUserToRolesCommand command,
        CancellationToken cancellationToken
    )
    {
        // get User from db
        var existingUser = await _userManager.FindByIdAsync(command.UserId.ToString());
        // if User not found
        if (existingUser is null)
            return _resultService.NotFound(nameof(AddUserToRolesCommand), nameof(User), command.UserId);

        // if any role not found
        var foundRoles =  await _roleManager.Roles.CountAsync(e => command.Roles.Contains(e.Name));
        if(foundRoles < command.Roles.Count)
            return _resultService.NotFound(nameof(AddUserToRolesCommand), "one or more Role(s) not found!!!");

        // add User To Roles
        var identityResult = await _userManager.AddToRolesAsync(existingUser, command.Roles);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(AddUserToRolesCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded($"User with Id: {command.UserId} has been added to the given Roles successfully ...");
    }
}
