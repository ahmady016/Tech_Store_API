using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;
using Dtos;

namespace Admin.Queries;

public class DeleteUserCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "UserId Must between 10 and 450 characters")]
    public string UserId { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<User> _logger;
    private string _errorMessage;
    public DeleteUserCommandHandler (
        UserManager<User> userManager,
        ILogger<User> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        DeleteUserCommand command,
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

        var identityResult = await _userManager.DeleteAsync(existedUser);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = $"User with Id: {command.UserId} Deleted successfully ..." });
    }

}
