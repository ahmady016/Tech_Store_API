using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class UpdateUserRolesCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }
    public List<string> AddedRoles { get; set; }
    public List<string> RemovedRoles { get; set; }
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IResultService _resultService;
    public UpdateUserRolesCommandHandler(
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
        UpdateUserRolesCommand command,
        CancellationToken cancellationToken
    )
    {
        // if no roles given
        var isAddedRolesEmpty = command.AddedRoles is null || command.AddedRoles.Count == 0;
        var isRemovedRolesEmpty = command.RemovedRoles is null || command.RemovedRoles.Count == 0;
        if(isAddedRolesEmpty && isRemovedRolesEmpty)
            return _resultService.BadRequest(
                nameof(UpdateUserRolesCommand),
                "Must specify at least one role either in [AddedRoles] or in [RemovedRoles]"
            );

        // get User from db
        var existingUser = await _userManager.FindByIdAsync(command.UserId.ToString());
        // if User not found
        if (existingUser is null)
            return _resultService.NotFound(nameof(UpdateUserRolesCommand), nameof(User), command.UserId);

        // if added roles not empty
        var addedRolesError = string.Empty;
        if(!isAddedRolesEmpty)
        {
            // if any role not found
            var foundRoles =  await _roleManager.Roles.CountAsync(e => command.AddedRoles.Contains(e.Name));
            if(foundRoles < command.AddedRoles.Count)
                addedRolesError = "one or more AddedRoles(s) not found!!!";
            else
            {
                // add User To Roles
                var addedRolesResult = await _userManager.AddToRolesAsync(existingUser, command.AddedRoles);
                if(addedRolesResult.Succeeded is false)
                    addedRolesError = string.Join(", ", addedRolesResult.Errors.Select(error => error.Description));
            }
        }
        // if removed roles not empty
        var removedRolesError = string.Empty;
        if(!isRemovedRolesEmpty)
        {
            // if any role not found
            var foundRoles =  await _roleManager.Roles.CountAsync(e => command.RemovedRoles.Contains(e.Name));
            if(foundRoles < command.RemovedRoles.Count)
                removedRolesError = "one or more RemovedRoles(s) not found!!!";
            else
            {
                // remove User From Roles
                var removedRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, command.RemovedRoles);
                if(removedRolesResult.Succeeded is false)
                    removedRolesError = string.Join(", ", removedRolesResult.Errors.Select(error => error.Description));
            }
        }

        // return the result
        var addedRolesSucceeded = string.IsNullOrEmpty(addedRolesError);
        var removedRolesSucceeded = string.IsNullOrEmpty(removedRolesError);
        if(addedRolesSucceeded && removedRolesSucceeded)
            return _resultService.Succeeded($"User [{command.UserId}] roles has been updated successfully ...");
        else if(removedRolesSucceeded is false && removedRolesSucceeded is false)
            return _resultService.Conflict(nameof(UpdateUserRolesCommand), string.Join(", ", addedRolesError, removedRolesError));
        else if(removedRolesSucceeded is false)
            return _resultService.Conflict(nameof(UpdateUserRolesCommand), $"User [{command.UserId}] => added to roles successfully, but there is errors in removed roles: {removedRolesError}");
        else
            return _resultService.Conflict(nameof(UpdateUserRolesCommand), $"User [{command.UserId}] => removed from roles successfully, but there is errors in added roles: {addedRolesError}");
    }
}
