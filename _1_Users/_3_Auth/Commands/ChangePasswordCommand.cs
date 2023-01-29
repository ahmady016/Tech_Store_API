
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Auth.Commands;

public class ChangePasswordCommand : IRequest<IResult>
{
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "OldPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "OldPassword Must be between 8 and 50 characters")]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "NewPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "NewPassword Must be between 8 and 50 characters")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "ConfirmNewPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "ConfirmNewPassword Must be between 8 and 50 characters")]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IResult>
{
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IResultService _resultService;
    public ChangePasswordCommandHandler(
        IHttpContextAccessor httpAccessor,
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _httpAccessor = httpAccessor;
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        // get the currentUser by old email and return error if not found
        var userId = JwtService.GetCurrentUserId(_httpAccessor.HttpContext);
        var currentUser = await _userManager.FindByIdAsync(userId);
        if(currentUser is null)
            return _resultService.NotFound(nameof(ChangePasswordCommand), nameof(User), Guid.Parse(userId));

        var identityResult = await _userManager.ChangePasswordAsync(currentUser, command.OldPassword, command.NewPassword);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(ChangePasswordCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        // return success message
        return _resultService.Succeeded("The User Password changed successfully ...");
    }
}
