using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using MediatR;

using Common;
using DB;
using Entities;
using Dtos;

namespace Sales.Queries;

public class SearchSalesItemsQuery : SearchQuery {}

public class SearchSalesItemsQueryHandler : IRequestHandler<SearchSalesItemsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly ILogger<Sale> _logger;
    private string _errorMessage;
    private Expression<Func<SaleItem, DetailedSaleItemDto>> _selector = item => new DetailedSaleItemDto()
        {
            Id = item.Id,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            ModelId = item.ModelId,
            ModelTitle = item.Model.Title,
            SaleId = item.SaleId,
            SoldAt = item.Sale.SoldAt,
            EmployeeId = item.Sale.EmployeeId
        };
    public SearchSalesItemsQueryHandler(
        IDBQueryService dbQueryService,
        ILogger<Sale> logger
    )
    {
        _dbQueryService = dbQueryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        SearchSalesItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.Where is null && request.Select is null && request.OrderBy is null)
        {
            _errorMessage = "SearchSalesItems: Must supply at least one of the following: [where, select, orderBy]";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(new { Message = _errorMessage });
        }

        var query = _dbQueryService.GetQuery<SaleItem>();
        if (request.Where is not null)
            query = query.Where(request.Where);
        if (request.OrderBy is not null)
            query = query.OrderBy(request.OrderBy.RemoveEmptyElements(','));
        if (request.Select is not null)
            query = query.Select(request.Select.RemoveEmptyElements(',')) as IQueryable<SaleItem>;

        IResult result;
        if (request.PageSize is not null && request.PageSize is not null)
        {
            if(request.Select is not null)
            {
                var page = await _dbQueryService.GetPageAsync<SaleItem>(query, (int)request.PageSize, (int)request.PageNumber);
                result = Results.Ok(page);
            }
            else
            {
                var page = await _dbQueryService.GetPageAsync<DetailedSaleItemDto>(
                    query.Include(e => e.Sale)
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
                var list = await query.Include(e => e.Sale)
                    .Include(e => e.Model)
                    .Select(_selector)
                    .ToListAsync();
                result = Results.Ok(list);
            }
        }

        return result;
    }

}
