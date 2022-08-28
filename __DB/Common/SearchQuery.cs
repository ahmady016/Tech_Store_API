using MediatR;

namespace Common;
public class SearchQuery : IRequest<IResult>
{
    public string Where { get; set; }
    public string Select { get; set; }
    public string OrderBy { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}
