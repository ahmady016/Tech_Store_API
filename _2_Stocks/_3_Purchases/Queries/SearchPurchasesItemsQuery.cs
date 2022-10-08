using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using MediatR;

using Common;
using DB;
using Entities;
using Dtos;

namespace Purchases.Queries;

public class SearchPurchasesItemsQuery : SearchQuery {}

public class SearchPurchasesItemsQueryHandler : IRequestHandler<SearchPurchasesItemsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly ILogger<Purchase> _logger;
    private string _errorMessage;
    private Expression<Func<PurchaseItem, DetailedPurchaseItemDto>> _selector = item => new DetailedPurchaseItemDto()
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
        };
    public SearchPurchasesItemsQueryHandler(
        IDBQueryService dbQueryService,
        ILogger<Purchase> logger
    )
    {
        _dbQueryService = dbQueryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        SearchPurchasesItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.Where is null && request.Select is null && request.OrderBy is null)
        {
            _errorMessage = "SearchPurchasesItems: Must supply at least one of the following: [where, select, orderBy]";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(new { Message = _errorMessage });
        }

        var query = _dbQueryService.GetQuery<PurchaseItem>();
        if (request.Where is not null)
            query = query.Where(request.Where);
        if (request.OrderBy is not null)
            query = query.OrderBy(request.OrderBy.RemoveEmptyElements(','));
        if (request.Select is not null)
            query = query.Select(request.Select.RemoveEmptyElements(',')) as IQueryable<PurchaseItem>;

        IResult result;
        if (request.PageSize is not null && request.PageNumber is not null)
        {
            if(request.Select is not null)
            {
                var page = await _dbQueryService.GetPageAsync<PurchaseItem>(query, (int)request.PageSize, (int)request.PageNumber);
                result = Results.Ok(page);
            }
            else
            {
                var page = await _dbQueryService.GetPageAsync<DetailedPurchaseItemDto>(
                    query.Include(e => e.Purchase)
                        .Include(e => e.Model)
                        .Select(_selector),
                    (int)request.PageSize,
                    (int)request.PageNumber
                );
                result = Results.Ok(page);
            }
        }
        else
        {
            if(request.Select is not null)
            {
                var list = await query.ToDynamicListAsync();
                result = Results.Ok(list);
            }
            else
            {
                var list = await query.Include(e => e.Purchase)
                    .Include(e => e.Model)
                    .Select(_selector)
                    .ToListAsync();
                result = Results.Ok(list);
            }
        }

        return result;
    }

}
