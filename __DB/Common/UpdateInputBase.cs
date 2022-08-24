using MediatR;
namespace Common;

public class UpdateInputBase : IRequest<IResult>
{
    public Guid Id { get; set; }
}
