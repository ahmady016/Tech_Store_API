using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class AssignRoleToUsersCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    public Guid RoleId { get; set; }

    [Required(ErrorMessage = "UsersIds is required")]
    public List<Guid> UsersIds { get; set; }
}

public class AssignRoleToUsersCommandHandler : IRequestHandler<AssignRoleToUsersCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IRawDBService _rawDBService;
    private readonly IResultService _resultService;
    public AssignRoleToUsersCommandHandler(
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
        AssignRoleToUsersCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existingRole from db
        var existingRole = await _roleManager.FindByIdAsync(command.RoleId.ToString());
        // if existingRole not found
        if(existingRole is null)
            return _resultService.NotFound(nameof(AssignRoleToUsersCommand), nameof(Role), command.RoleId);

        // if any user not found
        var usersCount = await _rawDBService.CountAsync<User>(e => command.UsersIds.Contains(e.Id));
        if(usersCount < command.UsersIds.Count)
            return _resultService.NotFound(nameof(AssignRoleToUsersCommand), "one or more User(s) not found!!!");

        // if any user already in This Role
        var existingRoleUsers = await _rawDBService.GetListAsync<UserRole>(
            e => e.RoleId == command.RoleId && command.UsersIds.Contains(e.UserId)
        );
        if(existingRoleUsers.Any(e => command.UsersIds.Contains(e.UserId)))
            return _resultService.Conflict(nameof(AssignRoleToUsersCommand), "one or more User(s) already in This Role!!!");

        // Assign Role To Users
        var usersRoles = command.UsersIds
            .Select(userId => new UserRole() { UserId = userId, RoleId = existingRole.Id })
            .ToList();
        _rawDBService.AddRange<UserRole>(usersRoles);
        await _rawDBService.SaveChangesAsync();

        return _resultService.Succeeded($"Role with Id: {command.RoleId} has been assigned to the given Users successfully ...");
    }
}
