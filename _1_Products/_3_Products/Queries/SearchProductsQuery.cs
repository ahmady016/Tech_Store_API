using DB;
using Dtos;
using Entities;
using MediatR;

namespace Products.Queries;
public class SearchProductsQuery : IRequest<IResult>
{
    public string Where { get; set; }
    public string Select { get; set; }
    public string OrderBy { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IResult>
{
    private readonly ICrudService _crudService;
    public SearchProductsQueryHandler(ICrudService crudService)
    {
        _crudService = crudService;
    }

    public async Task<IResult> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (request.PageSize is not null && request.PageSize is not null)
            result = Results.Ok(
                _crudService.QueryPage<Product, ProductDto>(request.Where, request.Select, request.OrderBy, (int)request.PageSize, (int)request.PageNumber)
            );
        else
            result = Results.Ok(
                _crudService.Query<Product, ProductDto>(request.Where, request.Select, request.OrderBy)
            );

        return await Task.FromResult(result);
    }

}
