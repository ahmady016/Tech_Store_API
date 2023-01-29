
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.DB.Common;
using TechStoreApi.Entities;

namespace TechStoreApi.Admin.Commands;

public class UpdateUserCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "FullName is Required")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "FullName must between 5 and 50 characters")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "BirthDate is required")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "GenderId is required")]
    [Range(1, 2, ErrorMessage = "GenderId value must either 1 or 2")]
    public Gender GenderId { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IResultService _resultService;
    public UpdateUserCommandHandler(
        UserManager<User> userManager,
        IResultService resultService
    )
    {
        _userManager = userManager;
        _resultService = resultService;
    }

    public async Task<IResult> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existingUser from db
        var existingUser = await _userManager.FindByIdAsync(command.Id.ToString());
        // if existingUser not found
        if (existingUser is null)
            return _resultService.NotFound(nameof(UpdateUserCommand), nameof(User), command.Id);

        // if no field changed
        var isFullNameChanged = command.FullName != existingUser.FullName;
        var isBirthDateChanged = command.BirthDate != existingUser.BirthDate;
        var isGenderIdChanged = command.GenderId != existingUser.GenderId;
        if(!isBirthDateChanged && !isBirthDateChanged && !isGenderIdChanged)
            return _resultService.BadRequest(
                nameof(UpdateUserCommand),
                "nothing to update, all fields are not changed!!!"
            );

        // modify fields if changed
        if(isFullNameChanged)
            existingUser.FullName = command.FullName;
        if(isBirthDateChanged)
            existingUser.BirthDate = command.BirthDate;
        if(isGenderIdChanged)
            existingUser.GenderId = command.GenderId;

        // update existingUser
        var identityResult = await _userManager.UpdateAsync(existingUser);
        if(identityResult.Succeeded is false)
            return _resultService.Conflict(
                nameof(UpdateUserCommand),
                String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray())
            );

        return _resultService.Succeeded($"User with Id: [{command.Id}] has been updated successfully ...");
    }
}
