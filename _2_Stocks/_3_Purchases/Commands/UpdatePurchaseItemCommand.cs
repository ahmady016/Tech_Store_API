using System.Net;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Dtos;
using Entities;

namespace Purchases.Commands;

public class UpdatePurchaseItemCommand : UpdateCommand<CreatePurchaseItemCommand> {}

public class UpdatePurchaseItemCommandHandler : IRequestHandler<UpdatePurchaseItemCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<PurchaseItem> _logger;
    private string _errorMessage;
    public UpdatePurchaseItemCommandHandler(
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
        UpdatePurchaseItemCommand command,
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

        // get the updated purchaseItem
        var updatedPurchaseItem = _mapper.Map<PurchaseItem>(command.ModifiedEntity);
        updatedPurchaseItem.Id = command.Id;
        // calc item totalPrice if not provided
        if (updatedPurchaseItem.TotalPrice == 0.0)
            updatedPurchaseItem.TotalPrice = updatedPurchaseItem.UnitPrice * updatedPurchaseItem.Quantity;

        // update the existed purchaseItem and save it to db
        _dbCommandService.Update<PurchaseItem>(updatedPurchaseItem);
        await _dbCommandService.SaveChangesAsync();

        // update model stock values and save it to db
        var stock = await _dbQueryService.GetOneAsync<Stock>(e => e.ModelId == updatedPurchaseItem.ModelId);
        if(stock is not null)
        {
            // calc the diff values between old and new purchaseItem
            var diffQuantity = updatedPurchaseItem.Quantity - existedPurchaseItem.Quantity;
            var diffTotalPrice = updatedPurchaseItem.TotalPrice - existedPurchaseItem.TotalPrice;
            stock.PurchaseUpdate(diffQuantity, diffTotalPrice);
            _dbCommandService.Update<Stock>(stock);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(_mapper.Map<PurchaseItemDto>(updatedPurchaseItem));
    }

}
