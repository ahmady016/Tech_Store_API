using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Purchases.Queries;

public class ListPurchasesQuery : ListQuery {}

public class ListPurchasesQueryHandler : IRequestHandler<ListPurchasesQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListPurchasesQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListPurchasesQuery query,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        if (query.PageSize is not null && query.PageSize is not null)
        {
            var page = await _dbQueryService.GetPageAsync<Purchase>(
                _dbQueryService.GetQuery<Purchase>(),
                (int)query.PageSize,
                (int)query.PageNumber
            );
            result = Results.Ok(page);
        }
        else
        {
            var list = await _dbQueryService.GetAllAsync<Purchase>();
            result = Results.Ok(list);
        }
        return result;
    }
}
