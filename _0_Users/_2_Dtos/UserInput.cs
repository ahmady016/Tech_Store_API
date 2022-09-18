using MediatR;
using System.ComponentModel.DataAnnotations;

using Entities;

namespace Dtos;

public class UserInput : IRequest<IResult>
{
    [Required(ErrorMessage = "FirstName is Required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "FirstName must between 5 and 100 characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is Required")]
    [StringLength(70, MinimumLength = 5, ErrorMessage = "LastName must between 10 and 400 characters")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "BirthDate is required")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Email Must be between 10 and 100 characters")]
    [EmailAddress(ErrorMessage = "Email Must be a valid email")]
    public string Email { get; set; }

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
