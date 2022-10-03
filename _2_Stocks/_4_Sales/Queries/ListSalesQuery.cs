using AutoMapper;
using MediatR;

using DB;
using Common;
using DB.Common;
using Entities;
using Dtos;

namespace Sales.Queries;

public class ListSalesQuery : ListQuery {}

public class ListSalesQueryHandler : IRequestHandler<ListSalesQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListSalesQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListSalesQuery query,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (query.PageSize is not null && query.PageSize is not null)
        {
            var page = await _dbQueryService.GetPageAsync<Sale>(
                _dbQueryService.GetQuery<Sale>(),
                (int)query.PageSize,
                (int)query.PageNumber
            );
            result = Results.Ok(new PageResult<SaleDto>()
            {
                PageItems = _mapper.Map<List<SaleDto>>(page.PageItems),
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages
            });
        }
        else
        {
            var list = await _dbQueryService.GetAllAsync<Sale>();
            result = Results.Ok(_mapper.Map<List<SaleDto>>(list));
        }
        return result;
    }
}
