using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Purchases.Queries;

public class ListPurchasesItemsQuery : ListQuery {}

public class ListPurchasesItemsQueryHandler : IRequestHandler<ListPurchasesItemsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    public ListPurchasesItemsQueryHandler (
        IDBQueryService dbQueryService,
        IMapper mapper
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        ListPurchasesItemsQuery query,
        CancellationToken cancellationToken
    )
    {
        IResult result;
        var dbQuery = _dbQueryService.GetQuery<PurchaseItem>()
                .Include(e => e.Purchase)
                .Include(e => e.Model)
                .Select(item => new DetailedPurchaseItemDto()
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    ModelId = item.ModelId,
                    ModelTitle = item.Model.Title,
                    PurchaseId = item.PurchaseId,
                    PurchasedAt = item.Purchase.PurchasedAt,
                    EmployeeId = item.Purchase.EmployeeId
                });

        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService
                .GetPageAsync<DetailedPurchaseItemDto>(dbQuery, (int)query.PageSize, (int)query.PageNumber);
            result = Results.Ok(page);
        }
        else
        {
            var list = await dbQuery.ToListAsync();
            result = Results.Ok(list);
        }
        return result;
    }
}
