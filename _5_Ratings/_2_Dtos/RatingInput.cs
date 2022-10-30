using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Dtos;

public class RatingInput : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }

    [Required(ErrorMessage = "Value is Required")]
    [Range(1, 5)]
    public byte Value { get; set; }
}
