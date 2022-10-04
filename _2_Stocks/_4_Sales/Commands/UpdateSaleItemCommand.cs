using System.Net;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Dtos;
using Entities;

namespace Sales.Commands;

public class UpdateSaleItemCommand : UpdateCommand<CreateSaleItemCommand> {}

public class UpdateSaleItemCommandHandler : IRequestHandler<UpdateSaleItemCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<SaleItem> _logger;
    private string _errorMessage;
    public UpdateSaleItemCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<SaleItem> logger
    )
    {
        _dbCommandService = dbCommandService;
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateSaleItemCommand command,
        CancellationToken cancellationToken
    )
    {
        // get the existed SaleItem
        var existedSaleItem = await _dbQueryService.FindAsync<SaleItem>(command.Id);
        if (existedSaleItem is not null)
        {
            _errorMessage = $"SaleItem with Id: {command.Id} was Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
        }

        // get the updated SaleItem
        var updatedSaleItem = _mapper.Map<SaleItem>(command.ModifiedEntity);
        updatedSaleItem.Id = command.Id;
        // calc item totalPrice if not provided
        if (updatedSaleItem.TotalPrice == 0.0)
            updatedSaleItem.TotalPrice = updatedSaleItem.UnitPrice * updatedSaleItem.Quantity;

        // update the existed SaleItem and save it to db
        _dbCommandService.Update<SaleItem>(updatedSaleItem);
        await _dbCommandService.SaveChangesAsync();

        // update model stock values and save it to db
        var stock = await _dbQueryService.GetOneAsync<Stock>(e => e.ModelId == updatedSaleItem.ModelId);
        if(stock is not null)
        {
            // calc the diff values between old and new SaleItem
            var diffQuantity = updatedSaleItem.Quantity - existedSaleItem.Quantity;
            var diffTotalPrice = updatedSaleItem.TotalPrice - existedSaleItem.TotalPrice;
            stock.SaleUpdate(diffQuantity, diffTotalPrice);
            _dbCommandService.Update<Stock>(stock);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(_mapper.Map<SaleItemDto>(updatedSaleItem));
    }

}
