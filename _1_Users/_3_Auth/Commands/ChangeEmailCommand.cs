using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class ChangeEmailCommand : IRequest<IResult>
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "NewEmail is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "NewEmail Must be between 10 and 100 characters")]
    public string NewEmail { get; set; }
}

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, IResult>
{
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IOptions<BaseUrl> _baseUrl;
    private readonly IEmailService _emailService;
    private readonly IResultService _resultService;
    public ChangeEmailCommandHandler(
        IHttpContextAccessor httpAccessor,
        UserManager<User> userManager,
        IOptions<BaseUrl> baseUrl,
        IEmailService emailService,
        IResultService resultService
    )
    {
        _httpAccessor = httpAccessor;
        _userManager = userManager;
        _baseUrl = baseUrl;
        _emailService = emailService;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        ChangeEmailCommand command,
        CancellationToken cancellationToken
    )
    {
        // get the currentUser by old email
        var userEmail = JwtService.GetCurrentUserEmail(_httpAccessor.HttpContext);

        // return error if not found
        var currentUser = await _userManager.FindByEmailAsync(userEmail);
        if(currentUser is null)
            return _resultService.NotFound(
                nameof(ChangeEmailCommand),
                $"User with Email: {userEmail} not Found !!!"
            );

        // return error if NewEmail not new
        if(currentUser.Email == command.NewEmail)
            return _resultService.BadRequest(nameof(ChangeEmailCommand), "NewEmail must be not the existing Email!!!");

        // generate email confirmation token and send confirmation email
        var token = await _userManager.GenerateChangeEmailTokenAsync(currentUser, command.NewEmail);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationUrl = new Uri($"{_baseUrl.Value.Server}/api/Auth/ConfirmChangeEmail?userId={currentUser.Id}&newEmail={command.NewEmail}&token={encodedToken}");
        await _emailService.SendAsync(
            currentUser.Email,
            "Please Confirm Your Email",
            $"Please Click on this link to confirm your email: {confirmationUrl}"
        );

        // return success message
        return _resultService.Succeeded($"please confirm the new email ... {confirmationUrl}");
    }
}
