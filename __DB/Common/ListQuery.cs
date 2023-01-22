using MediatR;

namespace TechStoreApi.Common;

public class ListQuery : IRequest<IResult>
{
    public string Type { get; set; } = "existing";
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}
