using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MediatR;

using Entities;
using Common;

namespace Auth.Commands;

public class SignupCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "FirstName is Required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "FirstName must between 5 and 100 characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is Required")]
    [StringLength(70, MinimumLength = 5, ErrorMessage = "LastName must between 10 and 400 characters")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "BirthDate is required")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Email Must be between 6 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password Must be between 8 and 50 characters")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "ConfirmPassword is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "ConfirmPassword Must be between 8 and 50 characters")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}

public class SignupCommandHandler : IRequestHandler<SignupCommand, IResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly ILogger<Product> _logger;
    private string _errorMessage;
    public SignupCommandHandler(
        UserManager<User> userManager,
        IEmailService emailService,
        IConfiguration Configuration,
        IMapper mapper,
        ILogger<Product> logger
    )
    {
        _userManager = userManager;
        _emailService = emailService;
        _config = Configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        SignupCommand command,
        CancellationToken cancellationToken
    )
    {
        var newUser = _mapper.Map<User>(command);
        var identityResult = await _userManager.CreateAsync(newUser, command.Password);
        if(identityResult.Succeeded)
        {
            // generate email confirmation token and send confirmation email
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationUrl = new Uri($"{_config["BaseUrl"]}/api/Auth/ConfirmEmail?userId={newUser.Id}&token={confirmationToken}");
            await _emailService.SendAsync(
                newUser.Email,
                "Please Confirm Your Email",
                $"Please Click on this link to confirm your email: {confirmationUrl}"
            );
            return Results.Ok(new { Message = "User created successfully!" });
        }

        _errorMessage = String.Join(", ",
            identityResult.Errors
                .Select(error => error.Description)
                .ToArray()
        );
        _logger.LogError(_errorMessage);
        return Results.Conflict(new { Message = _errorMessage });
    }

}
