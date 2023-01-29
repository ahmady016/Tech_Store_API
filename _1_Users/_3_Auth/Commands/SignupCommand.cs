
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MediatR;
using Mapster;

using TechStoreApi.Common;
using TechStoreApi.Entities;
using TechStoreApi.Dtos;

namespace TechStoreApi.Auth.Commands;

public class SignupCommand : UserInput {}

public class SignupCommandHandler : IRequestHandler<SignupCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IOptions<BaseUrl> _baseUrl;
    private readonly IEmailService _emailService;
    private readonly IResultService _resultService;

    public SignupCommandHandler(
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
        SignupCommand command,
        CancellationToken cancellationToken
    )
    {
        // map to and create new user
        var newUser = command.Adapt<User>();
        var identityResult = await _userManager.CreateAsync(newUser, command.Password);

        // if any identity validation error return badRequest
        if(identityResult.Succeeded is false)
            return _resultService.BadRequest(
                nameof(SignupCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        // generate email confirmation token and send confirmation email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationUrl = new Uri($"{_baseUrl.Value.Server}/api/Auth/ConfirmEmail?userId={newUser.Id}&token={encodedToken}");
        await _emailService.SendAsync(
            newUser.Email,
            "Please Confirm Your Email",
            $"Please Click on this link to confirm your email: {confirmationUrl}"
        );

        // finally return success message
        return _resultService.Created(
            nameof(SignupCommand),
            $"New User with Id: [{newUser.Id}] has been created successfully and confirmation email was sent to his email address. confirmationUrl: {confirmationUrl}"
        );
    }
}
