using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Stocks.Queries;

public class FindStockQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
}

public class FindStockQueryHandler : IRequestHandler<FindStockQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly ILogger<Stock> _logger;
    private string _errorMessage;
    public FindStockQueryHandler (
        IDBQueryService dbQueryService,
        ILogger<Stock> logger
    )
    {
        _dbQueryService = dbQueryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        FindStockQuery query,
        CancellationToken cancellationToken
    )
    {
        var existedDetailedStock = await _dbQueryService.GetQuery<Stock>(
                e => e.ModelId == query.ModelId,
                "Model",
                "Model.Product",
                "Model.Brand"
            )
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
            })
            .FirstOrDefaultAsync();

        if(existedDetailedStock is null)
        {
            _errorMessage = $"Stock Record with Id: {query.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        return Results.Ok(existedDetailedStock);
    }

}
