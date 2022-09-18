using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;

namespace Admin.Commands;

public class UpdateUserCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Id is required")]
    [StringLength(450, MinimumLength = 10, ErrorMessage = "Id Must between 10 and 450 characters")]
    public string Id { get; set; }

    [Required(ErrorMessage = "FirstName is Required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "FirstName must between 5 and 100 characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is Required")]
    [StringLength(70, MinimumLength = 5, ErrorMessage = "LastName must between 10 and 400 characters")]
    public string LastName { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "BirthDate is required")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
        public UpdateUserCommandHandler(
        IMapper mapper,
        UserManager<User> userManager,
        ILogger<Product> logger
    )
    {
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    private void UpdateExistedUser(User existedUser, UpdateUserCommand updatedUser)
    {
        existedUser.FirstName = updatedUser.FirstName;
        existedUser.LastName = updatedUser.LastName;
        existedUser.BirthDate = updatedUser.BirthDate;
        existedUser.Gender = updatedUser.Gender;
    }

    public async Task<IResult> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        var existedUser = await _userManager.FindByIdAsync(command.Id);
        UpdateExistedUser(existedUser, command);
        var identityResult = await _userManager.UpdateAsync(existedUser);
        if(identityResult.Succeeded is false)
        {
            _errorMessage = String.Join(", ", identityResult.Errors.Select(error => error.Description).ToArray());
            _logger.LogError(_errorMessage);
            return Results.Conflict(new { Message = _errorMessage });
        }

        return Results.Ok(new { Message = "User Updated successfully ..." });
    }

}
