using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;

namespace Admin.Commands;

public class RemoveUserFromRolesCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "UserId Must between 10 and 450 characters")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Roles is required")]
    public List<string> Roles { get; set; }
}

public class RemoveUserFromRolesCommandHandler : IRequestHandler<RemoveUserFromRolesCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public RemoveUserFromRolesCommandHandler (
        UserManager<User> userManager,
        ILogger<User> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        RemoveUserFromRolesCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByIdAsync(command.UserId);
        if(existedUser is null)
        {
            _errorMessage = $"User Record with Id: {command.UserId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var identityResult = await _userManager.RemoveFromRolesAsync(existedUser, command.Roles);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = $"User with Id: {command.UserId} was removed from The Given Roles successfully ..." });
    }

}
