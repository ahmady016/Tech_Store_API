using MediatR;

namespace Common;
public class ListQuery : IRequest<IResult>
{
    public string ListType { get; set; } = "existed";
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}
