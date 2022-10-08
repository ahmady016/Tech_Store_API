using Microsoft.EntityFrameworkCore;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Stocks.Queries;

public class ListStocksQuery : ListQuery {}

public class ListStocksQueryHandler : IRequestHandler<ListStocksQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    public ListStocksQueryHandler (IDBQueryService dbQueryService)
    {
        _dbQueryService = dbQueryService;
    }

    public async Task<IResult> Handle(
        ListStocksQuery query,
        CancellationToken cancellationToken
    )
    {
        var dbQuery = _dbQueryService.GetQuery<Stock>()
            .Include("Model")
            .Include("Model.Product")
            .Include("Model.Brand")
            .Select(e => new DetailedStockDto()
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
            });

        IResult result;
        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService
                .GetPageAsync<DetailedStockDto>(dbQuery, (int)query.PageSize, (int)query.PageNumber);
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
