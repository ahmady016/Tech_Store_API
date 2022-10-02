using System.Net;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Dtos;
using Entities;

namespace Purchases.Commands;

public class RemovePurchaseItemCommand : IdInput {}

public class RemovePurchaseItemCommandHandler : IRequestHandler<RemovePurchaseItemCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<PurchaseItem> _logger;
    private string _errorMessage;
    public RemovePurchaseItemCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<PurchaseItem> logger
    )
    {
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        RemovePurchaseItemCommand command,
        CancellationToken cancellationToken
    )
    {
        // get the existed purchaseItem
        var existedPurchaseItem = await _dbQueryService.FindAsync<PurchaseItem>(command.Id);
        if (existedPurchaseItem is not null)
        {
            _errorMessage = $"PurchaseItem with Id: {command.Id} was Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // remove the existed purchaseItem and save it to db
        _dbCommandService.Remove<PurchaseItem>(existedPurchaseItem);
        await _dbCommandService.SaveChangesAsync();

        // update model stock values and save it to db
        var stock = await _dbQueryService.GetOneAsync<Stock>(e => e.ModelId == existedPurchaseItem.ModelId);
        if(stock is not null)
        {
            stock.PurchaseUpdate(-existedPurchaseItem.Quantity, -existedPurchaseItem.TotalPrice);
            _dbCommandService.Update<Stock>(stock);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(_mapper.Map<PurchaseItemDto>(new { Message = $"PurchaseItem Record with Id: {command.Id} was Removed Successfully" }));
    }

}
