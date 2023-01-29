using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.DB;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class UnassignRoleFromUsersCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    public Guid RoleId { get; set; }

    [Required(ErrorMessage = "UsersIds is required")]
    public List<Guid> UsersIds { get; set; }
}

public class UnassignRoleFromUsersCommandHandler : IRequestHandler<UnassignRoleFromUsersCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IRawDBService _rawDBService;
    private readonly IResultService _resultService;
    public UnassignRoleFromUsersCommandHandler(
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
        UnassignRoleFromUsersCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existingRole from db
        var existingRole = await _roleManager.FindByIdAsync(command.RoleId.ToString());
        // if existingRole not found
        if(existingRole is null)
            return _resultService.NotFound(nameof(UnassignRoleFromUsersCommand), nameof(Role), command.RoleId);

        // Unassigned Role From Users
        var rowsAffected = await _rawDBService.ExecuteDeleteAsync<UserRole>(e => e.RoleId == command.RoleId && command.UsersIds.Contains(e.UserId));
        if(rowsAffected == 0)
            return _resultService.NotFound(
                nameof(UnassignRoleFromUsersCommand),
                $"all Users are either not found Or not assigned to Role with Id: {command.RoleId}"
            );

        return _resultService.Succeeded($"Role with Id: {command.RoleId} has been unassigned from {rowsAffected} User(s) successfully ...");
    }
}
