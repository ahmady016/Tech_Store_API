using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;

namespace Admin.Commands;

public class UpdateUserRolesCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "UserId Must between 10 and 450 characters")]
    public string UserId { get; set; }
    public List<string> AddToRoles { get; set; }
    public List<string> RemoveFromRoles { get; set; }
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public UpdateUserRolesCommandHandler (
        UserManager<User> userManager,
        ILogger<User> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateUserRolesCommand command,
        CancellationToken cancellationToken
    )
    {
        if(command.AddToRoles is null && command.RemoveFromRoles is null)
        {
            _errorMessage = $"Must specify atleast one of [AddToRoles, RemoveFromRoles]";
            _logger.LogError(_errorMessage);
            return Results.BadRequest( new { Message = _errorMessage });
        }

        var existedUser = await _userManager.FindByIdAsync(command.UserId);
        if(existedUser is null)
        {
            _errorMessage = $"User Record with Id: {command.UserId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var addToRolesResult = await _userManager.AddToRolesAsync(existedUser, command.AddToRoles);
        if(addToRolesResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", addToRolesResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }
        var removeFromRolesResult = await _userManager.RemoveFromRolesAsync(existedUser, command.RemoveFromRoles);
        if(removeFromRolesResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", removeFromRolesResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = $"User [with Id: {command.UserId}] Roles Updated successfully ..." });
    }

}
