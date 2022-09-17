using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;
using Common;

namespace Auth.Commands;

public class ForgotPasswordCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public ForgotPasswordCommandHandler(
        UserManager<User> userManager,
        IEmailService emailService,
        IConfiguration Configuration,
        ILogger<Product> logger
    )
    {
        _userManager = userManager;
        _emailService = emailService;
        _config = Configuration;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByEmailAsync(command.Email);
        if(existedUser is null)
        {
            _errorMessage = $"User with Email: {command.Email} not Found !!!";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        // generate ForgotPassword confirmation token and send confirmation email
        var confirmationToken = await _userManager.GeneratePasswordResetTokenAsync(existedUser);
        var confirmationUrl = new Uri($"{_config["WebBaseUrl"]}/auth/reset-password?userId={existedUser.Id}&token={confirmationToken}");
        await _emailService.SendAsync(
            existedUser.Email,
            "Please Reset Your Password",
            $"Please Click on this link to reset your password: {confirmationUrl}"
        );
        return Results.Ok(new { Message = "The Reset Password Link is sent to your email ..." });
    }

}
