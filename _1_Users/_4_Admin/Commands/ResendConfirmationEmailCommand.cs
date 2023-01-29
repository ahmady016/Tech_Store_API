using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class ResendConfirmationEmailCommand : IRequest<IResult>
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }
}

public class SendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IOptions<BaseUrl> _baseUrl;
    private readonly IEmailService _emailService;
    private readonly IResultService _resultService;
    public SendConfirmationEmailCommandHandler(
        UserManager<User> userManager,
        IOptions<BaseUrl> baseUrl,
        IEmailService emailService,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _baseUrl = baseUrl;
        _emailService = emailService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        ResendConfirmationEmailCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existingUser from db
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        // if existingUser not found
        if(existingUser is null)
            return _resultService.NotFound(nameof(ResendConfirmationEmailCommand), "email not found");

        // if User Email is Confirmed already
        if(existingUser.EmailConfirmed)
            return _resultService.Conflict(nameof(ResendConfirmationEmailCommand), $"This email: {command.Email} is already confirmed");

        // generate email confirmation token and send confirmation email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationUrl = new Uri($"{_baseUrl.Value.Server}/api/Auth/ConfirmEmail?userId={existingUser.Id}&token={encodedToken}");
        await _emailService.SendAsync(
            existingUser.Email,
            "Please Confirm Your Email",
            $"Please Click on this link to confirm your email: {confirmationUrl}"
        );

        // finally return success message
        return _resultService.Created(
            nameof(ResendConfirmationEmailCommand),
            $"New User with Id: [{existingUser.Id}] has been created successfully and confirmation email was sent to his email address. confirmationUrl: {confirmationUrl}"
        );
    }
}
