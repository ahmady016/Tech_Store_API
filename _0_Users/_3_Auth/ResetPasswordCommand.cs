using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;
using Common;

namespace Auth.Commands;

public class ResetPasswordCommand : IRequest<IResult>
{
    public string UserId { get; set; }
    public string Token { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "Password Must be between 8 and 64 characters")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "ConfirmPassword is required")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "ConfirmPassword Must be between 8 and 64 characters")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public ResetPasswordCommandHandler(
        IEmailService emailService,
        UserManager<User> userManager,
        ILogger<Product> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        ResetPasswordCommand command,
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

        if(command.Password != command.ConfirmPassword)
        {
            _errorMessage = $"ConfirmPassword and Password does't match !!!";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        var identityResult = await _userManager.ResetPasswordAsync(existedUser, command.Token, command.Password);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = "Reset Password Completed Successfully ..." });
    }

}
