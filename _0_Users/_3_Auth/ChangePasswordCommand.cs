using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;
using Common;

namespace Auth.Commands;

public class ChangePasswordCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "UserId Must be between 10 and 450 characters")]
    public string UserId { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "OldPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "OldPassword Must be between 8 and 50 characters")]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "NewPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "NewPassword Must be between 8 and 50 characters")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "ConfirmNewPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "ConfirmNewPassword Must be between 8 and 50 characters")]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public ChangePasswordCommandHandler(
        UserManager<User> userManager,
        ILogger<Product> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByIdAsync(command.UserId);
        if (existedUser is null)
        {
            _errorMessage = $"User with Id: {command.UserId} not Found !!!";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        if(command.NewPassword != command.ConfirmNewPassword)
        {
            _errorMessage = $"ConfirmNewPassword and NewPassword does't match !!!";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        var identityResult = await _userManager.ChangePasswordAsync(existedUser, command.OldPassword, command.NewPassword);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = "User Password Changed Successfully ..." });
    }

}
