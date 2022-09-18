using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;

namespace Admin.Commands;

public class DeleteRoleCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "RoleId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "RoleId Must between 10 and 450 characters")]
    public string RoleId { get; set; }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResult>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<Role> _logger;
    private string _errorMessage;
    public DeleteRoleCommandHandler (
        RoleManager<Role> roleManager,
        ILogger<Role> logger
    )
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        DeleteRoleCommand command,
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

        var identityResult = await _roleManager.DeleteAsync(existedRole);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = $"Role with Id: {command.RoleId} Deleted successfully ..." });
    }

}
