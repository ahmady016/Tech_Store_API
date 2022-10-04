using System.Net;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Dtos;
using Entities;

namespace Sales.Commands;

public class RemoveSaleItemCommand : IdInput {}

public class RemoveSaleItemCommandHandler : IRequestHandler<RemoveSaleItemCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<SaleItem> _logger;
    private string _errorMessage;
    public RemoveSaleItemCommandHandler(
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
        RemoveSaleItemCommand command,
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

        // remove the existed SaleItem and save it to db
        _dbCommandService.Remove<SaleItem>(existedSaleItem);
        await _dbCommandService.SaveChangesAsync();

        // update model stock values and save it to db
        var stock = await _dbQueryService.GetOneAsync<Stock>(e => e.ModelId == existedSaleItem.ModelId);
        if(stock is not null)
        {
            stock.SaleUpdate(-existedSaleItem.Quantity, -existedSaleItem.TotalPrice);
            _dbCommandService.Update<Stock>(stock);
            await _dbCommandService.SaveChangesAsync();
        }

        return Results.Ok(_mapper.Map<SaleItemDto>(new { Message = $"SaleItem Record with Id: {command.Id} was Removed Successfully" }));
    }

}
