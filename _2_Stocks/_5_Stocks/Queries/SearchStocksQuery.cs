using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using MediatR;

using Common;
using DB;
using Entities;
using Dtos;

namespace Stocks.Queries;

public class SearchStocksQuery : SearchQuery {}

public class SearchStocksQueryHandler : IRequestHandler<SearchStocksQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly ILogger<Sale> _logger;
    private string _errorMessage;
    private Expression<Func<Stock, DetailedStockDto>> _selector = e => new DetailedStockDto()
    {
        ModelId = e.ModelId,
        ModelTitle = e.Model.Title,
        ModelDescription = e.Model.Description,
        ModelThumbUrl = e.Model.ThumbUrl,
        ProductId = e.Model.ProductId,
        ProductTitle = e.Model.Product.Title,
        ProductCategory = e.Model.Product.Category,
        BrandId = e.Model.BrandId,
        BrandTitle = e.Model.Brand.Title,
        TotalPurchasesPrice = e.TotalPurchasesPrice,
        TotalSalesPrice = e.TotalSalesPrice,
        Profit = e.Profit,
        TotalPurchasesQuantity = e.TotalPurchasesQuantity,
        TotalSalesQuantity = e.TotalSalesQuantity,
        TotalInStock = e.TotalInStock
    };
    public SearchStocksQueryHandler (
        IDBQueryService dbQueryService,
        ILogger<Sale> logger
    )
    {
        _dbQueryService = dbQueryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        SearchStocksQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.Where is null && request.Select is null && request.OrderBy is null)
        {
            _errorMessage = "SearchStocks: Must supply at least one of the following: [where, select, orderBy]";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(new { Message = _errorMessage });
        }

        var query = _dbQueryService.GetQuery<Stock>()
            .Include("Model")
            .Include("Model.Product")
            .Include("Model.Brand");

        if (request.Where is not null)
            query = query.Where(request.Where);
        if (request.OrderBy is not null)
            query = query.OrderBy(request.OrderBy.RemoveEmptyElements(','));
        if (request.Select is not null)
            query = query.Select(request.Select.RemoveEmptyElements(',')) as IQueryable<Stock>;

        IResult result;
        if (request.PageSize is not null && request.PageSize is not null)
        {
            if(request.Select is not null)
            {
                var page = await _dbQueryService.GetPageAsync<Stock>(
                    query,
                    (int)request.PageSize,
                    (int)request.PageNumber
                );
                result = Results.Ok(page);
            }
            else
            {
                var page = await _dbQueryService.GetPageAsync<DetailedStockDto>(
                    query.Select(_selector),
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
                var list = await query.Select(_selector).ToListAsync();
                result = Results.Ok(list);
            }
        }

        return result;
    }

}
