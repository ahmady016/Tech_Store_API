using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;

namespace Auth.Commands;

public class ConfirmEmailCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "UserId is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 450 characters")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Token is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Token Must be between 10 and 450 characters")]
    public string Token { get; set; }
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IResult> {
    private readonly UserManager<User> _userManager;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public ConfirmEmailCommandHandler(
        UserManager<User> userManager,
        ILogger<Product> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IResult> Handle (
        ConfirmEmailCommand command,
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

        var identityResult = await _userManager.ConfirmEmailAsync(existedUser, command.Token);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(_errorMessage);
        }

        return Results.Ok(new { Message = "Email is successfully confirmed, you can signin now" });
    }

}
