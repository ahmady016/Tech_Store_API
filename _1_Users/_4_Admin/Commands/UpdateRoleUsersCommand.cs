using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class UpdateRoleUsersCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    public Guid RoleId { get; set; }
    public List<Guid> AddedUsersIds { get; set; }
    public List<Guid> RemovedUsersIds { get; set; }
}

public class UpdateRoleUsersCommandHandler : IRequestHandler<UpdateRoleUsersCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IRawDBService _rawDBService;
    private readonly IResultService _resultService;
    public UpdateRoleUsersCommandHandler(
        RoleManager<Role> roleManager,
        IRawDBService rawDBService,
        IResultService resultService
    )
    {
        _roleManager = roleManager;
        _rawDBService = rawDBService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        UpdateRoleUsersCommand command,
        CancellationToken cancellationToken
    )
    {
        // if no UsersIds
        var isAddedUsersIdsEmpty = command.AddedUsersIds is null || command.AddedUsersIds.Count == 0;
        var isRemovedUsersIdsEmpty = command.RemovedUsersIds is null || command.RemovedUsersIds.Count == 0;
        if(isAddedUsersIdsEmpty && isRemovedUsersIdsEmpty)
            return _resultService.BadRequest(
                nameof(UpdateRoleUsersCommand),
                "Must specify at least one userId either in [AddedUsersIds] or in [RemovedUsersIds]"
            );

        // get existingRole from db
        var existingRole = await _roleManager.FindByIdAsync(command.RoleId.ToString());
        // if existingRole not found
        if(existingRole is null)
            return _resultService.NotFound(nameof(UpdateRoleUsersCommand), nameof(Role), command.RoleId);

        // Assign Role To Users
        var addedUsersErrors = new List<string>();
        if(!isAddedUsersIdsEmpty)
        {
            // if any user not found
            var usersCount = await _rawDBService.CountAsync<User>(e => command.AddedUsersIds.Contains(e.Id));
            if(usersCount < command.AddedUsersIds.Count)
                addedUsersErrors.Add("one or more User(s) not found!!!");
            // if any user already in This Role
            var existingRoleUsers = await _rawDBService.GetListAsync<UserRole>(
                e => e.RoleId == command.RoleId && command.AddedUsersIds.Contains(e.UserId)
            );
            if(existingRoleUsers.Any(e => command.AddedUsersIds.Contains(e.UserId)))
                addedUsersErrors.Add("one or more User(s) already in This Role!!!");

            var usersRoles = command.AddedUsersIds
                .Select(userId => new UserRole() { UserId = userId, RoleId = existingRole.Id })
                .ToList();
            _rawDBService.AddRange<UserRole>(usersRoles);
            await _rawDBService.SaveChangesAsync();
        }
        // Unassigned Role From Users
        string removedUsersError = string.Empty;
        if(!isRemovedUsersIdsEmpty)
        {
            var rowsAffected = await _rawDBService.ExecuteDeleteAsync<UserRole>(
                e => e.RoleId == command.RoleId && command.RemovedUsersIds.Contains(e.UserId)
            );
            if(rowsAffected == 0)
                removedUsersError = $"all RemovedUsersIds are either not found Or not assigned to Role with Id: {command.RoleId}";
        }
        // return the result
        var addedUsersSucceeded = addedUsersErrors.Count == 0;
        var removedUsersSucceeded = string.IsNullOrEmpty(removedUsersError);
        if(addedUsersSucceeded && removedUsersSucceeded)
            return _resultService.Succeeded($"Role [{command.RoleId}] Users has been updated successfully ...");
        else if(addedUsersSucceeded is false && removedUsersSucceeded is false)
        {
            addedUsersErrors.Add(removedUsersError);
            return _resultService.Conflict(nameof(UpdateRoleUsersCommand), string.Join(", ", addedUsersErrors));
        }
        else if(removedUsersSucceeded is false)
            return _resultService.Conflict(nameof(UpdateRoleUsersCommand), $"Role [{command.RoleId}] => assigned to users successfully, but there is errors in unassigned users: {removedUsersError}");
        else
            return _resultService.Conflict(nameof(UpdateRoleUsersCommand), $"Role [{command.RoleId}] => unassigned from users successfully, but there is errors in assigned users: {string.Join(", ", addedUsersErrors)}");
    }
}
