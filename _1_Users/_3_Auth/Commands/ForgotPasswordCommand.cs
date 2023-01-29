
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class ForgotPasswordCommand : IRequest<IResult>
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IOptions<BaseUrl> _baseUrl;
    private readonly IEmailService _emailService;
    private readonly IResultService _resultService;
    public ForgotPasswordCommandHandler(
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
        ForgotPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        // get the currentUser by old email and return error if not found
        var currentUser = await _userManager.FindByEmailAsync(command.Email);
        if(currentUser is null)
            return _resultService.NotFound(
                nameof(ForgotPasswordCommand),
                $"User with Email: {command.Email} not Found !!!"
            );

        // generate Password Reset Token and send reset password email
        var token = await _userManager.GeneratePasswordResetTokenAsync(currentUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationUrl = new Uri($"{_baseUrl.Value.Client}/auth/reset-password?userId={currentUser.Id}&token={encodedToken}");
        await _emailService.SendAsync(
            currentUser.Email,
            "Please Reset Your Password",
            $"Please Click on this link to reset your password: {confirmationUrl}"
        );

        // return success message
        return _resultService.Succeeded($"The Reset Password Link is sent to your email ... {confirmationUrl}");
    }
}
