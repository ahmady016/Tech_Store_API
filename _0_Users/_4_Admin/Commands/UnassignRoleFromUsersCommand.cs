using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using DB;
using Entities;

namespace Admin.Commands;

public class UnassignRoleFromUsersCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "RoleId Must between 10 and 450 characters")]
    public string RoleId { get; set; }

    [Required(ErrorMessage = "UsersIds is required")]
    public List<string> UsersIds { get; set; }
}

public class UnassignRoleFromUsersCommandHandler : IRequestHandler<UnassignRoleFromUsersCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IDBCommandService _dbCommandService;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public UnassignRoleFromUsersCommandHandler (
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
        UnassignRoleFromUsersCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedRole = await _roleManager.FindByIdAsync(command.RoleId);
        if(existedRole is null)
        {
            _errorMessage = $"Role Record with Id: {command.RoleId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var usersRoles = command.UsersIds
            .Select(userId => new UserRole() { UserId = userId, RoleId = existedRole.Id })
            .ToList();
        _dbCommandService.RemoveRange<UserRole>(usersRoles);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"Role with Id: {command.RoleId} was Unassigned from The Given Users successfully ..." });
    }

}
