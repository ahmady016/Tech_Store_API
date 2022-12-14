using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using DB;
using Entities;

namespace Admin.Commands;

public class UpdateRoleUsersCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "RoleId Must between 10 and 450 characters")]
    public string RoleId { get; set; }
    public List<string> UsersIdsToAdd { get; set; }
    public List<string> UsersIdsToRemove { get; set; }
}

public class UpdateRoleUsersCommandHandler : IRequestHandler<UpdateRoleUsersCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IDBCommandService _dbCommandService;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public UpdateRoleUsersCommandHandler (
        RoleManager<Role> roleManager,
        IDBCommandService dbCommandService,
        ILogger<User> logger
    )
    {
        _roleManager = roleManager;
        _dbCommandService = dbCommandService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateRoleUsersCommand command,
        CancellationToken cancellationToken
    )
    {
        if(
            (command.UsersIdsToAdd is null || command.UsersIdsToAdd.Count == 0) &&
            (command.UsersIdsToRemove is null || command.UsersIdsToRemove.Count == 0)
        )
        {
            _errorMessage = $"Must specify at least one of [UsersIdsToAdd, UsersIdsToRemove]";
            _logger.LogError(_errorMessage);
            return Results.BadRequest( new { Message = _errorMessage });
        }

        var existedRole = await _roleManager.FindByIdAsync(command.RoleId);
        if(existedRole is null)
        {
            _errorMessage = $"Role Record with Id: {command.RoleId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        if(command.UsersIdsToAdd is not null && command.UsersIdsToAdd.Count > 0)
        {
            var usersRolesToAdd = command.UsersIdsToAdd
                .Select(userId => new UserRole() { UserId = userId, RoleId = existedRole.Id })
                .ToList();
            _dbCommandService.AddRange<UserRole>(usersRolesToAdd);
            await _dbCommandService.SaveChangesAsync();
        }

        if(command.UsersIdsToRemove is not null && command.UsersIdsToRemove.Count > 0)
        {
            var usersRoles = _dbCommandService.GetListAndRemoveRangeAsync<UserRole>(
                e => e.RoleId == command.RoleId && command.UsersIdsToRemove.Contains(e.UserId)
            );
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(new { Message = $"Role [with Id: {command.RoleId}] Users was Updated successfully ..." });
    }

}
