using MediatR;

namespace TechStoreApi.Common;

public class IdInput : IRequest<IResult>
{
    public Guid Id { get; set; }
}
