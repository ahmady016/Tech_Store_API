using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class ResetPasswordCommand : IRequest<IResult>
{
    public Guid UserId { get; set; }
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
    private readonly IResultService _resultService;
    public ResetPasswordCommandHandler(
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        ResetPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await _userManager.FindByIdAsync(command.UserId.ToString());
        if(currentUser is null)
            return _resultService.NotFound(nameof(ResetPasswordCommand), nameof(User), command.UserId);

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.Token));
        var identityResult = await _userManager.ResetPasswordAsync(currentUser, decodedToken, command.Password);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(ResetPasswordCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded("Password has been reset successfully ...");
    }
}
