using MediatR;

namespace TechStoreApi.Common;

public class SearchQuery : IRequest<IResult>
{
    public string Where { get; set; }
    public string OrderBy { get; set; }
    public string Select { get; set; }
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}
