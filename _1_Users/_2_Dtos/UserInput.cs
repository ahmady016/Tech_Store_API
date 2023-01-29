using System.ComponentModel.DataAnnotations;
using MediatR;

using TechStoreApi.DB.Common;

namespace TechStoreApi.Dtos;

public class UserInput : IRequest<IResult>
{
    [Required(ErrorMessage = "FullName is Required")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "FullName must between 5 and 50 characters")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "BirthDate is required")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "GenderId is required")]
    [Range(1, 2, ErrorMessage = "GenderId value must be 1 or 2")]
    public Gender GenderId { get; set; }

    [DataType(DataType.EmailAddress)]
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
