using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using Entities;
using Common;

namespace Auth.Commands;

public class ChangeEmailCommand : IRequest<IResult>
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    public string Email { get; set; }

    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "NewEmail is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "NewEmail Must be between 10 and 100 characters")]
    public string NewEmail { get; set; }
}

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public ChangeEmailCommandHandler(
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
        ChangeEmailCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByEmailAsync(command.Email);
        if (existedUser is null)
        {
            _errorMessage = $"User with Email: {command.Email} not Found !!!";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        // update UserName and Email and Save it
        existedUser.UserName = existedUser.Email = command.NewEmail;
        await _userManager.UpdateAsync(existedUser);

        // generate email confirmation token and send confirmation email
        var confirmationToken = await _userManager.GeneratePasswordResetTokenAsync(existedUser);
        var confirmationUrl = new Uri($"{_config["BaseUrl"]}/api/Auth/ConfirmEmail?userId={existedUser.Id}&token={confirmationToken}");        await _emailService.SendAsync(
            existedUser.Email,
            "Please Confirm Your Email",
            $"Please Click on this link to confirm your email: {confirmationUrl}"
        );
        return Results.Ok(new { Message = "Email Changed Successfully, please Confirm Your New Email  ..." });
    }

}
