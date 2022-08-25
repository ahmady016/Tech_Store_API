using MediatR;
using System.ComponentModel.DataAnnotations;
namespace Common;

public class IdInput : IRequest<IResult>
{
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid Id value"
    )]
    public Guid Id { get; set; }
}
