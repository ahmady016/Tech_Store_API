using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;
using Common;

namespace Auth.Commands;

public class SendConfirmationEmailCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }
}

public class SendConfirmationEmailCommandHandler : IRequestHandler<SendConfirmationEmailCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public SendConfirmationEmailCommandHandler(
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
        SendConfirmationEmailCommand command,
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
        // generate email confirmation token and send confirmation email
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(existedUser);
        var confirmationUrl = new Uri($"{_config["BaseUrl"]}/api/Auth/ConfirmEmail?userId={existedUser.Id}&token={confirmationToken}");
        await _emailService.SendAsync(
            existedUser.Email,
            "Please Confirm Your Email",
            $"Please Click on this link to confirm your email: {confirmationUrl}"
        );
        return Results.Ok(new { Message = "Confirmation Email was sent successfully ..." });
    }

}
