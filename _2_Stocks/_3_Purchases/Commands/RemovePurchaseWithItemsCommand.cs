using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;

namespace Purchases.Commands;

public class RemovePurchaseWithItemsCommand : IdInput {}

public class RemovePurchaseWithItemsCommandHandler : IRequestHandler<RemovePurchaseWithItemsCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Brand> _logger;
    private string _errorMessage;
    public RemovePurchaseWithItemsCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Brand> logger
    )
    {
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        RemovePurchaseWithItemsCommand command,
        CancellationToken cancellationToken
    )
    {
        // find existedPurchase with its items
        var existedPurchase = await _dbQueryService.GetQuery<Purchase>()
            .Include(e => e.Items)
            .Where(e => e.Id == command.Id)
            .FirstOrDefaultAsync();

        if(existedPurchase is null)
        {
            _errorMessage = $"Purchase Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        // remove existedPurchase with its items
        _dbCommandService.Remove<Purchase>(existedPurchase);
        _dbCommandService.RemoveRange<PurchaseItem>(existedPurchase.Items.ToList());
        await _dbCommandService.SaveChangesAsync();

        // update each model stock values and save it to db
        var modelIds = existedPurchase.Items.Select(e => e.ModelId).ToList();
        var stocks = await _dbQueryService.GetListAsync<Stock>(e => modelIds.Contains(e.ModelId));
        foreach (var stock in stocks)
        {
            var purchaseItem = existedPurchase.Items.FirstOrDefault(e => e.ModelId == stock.ModelId);
            if(purchaseItem is not null)
                stock.PurchaseUpdate(-purchaseItem.Quantity, -purchaseItem.TotalPrice);
        }
        _dbCommandService.UpdateRange<Stock>(stocks);
        await _dbCommandService.SaveChangesAsync();

        return Results.Ok(new { Message = $"Purchase Record with Id: {command.Id} with it's Items was Removed Successfully" });
    }
}
